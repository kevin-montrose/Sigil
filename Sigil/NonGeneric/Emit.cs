using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Sigil.NonGeneric
{
    /// <summary>
    /// Helper for CIL generation that fails as soon as a sequence of instructions
    /// can be shown to be invalid.
    /// 
    /// Unlike Emit&lt;DelegateType&gt;, does not require a known delegate type to construct.
    /// However, if possible use Emit&lt;DelegateType&gt; so as to avoid common type mistakes.
    /// </summary>
    public partial class Emit
    {
        private Emit<NonGenericPlaceholderDelegate> InnerEmit;
        private Module Module;
        private string Name;
        private Type ReturnType;
        private Type[] ParameterTypes;

        private Delegate CreatedDelegate;
        private MethodBuilder CreatedMethod;
        private ConstructorBuilder CreatedConstructor;

        private NonGenericEmitType EmitType;

        private TypeBuilder TypeBuilder;
        private MethodAttributes Attributes;
        private CallingConventions CallingConvention;

        private EmitShorthand Shorthand;

        /// <summary>
        /// Lookup for declared labels by name.
        /// </summary>
        public LabelLookup Labels { get { return InnerEmit.Labels; } }

        /// <summary>
        /// Returns the maxmimum number of items on the stack for the IL stream created with the current emit.
        /// 
        /// This is not the maximum that *can be placed*, but the maximum that actually are.
        /// </summary>
        public int MaxStackSize { get { return InnerEmit.MaxStackSize; } }

        /// <summary>
        /// Lookup for the locals currently in scope by name.
        /// 
        /// Locals go out of scope when released (by calling Dispose() directly, or via using) and go into scope
        /// immediately after a DeclareLocal()
        /// </summary>
        public LocalLookup Locals { get { return InnerEmit.Locals; } }

        /// <summary>
        /// Returns true if this Emit can make use of unverifiable instructions.
        /// </summary>
        public bool AllowsUnverifiableCIL { get { return InnerEmit.AllowsUnverifiableCIL; } }

        private Emit(Emit<NonGenericPlaceholderDelegate> innerEmit, NonGenericEmitType type)
        {
            InnerEmit = innerEmit;
            EmitType = type;

            innerEmit.IsBuildingConstructor = type == NonGenericEmitType.Constructor;

            Shorthand = new EmitShorthand(this);
        }

        private static void ValidateReturnAndParameterTypes(Type returnType, Type[] parameterTypes)
        {
            if (returnType == null)
            {
                throw new ArgumentNullException("returnType");
            }

            if (parameterTypes == null)
            {
                throw new ArgumentNullException("parameterTypes");
            }

            for (var i = 0; i < parameterTypes.Length; i++)
            {
                var parameterType = parameterTypes[i];
                if (parameterType == null)
                {
                    throw new ArgumentException("parameterTypes contains a null reference at index " + i);
                }
            }
        }

        /// <summary>
        /// Creates a new EmitNonGeneric, optionally using the provided name and module for the inner DynamicMethod.
        /// 
        /// If name is not defined, a sane default is generated.
        /// 
        /// If module is not defined, a module with the same trust as the executing assembly is used instead.
        /// 
        /// If doVerify is false (default is true) Sigil will *not* throw an exception on invalid IL.  This is faster, but the benefits
        /// of Sigil are reduced to "a nicer ILGenerator interface".
        /// 
        /// If strictBranchValidation is true (default is false) Sigil will enforce "Backward branch constraints" which are *technically* required
        /// for valid CIL, but in practice often ignored.  The most common case to set this option is if you are generating types to write to disk.
        /// </summary>
        public static Emit NewDynamicMethod(Type returnType, Type[] parameterTypes, string name = null, ModuleBuilder module = null, bool doVerify = true, bool strictBranchVerification = false)
        {
            ValidateReturnAndParameterTypes(returnType, parameterTypes);

            module = module ?? Emit<NonGenericPlaceholderDelegate>.Module;

            var innerEmit = Emit<NonGenericPlaceholderDelegate>.MakeNonGenericEmit(CallingConventions.Standard, returnType, parameterTypes, Emit<NonGenericPlaceholderDelegate>.AllowsUnverifiableCode(module), doVerify, strictBranchVerification);

            var ret = new Emit(innerEmit, NonGenericEmitType.DynamicMethod);
            ret.Module = module;
            ret.Name = name ?? AutoNamer.Next("_DynamicMethod");
            ret.ReturnType = returnType;
            ret.ParameterTypes = parameterTypes;

            return ret;
        }

        private void ValidateDelegateType(Type delegateType)
        {
            var baseTypes = new LinqHashSet<Type>();
            baseTypes.Add(delegateType);
            var bType = TypeHelpers.GetBaseType(delegateType);
            while (bType != null)
            {
                baseTypes.Add(bType);
                bType = TypeHelpers.GetBaseType(bType);
            }

            if (!baseTypes.Contains(typeof(Delegate)))
            {
                throw new ArgumentException("delegateType must be a delegate, found " + delegateType.FullName);
            }

            var invoke = delegateType.GetMethod("Invoke");
            var returnType = invoke.ReturnType;

            if (returnType != ReturnType)
            {
                throw new ArgumentException("Expected delegateType to return " + ReturnType + ", found " + returnType);
            }

            var parameterTypes = invoke.GetParameters();

            if (parameterTypes.Length != ParameterTypes.Length)
            {
                throw new ArgumentException("Expected delegateType to take " + ParameterTypes.Length + " parameters, found " + parameterTypes.Length);
            }

            for (var i = 0; i < parameterTypes.Length; i++)
            {
                var actualType = parameterTypes[i].ParameterType;
                var expectedType = ParameterTypes[i];

                if (actualType != expectedType)
                {
                    throw new ArgumentException("Expected delegateType's parameter at index " + i + " to be a " + expectedType + ", found " + actualType);
                }
            }
        }

        /// <summary>
        /// Converts the CIL stream into a delegate.
        /// 
        /// Validation that cannot be run until a method is finished is run, and various instructions
        /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
        /// 
        /// Once this method is called the Emit may no longer be modified.
        /// 
        /// `instructions` will be set to a representation of the instructions making up the returned delegate.
        /// Note that this string is typically *not* enough to regenerate the delegate, it is available for
        /// debugging purposes only.  Consumers may find it useful to log the instruction stream in case
        /// the returned delegate fails validation (indicative of a bug in Sigil) or
        /// behaves unexpectedly (indicative of a logic bug in the consumer code).
        /// </summary>
        public Delegate CreateDelegate(Type delegateType, out string instructions, OptimizationOptions optimizationOptions = OptimizationOptions.All)
        {
            if (EmitType != NonGenericEmitType.DynamicMethod)
            {
                throw new InvalidOperationException("Emit was not created to build a DynamicMethod, thus CreateDelegate cannot be called");
            }

            ValidateDelegateType(delegateType);

            if (InnerEmit.DynMethod == null)
            {
                var dynMethod = new DynamicMethod(Name, ReturnType, ParameterTypes, Module, skipVisibility: true);

                InnerEmit.DynMethod = dynMethod;
            }

            if (CreatedDelegate != null)
            {
                instructions = null;
                return CreatedDelegate;
            }

            CreatedDelegate = InnerEmit.InnerCreateDelegate(delegateType, out instructions, optimizationOptions);

            return CreatedDelegate;
        }

        /// <summary>
        /// Converts the CIL stream into a delegate.
        /// 
        /// Validation that cannot be run until a method is finished is run, and various instructions
        /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
        /// 
        /// Once this method is called the Emit may no longer be modified.
        /// </summary>
        public Delegate CreateDelegate(Type delegateType, OptimizationOptions optimizationOptions = OptimizationOptions.All)
        {
            string ignored;
            return CreateDelegate(delegateType, out ignored, optimizationOptions);
        }

        /// <summary>
        /// Converts the CIL stream into a delegate.
        /// 
        /// Validation that cannot be run until a method is finished is run, and various instructions
        /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
        /// 
        /// Once this method is called the Emit may no longer be modified.
        /// 
        /// `instructions` will be set to a representation of the instructions making up the returned delegate.
        /// Note that this string is typically *not* enough to regenerate the delegate, it is available for
        /// debugging purposes only.  Consumers may find it useful to log the instruction stream in case
        /// the returned delegate fails validation (indicative of a bug in Sigil) or
        /// behaves unexpectedly (indicative of a logic bug in the consumer code).
        /// </summary>
        public DelegateType CreateDelegate<DelegateType>(out string instructions, OptimizationOptions optimizationOptions = OptimizationOptions.All)
        {
            return (DelegateType)(object)CreateDelegate(typeof(DelegateType), out instructions, optimizationOptions);
        }

        /// <summary>
        /// Converts the CIL stream into a delegate.
        /// 
        /// Validation that cannot be run until a method is finished is run, and various instructions
        /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
        /// 
        /// Once this method is called the Emit may no longer be modified.
        /// </summary>
        public DelegateType CreateDelegate<DelegateType>(OptimizationOptions optimizationOptions = OptimizationOptions.All)
        {
            string ignored;
            return CreateDelegate<DelegateType>(out ignored, optimizationOptions);
        }

        private static bool HasFlag(CallingConventions value, CallingConventions flag)
        {
            return (value & flag) != 0;
        }

        /// <summary>
        /// Creates a new Emit, suitable for building a method on the given TypeBuilder.
        /// 
        /// The DelegateType and MethodBuilder must agree on return types, parameter types, and parameter counts.
        /// 
        /// If you intend to use unveriable code, you must set allowUnverifiableCode to true.
        /// 
        /// If doVerify is false (default is true) Sigil will *not* throw an exception on invalid IL.  This is faster, but the benefits
        /// of Sigil are reduced to "a nicer ILGenerator interface".
        /// 
        /// If strictBranchValidation is true (default is false) Sigil will enforce "Backward branch constraints" which are *technically* required
        /// for valid CIL, but in practice often ignored.  The most common case to set this option is if you are generating types to write to disk.
        /// </summary>
        public static Emit BuildMethod(Type returnType, Type[] parameterTypes, TypeBuilder type, string name, MethodAttributes attributes, CallingConventions callingConvention, bool allowUnverifiableCode = false, bool doVerify = true, bool strictBranchVerification = false)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            Emit<NonGenericPlaceholderDelegate>.CheckAttributesAndConventions(attributes, callingConvention);

            ValidateReturnAndParameterTypes(returnType, parameterTypes);

            var passedParameterTypes = parameterTypes;

            if (HasFlag(callingConvention, CallingConventions.HasThis))
            {
                // Shove `this` in front, can't require it because it doesn't exist yet!
                var pList = new List<Type>(parameterTypes);
                pList.Insert(0, TypeHelpers.AsType(type));

                parameterTypes = pList.ToArray();
            }

            var innerEmit = Emit<NonGenericPlaceholderDelegate>.MakeNonGenericEmit(callingConvention, returnType, parameterTypes, allowUnverifiableCode, doVerify, strictBranchVerification);

            var ret = new Emit(innerEmit, NonGenericEmitType.Method);
            ret.Name = name;
            ret.ReturnType = returnType;
            ret.ParameterTypes = passedParameterTypes;
            ret.Attributes = attributes;
            ret.CallingConvention = callingConvention;
            ret.TypeBuilder = type;

            return ret;
        }

        /// <summary>
        /// Convenience method for creating instance methods.
        /// 
        /// Equivalent to calling to BuildMethod, but with CallingConventions.HasThis.
        /// </summary>
        public static Emit BuildInstanceMethod(Type returnType, Type[] parameterTypes, TypeBuilder type, string name, MethodAttributes attributes, bool allowUnverifiableCode = false, bool doVerify = true, bool strictBranchVerification = false)
        {
            return BuildMethod(returnType, parameterTypes, type, name, attributes, CallingConventions.HasThis, allowUnverifiableCode, doVerify, strictBranchVerification);
        }

        /// <summary>
        /// Convenience method for creating static methods.
        /// 
        /// Equivalent to calling to BuildMethod, but with MethodAttributes.Static set and CallingConventions.Standard.
        /// </summary>
        public static Emit BuildStaticMethod(Type returnType, Type[] parameterTypes, TypeBuilder type, string name, MethodAttributes attributes, bool allowUnverifiableCode = false, bool doVerify = true, bool strictBranchVerification = false)
        {
            return BuildMethod(returnType, parameterTypes, type, name, attributes | MethodAttributes.Static, CallingConventions.Standard, allowUnverifiableCode, doVerify, strictBranchVerification);
        }

        /// <summary>
        /// Writes the CIL stream out to the MethodBuilder used to create this Emit.
        /// 
        /// Validation that cannot be run until a method is finished is run, and various instructions
        /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
        /// 
        /// Once this method is called the Emit may no longer be modified.
        /// 
        /// Returns a MethodBuilder, which can be used to define overrides or for further inspection.
        /// 
        /// `instructions` will be set to a representation of the instructions making up the returned method.
        /// Note that this string is typically *not* enough to regenerate the method, it is available for
        /// debugging purposes only.  Consumers may find it useful to log the instruction stream in case
        /// the returned method fails validation (indicative of a bug in Sigil) or
        /// behaves unexpectedly (indicative of a logic bug in the consumer code).
        /// </summary>
        public MethodBuilder CreateMethod(out string instructions, OptimizationOptions optimizationOptions = OptimizationOptions.All)
        {
            if (EmitType != NonGenericEmitType.Method)
            {
                throw new InvalidOperationException("Emit was not created to build a method, thus CreateMethod cannot be called");
            }

            if (CreatedMethod != null)
            {
                instructions = null;
                return CreatedMethod;
            }

            var methodBuilder = TypeBuilder.DefineMethod(Name, Attributes, CallingConvention, ReturnType, ParameterTypes);

            InnerEmit.MtdBuilder = methodBuilder;

            CreatedMethod = InnerEmit.CreateMethod(out instructions, optimizationOptions);

            return CreatedMethod;
        }

        /// <summary>
        /// Writes the CIL stream out to the MethodBuilder used to create this Emit.
        /// 
        /// Validation that cannot be run until a method is finished is run, and various instructions
        /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
        /// 
        /// Once this method is called the Emit may no longer be modified.
        /// 
        /// Returns a MethodBuilder, which can be used to define overrides or for further inspection.
        /// </summary>
        public MethodBuilder CreateMethod(OptimizationOptions optimizationOptions = OptimizationOptions.All)
        {
            string ignored;
            return CreateMethod(out ignored, optimizationOptions);
        }

        /// <summary>
        /// Creates a new Emit, suitable for building a constructor on the given TypeBuilder.
        /// 
        /// The DelegateType and TypeBuilder must agree on parameter types and parameter counts.
        /// 
        /// If you intend to use unveriable code, you must set allowUnverifiableCode to true.
        /// 
        /// If doVerify is false (default is true) Sigil will *not* throw an exception on invalid IL.  This is faster, but the benefits
        /// of Sigil are reduced to "a nicer ILGenerator interface".
        /// 
        /// If strictBranchValidation is true (default is false) Sigil will enforce "Backward branch constraints" which are *technically* required
        /// for valid CIL, but in practice often ignored.  The most common case to set this option is if you are generating types to write to disk.
        /// </summary>
        public static Emit BuildConstructor(Type[] parameterTypes, TypeBuilder type, MethodAttributes attributes, CallingConventions callingConvention = CallingConventions.HasThis, bool allowUnverifiableCode = false, bool doVerify = true, bool strictBranchVerification = false)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            Emit<NonGenericPlaceholderDelegate>.CheckAttributesAndConventions(attributes, callingConvention);

            if (!HasFlag(callingConvention, CallingConventions.HasThis))
            {
                throw new ArgumentException("Constructors always have a this reference");
            }

            ValidateReturnAndParameterTypes(type
#if COREFX
                .AsType()
#endif
                , parameterTypes);

            var passedParameters = parameterTypes;

            // Constructors always have a `this`
            var pList = new List<Type>(parameterTypes);
            pList.Insert(0, TypeHelpers.AsType(type));

            parameterTypes = pList.ToArray();

            var innerEmit = Emit<NonGenericPlaceholderDelegate>.MakeNonGenericEmit(callingConvention, typeof(void), parameterTypes, allowUnverifiableCode, doVerify, strictBranchVerification);
            innerEmit.ConstructorDefinedInType = TypeHelpers.AsType(type);

            var ret = new Emit(innerEmit, NonGenericEmitType.Constructor);
            ret.ReturnType = TypeHelpers.AsType(type);
            ret.ParameterTypes = passedParameters;
            ret.Attributes = attributes;
            ret.CallingConvention = callingConvention;
            ret.TypeBuilder = type;

            return ret;
        }

        /// <summary>
        /// Creates a new Emit, suitable for building a type initializer on the given TypeBuilder.
        /// 
        /// If you intend to use unveriable code, you must set allowUnverifiableCode to true.
        /// 
        /// If doVerify is false (default is true) Sigil will *not* throw an exception on invalid IL.  This is faster, but the benefits
        /// of Sigil are reduced to "a nicer ILGenerator interface".
        /// 
        /// If strictBranchValidation is true (default is false) Sigil will enforce "Backward branch constraints" which are *technically* required
        /// for valid CIL, but in practice often ignored.  The most common case to set this option is if you are generating types to write to disk.
        /// </summary>
        public static Emit BuildTypeInitializer(TypeBuilder type, bool allowUnverifiableCode = false, bool doVerify = true, bool strictBranchVerification = false)
        {
            if (type == null) 
            {
                throw new ArgumentNullException("type");
            }

            var innerEmit = Emit<NonGenericPlaceholderDelegate>.MakeNonGenericEmit(CallingConventions.Standard, typeof(void), TypeHelpers.EmptyTypes, allowUnverifiableCode, doVerify, strictBranchVerification);
            innerEmit.ConstructorDefinedInType = TypeHelpers.AsType(type);

            var ret = new Emit(innerEmit, NonGenericEmitType.TypeInitializer);
            ret.ReturnType = TypeHelpers.AsType(type);
            ret.ParameterTypes = TypeHelpers.EmptyTypes;
            ret.Attributes = 0;
            ret.CallingConvention = CallingConventions.Standard;
            ret.TypeBuilder = type;

            return ret;
        }

        /// <summary>
        /// Writes the CIL stream out to the ConstructorBuilder used to create this Emit.
        /// 
        /// Validation that cannot be run until a method is finished is run, and various instructions
        /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
        /// 
        /// Once this method is called the Emit may no longer be modified.
        /// 
        /// Returns a ConstructorBuilder, which can be used to define overrides or for further inspection.
        /// 
        /// `instructions` will be set to a representation of the instructions making up the returned constructor.
        /// Note that this string is typically *not* enough to regenerate the constructor, it is available for
        /// debugging purposes only.  Consumers may find it useful to log the instruction stream in case
        /// the returned constructor fails validation (indicative of a bug in Sigil) or
        /// behaves unexpectedly (indicative of a logic bug in the consumer code).
        /// </summary>
        public ConstructorBuilder CreateConstructor(out string instructions, OptimizationOptions optimizationOptions = OptimizationOptions.All)
        {
            if (EmitType != NonGenericEmitType.Constructor)
            {
                throw new InvalidOperationException("Emit was not created to build a constructor, thus CreateConstructor cannot be called");
            }

            if (CreatedConstructor != null)
            {
                instructions = null;
                return CreatedConstructor;
            }

            var constructorBuilder = TypeBuilder.DefineConstructor(Attributes, CallingConvention, ParameterTypes);

            InnerEmit.ConstrBuilder = constructorBuilder;

            CreatedConstructor = InnerEmit.CreateConstructor(out instructions, optimizationOptions);

            return CreatedConstructor;
        }

        /// <summary>
        /// Writes the CIL stream out to the ConstructorBuilder used to create this Emit.
        /// 
        /// Validation that cannot be run until a method is finished is run, and various instructions
        /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
        /// 
        /// Once this method is called the Emit may no longer be modified.
        /// 
        /// Returns a ConstructorBuilder, which can be used to define overrides or for further inspection.
        /// </summary>
        public ConstructorBuilder CreateConstructor(OptimizationOptions optimizationOptions = OptimizationOptions.All)
        {
            string ignored;
            return CreateConstructor(out ignored, optimizationOptions);
        }

        /// <summary>
        /// Writes the CIL stream out to the ConstructorBuilder used to create this Emit.
        /// 
        /// Validation that cannot be run until a method is finished is run, and various instructions
        /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
        /// 
        /// Once this method is called the Emit may no longer be modified.
        /// 
        /// Returns a ConstructorBuilder, which can be used to define overrides or for further inspection.
        /// 
        /// `instructions` will be set to a representation of the instructions making up the returned constructor.
        /// Note that this string is typically *not* enough to regenerate the constructor, it is available for
        /// debugging purposes only.  Consumers may find it useful to log the instruction stream in case
        /// the returned constructor fails validation (indicative of a bug in Sigil) or
        /// behaves unexpectedly (indicative of a logic bug in the consumer code).
        /// </summary>
        public ConstructorBuilder CreateTypeInitializer(out string instructions, OptimizationOptions optimizationOptions = OptimizationOptions.All) 
        {
            if (EmitType != NonGenericEmitType.TypeInitializer) {
                throw new InvalidOperationException("Emit was not created to build a type initializer, thus CreateTypeInitializer cannot be called");
            }

            if (CreatedConstructor != null) {
                instructions = null;
                return CreatedConstructor;
            }

            var constructorBuilder = TypeBuilder.DefineTypeInitializer();

            InnerEmit.ConstrBuilder = constructorBuilder;

            CreatedConstructor = InnerEmit.CreateTypeInitializer(out instructions, optimizationOptions);

            return CreatedConstructor;
        }

        /// <summary>
        /// Writes the CIL stream out to the ConstructorBuilder used to create this Emit.
        /// 
        /// Validation that cannot be run until a method is finished is run, and various instructions
        /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
        /// 
        /// Once this method is called the Emit may no longer be modified.
        /// 
        /// Returns a ConstructorBuilder, which can be used to define overrides or for further inspection.
        /// </summary>
        public ConstructorBuilder CreateTypeInitializer(OptimizationOptions optimizationOptions = OptimizationOptions.All) 
        {
            string ignored;
            return CreateTypeInitializer(out ignored, optimizationOptions);
        }

        /// <summary>
        /// Returns a string representation of the CIL opcodes written to this Emit to date.
        /// 
        /// This method is meant for debugging purposes only.
        /// </summary>
        public string Instructions()
        {
            return InnerEmit.Instructions();
        }

        /// <summary>
        /// Returns a proxy for this Emit that exposes method names that more closely
        /// match the fields on System.Reflection.Emit.OpCodes.
        /// 
        /// IF you're well versed in ILGenerator, the shorthand version may be easier to use.
        /// </summary>
        public EmitShorthand AsShorthand()
        {
            return Shorthand;
        }
    }
}
