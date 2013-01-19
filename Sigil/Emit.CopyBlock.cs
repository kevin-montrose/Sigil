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
        public void CopyBlock()
        {
            var onStack = Stack.Top(3);

            if (onStack == null)
            {
                throw new SigilException("CopyBlock expects three values to be on the stack", Stack);
            }

            var dest = onStack[2];
            var source = onStack[1];
            var count = onStack[0];

            if (!(dest.IsPointer || dest.IsReference || dest == TypeOnStack.Get<NativeInt>()))
            {
                throw new SigilException("CopyBlock expects the destination value to be a pointer, reference, or native int; found " + dest, Stack);
            }

            if (!(source.IsPointer || source.IsReference || source == TypeOnStack.Get<NativeInt>()))
            {
                throw new SigilException("CopyBlock expects the source value to be a pointer, reference, or native int; found " + source, Stack);
            }

            if (count != TypeOnStack.Get<int>())
            {
                throw new SigilException("CopyBlock expects the count value to be an int; found " + count, Stack);
            }

            if (source != dest)
            {
                throw new SigilException("CopyBlock expects source and destination types to match; found " + source + " and " + dest, Stack);
            }

            UpdateState(OpCodes.Cpblk, pop: 3);
        }
    }
}
