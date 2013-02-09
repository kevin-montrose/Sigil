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
        /// Pushes a copy of the current top value on the stack.
        /// </summary>
        public Emit<DelegateType> Duplicate()
        {
            var onStack = Stack.Top();

            if (onStack == null)
            {
                FailStackUnderflow(1);
            }

            UpdateState(OpCodes.Dup, onStack[0]);

            return this;
        }
    }
}
