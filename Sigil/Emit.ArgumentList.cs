using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pushes a pointer to the current argument list onto the stack.
        /// 
        /// This instruction can only be used in VarArgs methods.
        /// </summary>
        public Emit<DelegateType> ArgumentList()
        {
            if (!AllowsUnverifiableCIL)
            {
                FailUnverifiable();
            }

            if (MtdBuilder == null || MtdBuilder.CallingConvention != System.Reflection.CallingConventions.VarArgs)
            {
                throw new InvalidOperationException("ArgumentList can only be called in VarArgs methods");
            }

            UpdateState(OpCodes.Arglist, StackTransition.Push<NativeIntType>().Wrap("ArgumentList"));

            return this;
        }
    }
}
