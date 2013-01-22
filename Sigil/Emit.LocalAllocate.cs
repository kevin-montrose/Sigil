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
        public void LocalAllocate()
        {
            if (CatchBlocks.Any(c => c.Value.Item2 == -1))
            {
                throw new SigilException("LocalAllocate cannot be used in a catch block", Stack);
            }

            if (FinallyBlocks.Any(f => f.Value.Item2 == -1))
            {
                throw new SigilException("LocalAllocate cannot be used in a finally block", Stack);
            }

            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("LocalAllocate expects a value on the stack, but it was empty", Stack);
            }

            var numBytes = top[0];

            if (numBytes != TypeOnStack.Get<int>() && numBytes != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilException("LocalAllocate expected an int or native int, found " + numBytes, Stack);
            }

            UpdateState(OpCodes.Localloc, TypeOnStack.Get<NativeInt>(), pop: 1);
        }
    }
}
