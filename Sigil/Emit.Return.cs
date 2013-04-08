using Sigil.Impl;
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
                UpdateState(Wrap(new[] { new StackTransition(0) }, "Return"));

                UpdateState(OpCodes.Ret, Wrap(StackTransition.None(), "Return"));

                Returns.Add(IL.Index);
                MustMark = true;

                return this;
            }

            UpdateState(OpCodes.Ret, Wrap(StackTransition.Pop(ReturnType), "Return"));

            Returns.Add(IL.Index);

            UpdateState(Wrap(new[] { new StackTransition(0) }, "Return"));
            MustMark = true;

            var verify = CurrentVerifiers.Return();
            if (!verify.Success)
            {
                throw new SigilVerificationException("Return", verify, IL.Instructions(AllLocals));
            }

            return this;
        }
    }
}
