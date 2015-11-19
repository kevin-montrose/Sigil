using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Sigil
{
    /// <summary>
    /// Helper for CIL generation that fails as soon as a sequence of instructions
    /// can be shown to be invalid.
    /// </summary>
    /// <typeparam name="DelegateType">The type of delegate being built</typeparam>
    public partial class Emit<DelegateType>
    {
        internal static readonly ModuleBuilder Module;

        static Emit()
        {
#if COREFX
            var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Sigil.Emit.DynamicAssembly"), AssemblyBuilderAccess.Run);
#else
            var asm = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Sigil.Emit.DynamicAssembly"), AssemblyBuilderAccess.Run);
#endif
            Module = asm.DefineDynamicModule("DynamicModule");
        }

        private bool Invalidated;

        private BufferedILGenerator<DelegateType> IL;
        private TypeOnStack ReturnType;
        private Type[] ParameterTypes;
        private CallingConventions CallingConventions;

        private LinqList<VerifiableTracker> Trackers;

        private ushort NextLocalIndex = 0;

        private LinqList<Local> AllLocals;

        private LinqHashSet<Local> UnusedLocals;
        private LinqHashSet<Label> UnusedLabels;
        private LinqHashSet<Label> UnmarkedLabels;

        private LinqList<SigilTuple<OpCode, Label, int>> Branches;
        private LinqDictionary<Label, int> Marks;
        private LinqList<int> Returns;
        private LinqList<int> Throws;

        private LinqDictionary<int, SigilTuple<Label, UpdateOpCodeDelegate, OpCode>> BranchPatches;

        private Stack<ExceptionBlock> CurrentExceptionBlock;

        private LinqDictionary<ExceptionBlock, SigilTuple<int, int>> TryBlocks;
        private LinqDictionary<CatchBlock, SigilTuple<int, int>> CatchBlocks;
        private LinqDictionary<FinallyBlock, SigilTuple<int, int>> FinallyBlocks;

        private LinqList<SigilTuple<int, TypeOnStack>> ReadonlyPatches;

        private EmitShorthand<DelegateType> Shorthand;

        // These can only ever be set if we're building a DynamicMethod
        private DelegateType CreatedDelegate;
        internal DynamicMethod DynMethod { get; set; }

        // These can only ever be set if we're building a MethodBuilder
        internal MethodBuilder MtdBuilder { get; set; }
        private bool MethodBuilt;

        internal ConstructorBuilder ConstrBuilder { get; set; }
        internal bool IsBuildingConstructor { get; set; }
        internal Type ConstructorDefinedInType { get; set; }
        private bool ConstructorBuilt;

        /// <summary>
        /// Returns true if this Emit can make use of unverifiable instructions.
        /// </summary>
        public bool AllowsUnverifiableCIL { get; private set; }

        private int _MaxStackSize;
        /// <summary>
        /// Returns the maxmimum number of items on the stack for the IL stream created with the current emit.
        /// 
        /// This is not the maximum that *can be placed*, but the maximum that actually are.
        /// </summary>
        public int MaxStackSize
        {
            get
            {
                if (!IsVerifying)
                {
                    throw new InvalidOperationException("MaxStackSize is not available on non-verifying Emits");
                }

                return _MaxStackSize;
            }
            private set
            {
                _MaxStackSize = value;
            }
        }

        private LinqList<Local> FreedLocals { get; set; }

        private LinqDictionary<string, Local> CurrentLocals;

        /// <summary>
        /// Lookup for the locals currently in scope by name.
        /// 
        /// Locals go out of scope when released (by calling Dispose() directly, or via using) and go into scope
        /// immediately after a DeclareLocal()
        /// </summary>
        public LocalLookup Locals { get; private set; }

        private LinqDictionary<string, Label> CurrentLabels;

        /// <summary>
        /// Lookup for declared labels by name.
        /// </summary>
        public LabelLookup Labels { get; private set; }

        private RollingVerifier CurrentVerifiers;

        private bool MustMark;

        private LinqList<int> ElidableCasts;

        private LinqDictionary<int, LinqList<TypeOnStack>> TypesProducedAtIndex;

        private bool IsVerifying;

        private bool UsesStrictBranchVerification;

        private Emit(CallingConventions callConvention, Type returnType, Type[] parameterTypes, bool allowUnverifiable, bool doVerify, bool strictBranchVerification)
        {
            CallingConventions = callConvention;

            AllowsUnverifiableCIL = allowUnverifiable;

            IsVerifying = doVerify;
            UsesStrictBranchVerification = strictBranchVerification;

            ReturnType = TypeOnStack.Get(returnType);
            ParameterTypes = parameterTypes;

            IL = new BufferedILGenerator<DelegateType>();
            
            Trackers = new LinqList<VerifiableTracker>();

            AllLocals = new LinqList<Local>();

            UnusedLocals = new LinqHashSet<Local>();
            UnusedLabels = new LinqHashSet<Label>();
            UnmarkedLabels = new LinqHashSet<Label>();

            Branches = new LinqList<SigilTuple<OpCode, Label, int>>();
            Marks = new LinqDictionary<Label, int>();
            Returns = new LinqList<int>();
            Throws = new LinqList<int>();

            BranchPatches = new LinqDictionary<int, SigilTuple<Label, UpdateOpCodeDelegate, OpCode>>();

            CurrentExceptionBlock = new Stack<ExceptionBlock>();

            TryBlocks = new LinqDictionary<ExceptionBlock, SigilTuple<int, int>>();
            CatchBlocks = new LinqDictionary<CatchBlock, SigilTuple<int, int>>();
            FinallyBlocks = new LinqDictionary<FinallyBlock, SigilTuple<int, int>>();

            ReadonlyPatches = new LinqList<SigilTuple<int, TypeOnStack>>();

            Shorthand = new EmitShorthand<DelegateType>(this);

            FreedLocals = new LinqList<Local>();

            CurrentLocals = new LinqDictionary<string, Local>();
            Locals = new LocalLookup(CurrentLocals);

            CurrentLabels = new LinqDictionary<string, Label>();
            Labels = new LabelLookup(CurrentLabels);

            ElidableCasts = new LinqList<int>();

            TypesProducedAtIndex = new LinqDictionary<int, LinqList<TypeOnStack>>();

            var start = DefineLabel("__start");
            CurrentVerifiers = IsVerifying ? new RollingVerifier(start, UsesStrictBranchVerification) : new RollingVerifierWithoutVerification(start);
            MarkLabel(start);
        }

        internal static Emit<NonGenericPlaceholderDelegate> MakeNonGenericEmit(CallingConventions callConvention, Type returnType, Type[] parameterTypes, bool allowUnverifiable, bool doVerify, bool strictBranchVerification)
        {
            return new Emit<NonGenericPlaceholderDelegate>(callConvention, returnType, parameterTypes, allowUnverifiable, doVerify, strictBranchVerification);
        }

        /// <summary>
        /// Returns a proxy for this Emit that exposes method names that more closely
        /// match the fields on System.Reflection.Emit.OpCodes.
        /// 
        /// IF you're well versed in ILGenerator, the shorthand version may be easier to use.
        /// </summary>
        public EmitShorthand<DelegateType> AsShorthand()
        {
            return Shorthand;
        }

        /// <summary>
        /// Returns a string representation of the CIL opcodes written to this Emit to date.
        /// 
        /// This method is meant for debugging purposes only.
        /// </summary>
        public string Instructions()
        {
            var ret = new StringBuilder();

            foreach (var line in ((LinqArray<string>)IL.Instructions(AllLocals)).Skip(2).AsEnumerable())
            {
                ret.AppendLine(line);
            }

            return ret.ToString().Trim();
        }

        /// <summary>
        /// Returns the current instruction offset (effectively, the length of the CIL stream to date).
        /// 
        /// This does not necessarily increase monotonically, as rewrites can cause it to shrink.
        /// 
        /// Likewise the effect of any given call is not guaranteed to be the same under all circumstance, as current and future
        /// state may influence opcode choice.
        /// 
        /// This method is meant for debugging purposes only.
        /// </summary>
        public int ILOffset()
        {
            return IL.ByteDistance(0, IL.Index);
        }

        private void Seal(OptimizationOptions optimizationOptions)
        {
            if ((optimizationOptions & ~OptimizationOptions.All) != 0)
            {
                throw new ArgumentException("optimizationOptions contained unknown flags, found " + optimizationOptions);
            }

            if ((optimizationOptions & OptimizationOptions.EnableTrivialCastEliding) != 0)
            {
                ElideCasts();
            }

            InjectTailCall();
            InjectReadOnly();

            if ((optimizationOptions & OptimizationOptions.EnableBranchPatching) != 0)
            {
                PatchBranches();
            }

            Validate();

            Invalidated = true;
        }

        /// <summary>
        /// Traces where the values produced by certain operations are used.
        /// 
        /// For example:
        ///   ldc.i4 32
        ///   ldc.i4 64
        ///   add
        ///   ret
        ///   
        /// Would be represented by a series of OperationResultUsage like so:
        ///   - (lcd.i4 32) -> add
        ///   - (ldc.i4 64) -> add
        ///   - (add) -> ret
        /// </summary>
        public IEnumerable<OperationResultUsage<DelegateType>> TraceOperationResultUsage()
        {
            var ret = new List<OperationResultUsage<DelegateType>>();

            foreach (var r in TypesProducedAtIndex.AsEnumerable())
            {
                var allUsage = new LinqList<InstructionAndTransitions>(r.Value.SelectMany(k => k.UsedBy.Select(u => u.Item1)).AsEnumerable());

                var usedBy = new List<Operation<DelegateType>>(allUsage.Select(u => IL.Operations[u.InstructionIndex.Value]).Distinct().AsEnumerable());

                if (r.Key < IL.Operations.Count)
                {
                    var key = IL.Operations[r.Key];

                    if (key != null)
                    {
                        ret.Add(new OperationResultUsage<DelegateType>(key, usedBy, r.Value.AsEnumerable()));
                    }
                }
            }

            return ret;
        }

        internal Delegate InnerCreateDelegate(Type delegateType, out string instructions, OptimizationOptions optimizationOptions)
        {
            Seal(optimizationOptions);

            var il = DynMethod.GetILGenerator();
            instructions = IL.UnBuffer(il);

            AutoNamer.Release(this);

            return DynMethod.CreateDelegate(delegateType);
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
        public DelegateType CreateDelegate(out string instructions, OptimizationOptions optimizationOptions = OptimizationOptions.All)
        {
            if (DynMethod == null)
            {
                throw new InvalidOperationException("Emit was not created to build a DynamicMethod, thus CreateDelegate cannot be called");
            }

            if (CreatedDelegate != null)
            {
                instructions = null;
                return CreatedDelegate;
            }

            CreatedDelegate = (DelegateType)(object)InnerCreateDelegate(typeof(DelegateType), out instructions, optimizationOptions);

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
        public DelegateType CreateDelegate(OptimizationOptions optimizationOptions = OptimizationOptions.All)
        {
            string ignored;
            return CreateDelegate(out ignored, optimizationOptions);
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
            if (MtdBuilder == null)
            {
                throw new InvalidOperationException("Emit was not created to build a method, thus CreateMethod cannot be called");
            }

            if (MethodBuilt)
            {
                instructions = null;
                return MtdBuilder;
            }

            Seal(optimizationOptions);

            MethodBuilt = true;

            var il = MtdBuilder.GetILGenerator();
            instructions = IL.UnBuffer(il);

            AutoNamer.Release(this);

            return MtdBuilder;
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
            if (ConstrBuilder == null || !IsBuildingConstructor)
            {
                throw new InvalidOperationException("Emit was not created to build a constructor, thus CreateConstructor cannot be called");
            }

            if (ConstructorBuilt)
            {
                instructions = null;
                return ConstrBuilder;
            }

            ConstructorBuilt = true;

            Seal(optimizationOptions);

            var il = ConstrBuilder.GetILGenerator();
            instructions = IL.UnBuffer(il);

            AutoNamer.Release(this);

            return ConstrBuilder;
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
            if (ConstrBuilder == null || IsBuildingConstructor) 
            {
                throw new InvalidOperationException("Emit was not created to build a type initializer, thus CreateTypeInitializer cannot be called");
            }

            if (ConstructorBuilt) 
            {
                instructions = null;
                return ConstrBuilder;
            }

            ConstructorBuilt = true;

            Seal(optimizationOptions);

            var il = ConstrBuilder.GetILGenerator();
            instructions = IL.UnBuffer(il);

            AutoNamer.Release(this);

            return ConstrBuilder;
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

        private static void ValidateNewParameters<CheckDelegateType>()
        {
            var delType = typeof(CheckDelegateType);

            var baseTypes = new LinqHashSet<Type>();
            baseTypes.Add(delType);
#if COREFX
            var bType = delType.GetTypeInfo().BaseType;
#else
            var bType = delType.BaseType;
#endif
            while (bType != null)
            {
                baseTypes.Add(bType);
                bType = TypeHelpers.GetBaseType(bType);
            }

            if (!baseTypes.Contains(typeof(Delegate)))
            {
                throw new ArgumentException("DelegateType must be a delegate, found " + delType.FullName);
            }
        }

#if !COREFX // see https://github.com/dotnet/corefx/issues/4543 item 2
        internal static bool AllowsUnverifiableCode(Module m)
        {
            return Attribute.IsDefined(m, typeof(System.Security.UnverifiableCodeAttribute));
        }
#endif

        internal static bool AllowsUnverifiableCode(ModuleBuilder m)
        {
            var canaryMethod = new DynamicMethod("__Canary" + Guid.NewGuid(), typeof(void), TypeHelpers.EmptyTypes, m);
            var il = canaryMethod.GetILGenerator();
            il.Emit(OpCodes.Ldc_I4, 1024);
            il.Emit(OpCodes.Localloc);
            il.Emit(OpCodes.Pop);
            il.Emit(OpCodes.Ret);

            var d1 = (SigilAction)canaryMethod.CreateDelegate(typeof(SigilAction));

            try
            {
                d1();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a new Emit, optionally using the provided name and module for the inner DynamicMethod.
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
        public static Emit<DelegateType> NewDynamicMethod(string name = null, ModuleBuilder module = null, bool doVerify = true, bool strictBranchVerification = false)
        {
            return DisassemblerDynamicMethod(name: name, module: module, doVerify: doVerify, strictBranchVerification: strictBranchVerification);
        }

        internal static Emit<DelegateType> DisassemblerDynamicMethod(Type[] parameters = null, string name = null, ModuleBuilder module = null, bool doVerify = true, bool strictBranchVerification = false)
        {
            module = module ?? Module;

            name = name ?? AutoNamer.Next("_DynamicMethod");
            
            ValidateNewParameters<DelegateType>();

            var delType = typeof(DelegateType);

            var invoke = delType.GetMethod("Invoke");
            var returnType = invoke.ReturnType;
            var parameterTypes = parameters ?? LinqAlternative.Select(invoke.GetParameters(), s => s.ParameterType).ToArray();

            var dynMethod = new DynamicMethod(name, returnType, parameterTypes, module, skipVisibility: true);

            var ret = new Emit<DelegateType>(dynMethod.CallingConvention, returnType, parameterTypes, AllowsUnverifiableCode(module), doVerify, strictBranchVerification);
            ret.DynMethod = dynMethod;

            return ret;
        }

        /// <summary>
        /// Creates a new Emit, optionally using the provided name and owner for the inner DynamicMethod.
        /// 
        /// If name is not defined, a sane default is generated.
        /// 
        /// If owner is not defined, a module with the same trust as the executing assembly is used instead.
        /// 
        /// If doVerify is false (default is true) Sigil will *not* throw an exception on invalid IL.  This is faster, but the benefits
        /// of Sigil are reduced to "a nicer ILGenerator interface".
        /// 
        /// If strictBranchValidation is true (default is false) Sigil will enforce "Backward branch constraints" which are *technically* required
        /// for valid CIL, but in practice often ignored.  The most common case to set this option is if you are generating types to write to disk.
        /// </summary>
        public static Emit<DelegateType> NewDynamicMethod(Type owner, string name = null, bool doVerify = true, bool strictBranchVerification = false)
        {
            if (owner == null)
            {
                return NewDynamicMethod(name: name, module: null);
            }

            name = name ?? AutoNamer.Next("_DynamicMethod");

            ValidateNewParameters<DelegateType>();

            var delType = typeof(DelegateType);

            var invoke = delType.GetMethod("Invoke");
            var returnType = invoke.ReturnType;
            var parameterTypes = ((LinqArray<ParameterInfo>)invoke.GetParameters()).Select(s => s.ParameterType).ToArray();

            var dynMethod = new DynamicMethod(name, returnType, parameterTypes, owner, skipVisibility: true);

#if COREFX // see https://github.com/dotnet/corefx/issues/4543 item 2
            const bool allowUnverifiable = false;
#else
            bool allowUnverifiable = AllowsUnverifiableCode(TypeHelpers.GetModule(owner));
#endif
            var ret = new Emit<DelegateType>(dynMethod.CallingConvention, returnType, parameterTypes, allowUnverifiable, doVerify, strictBranchVerification);
            ret.DynMethod = dynMethod;

            return ret;
        }

        internal static void CheckAttributesAndConventions(MethodAttributes attributes, CallingConventions callingConvention)
        {
            if ((attributes & ~(MethodAttributes.Abstract | MethodAttributes.Assembly | MethodAttributes.CheckAccessOnOverride | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.FamORAssem | MethodAttributes.Final | MethodAttributes.HasSecurity | MethodAttributes.HideBySig | MethodAttributes.MemberAccessMask | MethodAttributes.NewSlot | MethodAttributes.PinvokeImpl | MethodAttributes.Private | MethodAttributes.PrivateScope | MethodAttributes.Public | MethodAttributes.RequireSecObject | MethodAttributes.ReuseSlot | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.Static | MethodAttributes.UnmanagedExport | MethodAttributes.Virtual | MethodAttributes.VtableLayoutMask)) != 0)
            {
                throw new ArgumentException("Unrecognized flag in attributes");
            }

            if ((callingConvention & ~(CallingConventions.Any | CallingConventions.ExplicitThis | CallingConventions.HasThis | CallingConventions.Standard | CallingConventions.VarArgs)) != 0)
            {
                throw new ArgumentException("Unrecognized flag in callingConvention");
            }

            if (HasFlag(attributes, MethodAttributes.Static) && HasFlag(callingConvention, CallingConventions.HasThis))
            {
                throw new ArgumentException("Static methods cannot have a this reference");
            }
        }

        private static bool HasFlag(MethodAttributes value, MethodAttributes flag)
        {
            return (value & flag) != 0;
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
        public static Emit<DelegateType> BuildMethod(TypeBuilder type, string name, MethodAttributes attributes, CallingConventions callingConvention, bool allowUnverifiableCode = false, bool doVerify = true, bool strictBranchVerification = false)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            CheckAttributesAndConventions(attributes, callingConvention);

            ValidateNewParameters<DelegateType>();

            var delType = typeof(DelegateType);

            var invoke = delType.GetMethod("Invoke");
            var returnType = invoke.ReturnType;
            var parameterTypes = ((LinqArray<ParameterInfo>)invoke.GetParameters()).Select(s => s.ParameterType).ToArray();

            var methodBuilder = type.DefineMethod(name, attributes, callingConvention, returnType, parameterTypes);

            if (HasFlag(callingConvention, CallingConventions.HasThis))
            {
                // Shove `this` in front, can't require it because it doesn't exist yet!
                var pList = new List<Type>(parameterTypes);
                pList.Insert(0, TypeHelpers.AsType(type));

                parameterTypes = pList.ToArray();
            }

            var ret = new Emit<DelegateType>(callingConvention, returnType, parameterTypes, allowUnverifiableCode, doVerify, strictBranchVerification);
            ret.MtdBuilder = methodBuilder;

            return ret;
        }

        /// <summary>
        /// Convenience method for creating static methods.
        /// 
        /// Equivalent to calling to BuildMethod, but with MethodAttributes.Static set and CallingConventions.Standard.
        /// </summary>
        public static Emit<DelegateType> BuildStaticMethod(TypeBuilder type, string name, MethodAttributes attributes, bool allowUnverifiableCode = false, bool doVerify = true)
        {
            return BuildMethod(type, name, attributes | MethodAttributes.Static, CallingConventions.Standard, allowUnverifiableCode, doVerify);
        }

        /// <summary>
        /// Convenience method for creating instance methods.
        /// 
        /// Equivalent to calling to BuildMethod, but with CallingConventions.HasThis.
        /// </summary>
        public static Emit<DelegateType> BuildInstanceMethod(TypeBuilder type, string name, MethodAttributes attributes, bool allowUnverifiableCode = false, bool doVerify = true)
        {
            return BuildMethod(type, name, attributes, CallingConventions.HasThis, allowUnverifiableCode, doVerify);
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
        public static Emit<DelegateType> BuildConstructor(TypeBuilder type, MethodAttributes attributes, CallingConventions callingConvention = CallingConventions.HasThis, bool allowUnverifiableCode = false, bool doVerify = true, bool strictBranchVerification = false)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            CheckAttributesAndConventions(attributes, callingConvention);

            if (!HasFlag(callingConvention, CallingConventions.HasThis))
            {
                throw new ArgumentException("Constructors always have a this reference");
            }

            ValidateNewParameters<DelegateType>();

            var delType = typeof(DelegateType);

            var invoke = delType.GetMethod("Invoke");
            var returnType = invoke.ReturnType;
            var parameterTypes = LinqAlternative.Select(invoke.GetParameters(), s => s.ParameterType).ToArray();

            if (returnType != typeof(void))
            {
                throw new ArgumentException("DelegateType used must return void");
            }

            var constructorBuilder = type.DefineConstructor(attributes, callingConvention, parameterTypes);

            // Constructors always have a `this`
            var pList = new List<Type>(parameterTypes);
            pList.Insert(0, TypeHelpers.AsType(type));

            parameterTypes = pList.ToArray();

            var ret = new Emit<DelegateType>(callingConvention, typeof(void), parameterTypes, allowUnverifiableCode, doVerify, strictBranchVerification);
            ret.ConstrBuilder = constructorBuilder;
            ret.IsBuildingConstructor = true;

            return ret;
        }

        /// <summary>
        /// Creates a new Emit, suitable for building a type initializer on the given TypeBuilder.
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
        public static Emit<DelegateType> BuildTypeInitializer(TypeBuilder type, bool allowUnverifiableCode = false, bool doVerify = true, bool strictBranchVerification = false) 
        {
            if (type == null) 
            {
                throw new ArgumentNullException("type");
            }

            ValidateNewParameters<DelegateType>();

            var delType = typeof(DelegateType);

            var invoke = delType.GetMethod("Invoke");
            var returnType = invoke.ReturnType;
            var parameters = invoke.GetParameters();

            if (returnType != typeof(void)) 
            {
                throw new ArgumentException("DelegateType used must return void");
            }

            if (parameters.Length > 0) 
            {
                throw new ArgumentException("A type initializer can have no arguments.");
            }

            var constructorBuilder = type.DefineTypeInitializer();

            var ret = new Emit<DelegateType>(CallingConventions.Standard, typeof(void), TypeHelpers.EmptyTypes, allowUnverifiableCode, doVerify, strictBranchVerification);
            ret.ConstrBuilder = constructorBuilder;

            return ret;
        }

        private void RemoveInstruction(int index)
        {
            IL.Remove(index);

            // We need to update our state to account for the new insertion
            foreach (var v in Branches.Where(w => w.Item3 >= index).ToList().AsEnumerable())
            {
                Branches.Remove(v);
                Branches.Add(SigilTuple.Create(v.Item1, v.Item2, v.Item3 - 1));
            }

            foreach (var kv in Marks.Where(w => w.Value >= index).ToList().AsEnumerable())
            {
                Marks[kv.Key] = kv.Value - 1;
            }

            for (var i = 0; i < Returns.Count; i++)
            {
                if (Returns[i] >= index)
                {
                    Returns[i] = Returns[i] - 1;
                }
            }

            var needUpdateKeys = BranchPatches.Keys.Where(k => k >= index).ToList();

            foreach (var key in needUpdateKeys.AsEnumerable())
            {
                var cur = BranchPatches[key];
                BranchPatches.Remove(key);
                BranchPatches[key - 1] = cur;
            }

            foreach (var kv in TryBlocks.Where(kv => kv.Value.Item1 >= index).ToList().AsEnumerable())
            {
                TryBlocks[kv.Key] = SigilTuple.Create(kv.Value.Item1 - 1, kv.Value.Item2);
            }
            foreach (var kv in TryBlocks.Where(kv => kv.Value.Item2 >= index).ToList().AsEnumerable())
            {
                TryBlocks[kv.Key] = SigilTuple.Create(kv.Value.Item1, kv.Value.Item2 - 1);
            }

            foreach (var kv in CatchBlocks.Where(kv => kv.Value.Item1 >= index).ToList().AsEnumerable())
            {
                CatchBlocks[kv.Key] = SigilTuple.Create(kv.Value.Item1 - 1, kv.Value.Item2);
            }

            foreach (var kv in CatchBlocks.Where(kv => kv.Value.Item2 >= index).ToList().AsEnumerable())
            {
                CatchBlocks[kv.Key] = SigilTuple.Create(kv.Value.Item1, kv.Value.Item2 - 1);
            }

            foreach (var kv in FinallyBlocks.Where(kv => kv.Value.Item1 >= index).ToList().AsEnumerable())
            {
                FinallyBlocks[kv.Key] = SigilTuple.Create(kv.Value.Item1 - 1, kv.Value.Item2);
            }

            foreach (var kv in FinallyBlocks.Where(kv => kv.Value.Item2 >= index).ToList().AsEnumerable())
            {
                FinallyBlocks[kv.Key] = SigilTuple.Create(kv.Value.Item1, kv.Value.Item2 - 1);
            }

            foreach (var elem in ReadonlyPatches.ToList().AsEnumerable())
            {
                if (elem.Item1 >= index)
                {
                    var update = SigilTuple.Create(elem.Item1 - 1, elem.Item2);

                    ReadonlyPatches.Remove(elem);
                    ReadonlyPatches.Add(elem);
                }
            }
        }

        private void InsertInstruction(int index, OpCode instr)
        {
            IL.Insert(index, instr);

            // We need to update our state to account for the new insertion
            foreach (var v in Branches.Where(w => w.Item3 >= index).ToList().AsEnumerable())
            {
                Branches.Remove(v);
                Branches.Add(SigilTuple.Create(v.Item1, v.Item2, v.Item3 + 1));
            }

            foreach (var kv in Marks.Where(w => w.Value >= index).ToList().AsEnumerable())
            {
                Marks[kv.Key] = kv.Value + 1;
            }

            for (var i = 0; i < Returns.Count; i++)
            {
                if (Returns[i] >= index)
                {
                    Returns[i] = Returns[i] + 1;
                }
            }

            var needUpdateKeys = BranchPatches.Keys.Where(k => k >= index).ToList();

            foreach (var key in needUpdateKeys.AsEnumerable())
            {
                var cur = BranchPatches[key];
                BranchPatches.Remove(key);
                BranchPatches[key + 1] = cur;
            }

            foreach (var kv in TryBlocks.Where(kv => kv.Value.Item1 >= index).ToList().AsEnumerable())
            {
                TryBlocks[kv.Key] = SigilTuple.Create(kv.Value.Item1 + 1, kv.Value.Item2);
            }

            foreach (var kv in TryBlocks.Where(kv => kv.Value.Item2 >= index).ToList().AsEnumerable())
            {
                TryBlocks[kv.Key] = SigilTuple.Create(kv.Value.Item1, kv.Value.Item2 + 1);
            }

            foreach (var kv in CatchBlocks.Where(kv => kv.Value.Item1 >= index).ToList().AsEnumerable())
            {
                CatchBlocks[kv.Key] = SigilTuple.Create(kv.Value.Item1 + 1, kv.Value.Item2);
            }

            foreach (var kv in CatchBlocks.Where(kv => kv.Value.Item2 >= index).ToList().AsEnumerable())
            {
                CatchBlocks[kv.Key] = SigilTuple.Create(kv.Value.Item1, kv.Value.Item2 + 1);
            }

            foreach (var kv in FinallyBlocks.Where(kv => kv.Value.Item1 >= index).ToList().AsEnumerable())
            {
                FinallyBlocks[kv.Key] = SigilTuple.Create(kv.Value.Item1 + 1, kv.Value.Item2);
            }

            foreach (var kv in FinallyBlocks.Where(kv => kv.Value.Item2 >= index).ToList().AsEnumerable())
            {
                FinallyBlocks[kv.Key] = SigilTuple.Create(kv.Value.Item1, kv.Value.Item2 + 1);
            }

            foreach (var elem in ReadonlyPatches.ToList().AsEnumerable())
            {
                if (elem.Item1 >= index)
                {
                    var update = SigilTuple.Create(elem.Item1 + 1, elem.Item2);

                    ReadonlyPatches.Remove(elem);
                    ReadonlyPatches.Add(elem);
                }
            }
        }

        private void UpdateStackAndInstrStream(OpCode? instr, TransitionWrapper transitions, bool firstParamIsThis = false)
        {
            if (Invalidated)
            {
                throw new InvalidOperationException("Cannot modify Emit after a delegate has been generated from it");
            }

            if (MustMark)
            {
                throw new SigilVerificationException("Unreachable code detected", IL.Instructions(AllLocals));
            }

            var wrapped = new InstructionAndTransitions(instr, instr.HasValue ? (int?)IL.Index : null, transitions.Transitions);

            TypesProducedAtIndex[IL.Index] = transitions.Transitions.SelectMany(t => t.PushedToStack).ToList();

            var verifyRes = CurrentVerifiers.Transition(wrapped);
            if (!verifyRes.Success)
            {
                throw new SigilVerificationException(transitions.MethodName, verifyRes, IL.Instructions(AllLocals));
            }

            if (IsVerifying)
            {
                MaxStackSize = Math.Max(verifyRes.StackSize, MaxStackSize);
            }
        }

        private void UpdateState(TransitionWrapper transitions)
        {
            UpdateStackAndInstrStream(null, transitions);
        }

        private void UpdateState(OpCode instr, TransitionWrapper transitions)
        {
            UpdateStackAndInstrStream(instr, transitions);

            IL.Emit(instr);
        }

        private void UpdateState(OpCode instr, byte param, TransitionWrapper transitions)
        {
            UpdateStackAndInstrStream(instr, transitions);

            IL.Emit(instr, param);
        }

        private void UpdateState(OpCode instr, short param, TransitionWrapper transitions)
        {
            UpdateStackAndInstrStream(instr, transitions);

            IL.Emit(instr, param);
        }

        private void UpdateState(OpCode instr, int param, TransitionWrapper transitions)
        {
            UpdateStackAndInstrStream(instr, transitions);

            IL.Emit(instr, param);
        }

        private void UpdateState(OpCode instr, uint param, TransitionWrapper transitions)
        {
            UpdateStackAndInstrStream(instr, transitions);

            IL.Emit(instr, param);
        }

        private void UpdateState(OpCode instr, long param, TransitionWrapper transitions)
        {
            UpdateStackAndInstrStream(instr, transitions);

            IL.Emit(instr, param);
        }

        private void UpdateState(OpCode instr, ulong param, TransitionWrapper transitions)
        {
            UpdateStackAndInstrStream(instr, transitions);

            IL.Emit(instr, param);
        }

        private void UpdateState(OpCode instr, float param, TransitionWrapper transitions)
        {
            UpdateStackAndInstrStream(instr, transitions);

            IL.Emit(instr, param);
        }

        private void UpdateState(OpCode instr, double param, TransitionWrapper transitions)
        {
            UpdateStackAndInstrStream(instr, transitions);

            IL.Emit(instr, param);
        }

        private void UpdateState(OpCode instr, Local param, TransitionWrapper transitions)
        {
            UpdateStackAndInstrStream(instr, transitions);

            IL.Emit(instr, param);
        }

        private void UpdateState(OpCode instr, Label param, TransitionWrapper transitions, out UpdateOpCodeDelegate update)
        {
            UpdateStackAndInstrStream(instr, transitions);

            IL.Emit(instr, param, out update);
        }

        private void UpdateState(OpCode instr, Label[] param, TransitionWrapper transitions, out UpdateOpCodeDelegate update)
        {
            UpdateStackAndInstrStream(instr, transitions);

            IL.Emit(instr, param, out update);
        }

        private void UpdateState(OpCode instr, MethodInfo method, IEnumerable<Type> parameterTypes, TransitionWrapper transitions, bool firstParamIsThis = false, Type[] arglist = null)
        {
            UpdateStackAndInstrStream(instr, transitions, firstParamIsThis);

            if (arglist == null)
            {
                IL.Emit(instr, method, parameterTypes);
            }
            else
            {
                IL.EmitCall(instr, method, parameterTypes, arglist);
            }
        }

        private void UpdateState(OpCode instr, ConstructorInfo cons, IEnumerable<Type> parameterTypes, TransitionWrapper transitions)
        {
            UpdateStackAndInstrStream(instr, transitions, firstParamIsThis: true);

            IL.Emit(instr, cons, parameterTypes);
        }

        private void UpdateState(OpCode instr, ConstructorInfo cons, TransitionWrapper transitions)
        {
            UpdateStackAndInstrStream(instr, transitions);

            IL.Emit(instr, cons);
        }

        private void UpdateState(OpCode instr, Type type, TransitionWrapper transitions)
        {
            UpdateStackAndInstrStream(instr, transitions);

            IL.Emit(instr, type);
        }

        private void UpdateState(OpCode instr, FieldInfo field, TransitionWrapper transitions)
        {
            UpdateStackAndInstrStream(instr, transitions);

            IL.Emit(instr, field);
        }

        private void UpdateState(OpCode instr, string str, TransitionWrapper transitions)
        {
            UpdateStackAndInstrStream(instr, transitions);

            IL.Emit(instr, str);
        }

        private void UpdateState(OpCode instr, CallingConventions callConventions, Type returnType, Type[] parameterTypes, TransitionWrapper transitions, Type[] arglist)
        {
            UpdateStackAndInstrStream(instr, transitions);

            if (arglist == null)
            {
                IL.Emit(instr, callConventions, returnType, parameterTypes);
            }
            else
            {
                IL.EmitCalli(callConventions, returnType, parameterTypes, arglist);
            }
        }

        private TransitionWrapper Wrap(IEnumerable<StackTransition> trans, string name)
        {
            return TransitionWrapper.Get(name, trans);
        }
    }
}
