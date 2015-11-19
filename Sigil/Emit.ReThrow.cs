using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// From within a catch block, rethrows the exception that caused the catch block to be entered.
        /// </summary>
        public Emit<DelegateType> ReThrow()
        {
            if(!CatchBlocks.Any(c => c.Value.Item2 == -1))
            {
                throw new InvalidOperationException("ReThrow is only legal in a catch block");
            }

            UpdateState(OpCodes.Rethrow, Wrap(StackTransition.None(), "ReThrow"));
            UpdateState(Wrap(new[] { new StackTransition(new[] { typeof(PopAllType) }, TypeHelpers.EmptyTypes) }, "ReThrow"));

            Throws.Add(IL.Index);

            MustMark = true;

            var verify = CurrentVerifiers.ReThrow();
            if (!verify.Success)
            {
                throw new SigilVerificationException("ReThrow", verify, IL.Instructions(AllLocals));
            }

            return this;
        }
    }
}
