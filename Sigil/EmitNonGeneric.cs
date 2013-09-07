using System;
using Sigil.Impl;
using System.Reflection.Emit;

namespace Sigil
{
    /// <summary>
    /// Helper for CIL generation that fails as soon as a sequence of instructions
    /// can be shown to be invalid.
    /// 
    /// Unlike Emit&lt;DelegateType&gt;, does not require a known delegate type to construct.
    /// However, if possible use Emit&lt;DelegateType&gt; so as to avoid common type mistakes.
    /// </summary>
    public class EmitNonGeneric
    {
        private Emit<NonGenericPlaceholderDelegate> InnerEmit;

        private EmitNonGeneric(Emit<NonGenericPlaceholderDelegate> innerEmit)
        {
            InnerEmit = innerEmit;
        }

        private static void ValidateReturnAndParameterTypes(Type returnType, Type[] parameterTypes, ValidationOptions validationOptions)
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

            if ((validationOptions & ~ValidationOptions.All) != 0)
            {
                throw new ArgumentException("validationOptions contained unknown flags, found " + validationOptions);
            }
        }

        /// <summary>
        /// Creates a new EmitNonGeneric, optionally using the provided name and module for the inner DynamicMethod.
        /// 
        /// If name is not defined, a sane default is generated.
        /// 
        /// If module is not defined, a module with the same trust as the executing assembly is used instead.
        /// </summary>
        public static EmitNonGeneric NewDynamicMethod(Type returnType, Type[] parameterTypes, string name = null, ModuleBuilder module = null, ValidationOptions validationOptions = ValidationOptions.All)
        {
            ValidateReturnAndParameterTypes(returnType, parameterTypes, validationOptions);

            module = module ?? Emit<NonGenericPlaceholderDelegate>.Module;

            name = name ?? AutoNamer.Next("_DynamicMethod");

            var dynMethod = new DynamicMethod(name, returnType, parameterTypes, module, skipVisibility: true);

            var innerEmit = Emit<EmitNonGeneric>.MakeNonGenericEmit(dynMethod.CallingConvention, returnType, parameterTypes, Emit<NonGenericPlaceholderDelegate>.AllowsUnverifiableCode(module), validationOptions);
            innerEmit.DynMethod = dynMethod;

            var ret = new EmitNonGeneric(innerEmit);

            return ret;
        }
    }
}
