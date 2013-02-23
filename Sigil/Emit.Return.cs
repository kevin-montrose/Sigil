using Sigil.Impl;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Ends the execution of the current method.
        /// 
        /// If the current method does not return void, pops a value from the stack and returns it to the calling method.
        /// 
        /// Return should leave the stack empty.
        /// </summary>
        public Emit<DelegateType> Return()
        {
            if (ReturnType == TypeOnStack.Get(typeof(void)))
            {
                UpdateState((new[] { new StackTransition(0) }).Wrap("Return"));

                UpdateState(OpCodes.Ret, StackTransition.None().Wrap("Return"));

                CurrentVerifier = new VerifiableTracker(baseless: true);
                MustMark = true;

                return this;
            }

            UpdateState(OpCodes.Ret, StackTransition.Pop(ReturnType).Wrap("Return"));

            UpdateState((new[] { new StackTransition(0) }).Wrap("Return"));

            CurrentVerifier = new VerifiableTracker(baseless: true);
            MustMark = true;

            return this;
        }
    }
}
