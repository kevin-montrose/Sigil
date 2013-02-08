using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Removes the top value on the stack.
        /// </summary>
        public Emit<DelegateType> Pop()
        {
            var onStack = Stack.Top();

            if (onStack == null)
            {
                throw new SigilVerificationException("Pop expects a value on the stack, but it was empty", IL, Stack);
            }

            UpdateState(OpCodes.Pop, pop: 1);

            return this;
        }
    }
}
