using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;

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
                throw new SigilVerificationException("Throw expected a value on the stack, but it was empty", IL, Stack);
            }

            var val = onStack[0];

            if (!typeof(Exception).IsAssignableFrom(val))
            {
                throw new SigilVerificationException("Throw expected an exception to be on the stack, found " + val, IL, Stack);
            }

            UpdateState(OpCodes.Throw, pop: 1);

            return this;
        }
    }
}
