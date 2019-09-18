using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// <para>Pushes a pointer to the current argument list onto the stack.</para>
        /// <para>This instruction can only be used in VarArgs methods.</para>
        /// </summary>
        public Emit<DelegateType> ArgumentList()
        {
            if (!AllowsUnverifiableCIL)
            {
                FailUnverifiable("ArgumentList");
            }
            
            if (CallingConventions != System.Reflection.CallingConventions.VarArgs)
            {
                throw new InvalidOperationException("ArgumentList can only be called in VarArgs methods");
            }

            UpdateState(OpCodes.Arglist, Wrap(StackTransition.Push<NativeIntType>(), "ArgumentList"));

            return this;
        }
    }
}
