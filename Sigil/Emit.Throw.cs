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
            var onStack = Stack.Top();

            if (onStack == null)
            {
                FailStackUnderflow(1);
            }

            var val = onStack[0];

            if (!typeof(Exception).IsAssignableFrom(val))
            {
                throw new SigilVerificationException("Throw expected an exception to be on the stack, found " + val, IL.Instructions(LocalsByIndex), Stack, 0);
            }

            UpdateState(OpCodes.Throw, StackTransition.Pop<Exception>(), pop: 1);

            return this;
        }
    }
}
