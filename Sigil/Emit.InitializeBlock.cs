using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public void InitializeBlock()
        {
            var onStack = Stack.Top(3);

            if (onStack == null)
            {
                throw new SigilException("InitializeBlock expects three values to be on the stack", Stack);
            }

            var start = onStack[2];
            var init = onStack[1];
            var count = onStack[0];

            if (!start.IsPointer && !start.IsReference && start != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilException("InitializeBlock expects the start value to be a pointer, reference, or native int; found " + start, Stack);
            }

            if (init != TypeOnStack.Get<int>() && init != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilException("InitBlock expects the initial value to be an int or native int; found " + init, Stack);
            }

            if (count != TypeOnStack.Get<int>())
            {
                throw new SigilException("InitBlock expects the count to be an int; found " + count, Stack);
            }

            UpdateState(OpCodes.Initblk, pop: 3);
        }
    }
}
