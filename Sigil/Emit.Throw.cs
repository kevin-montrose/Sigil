using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pops a value off the stack and throws it as an exception.
        /// 
        /// Throw expects the value to be or extend from a System.Exception.
        /// </summary>
        public Emit<DelegateType> Throw()
        {
            UpdateState(OpCodes.Throw, Wrap(StackTransition.Pop<Exception>(), "Throw"));
            UpdateState(Wrap(new[] { new StackTransition(new[] { typeof(PopAllType) }, TypeHelpers.EmptyTypes) }, "Throw"));

            Throws.Add(IL.Index);

            MustMark = true;

            var verify = CurrentVerifiers.Throw();
            if (!verify.Success)
            {
                throw new SigilVerificationException("Throw", verify, IL.Instructions(AllLocals));
            }

            return this;
        }
    }
}
