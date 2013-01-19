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
        public void Duplicate()
        {
            var onStack = Stack.Top();

            if (onStack == null)
            {
                throw new SigilException("Duplicate expects a value on the stack, but it was empty", Stack);
            }

            UpdateState(OpCodes.Dup, onStack[0]);
        }
    }
}
