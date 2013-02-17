using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pops a reference to a rank 1 array off the stack, and pushes it's length onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadLength(Type arrayType = null)
        {
            // TODO: with rolling validator, it may not be possible to infer the type here; we should still *try* but we need to handle
            //         the failure case

            var onStack = Stack.Top();

            if (onStack == null)
            {
                FailStackUnderflow(1);
            }

            var arr = onStack[0];

            if (arr.IsReference || arr.IsPointer || !arr.Type.IsArray || arr.Type.GetArrayRank() != 1)
            {
                throw new SigilVerificationException("LoadLength expects a rank 1 array, found " + arr, IL.Instructions(LocalsByIndex), Stack, 0);
            }

            var transitions =
                new[] {
                    new StackTransition(new [] { arrayType ?? arr.Type }, new [] { typeof(int) })
                };

            UpdateState(OpCodes.Ldlen, transitions.Wrap("LoadLength"), TypeOnStack.Get<int>(), pop: 1);

            return this;
        }
    }
}
