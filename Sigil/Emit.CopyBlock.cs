using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Expects a destination pointer, a source pointer, and a length on the stack.  Pops all three values.
        /// 
        /// Copies length bytes from destination to the source.
        /// </summary>
        public Emit<DelegateType> CopyBlock(bool isVolatile = false, int? unaligned = null)
        {
            if (unaligned.HasValue && (unaligned != 1 && unaligned != 2 && unaligned != 4))
            {
                throw new ArgumentException("unaligned must be null, 1, 2, or 4", "unaligned");
            }

            if (!AllowsUnverifiableCIL)
            {
                FailUnverifiable();
            }

            var onStack = Stack.Top(3);

            if (onStack == null)
            {
                FailStackUnderflow(3);
            }

            var dest = onStack[2];
            var source = onStack[1];
            var count = onStack[0];

            if (!(dest.IsPointer || dest.IsReference || dest == TypeOnStack.Get<NativeIntType>()))
            {
                throw new SigilVerificationException("CopyBlock expects the destination value to be a pointer, reference, or native int; found " + dest, IL.Instructions(LocalsByIndex), Stack, 2);
            }

            if (!(source.IsPointer || source.IsReference || source == TypeOnStack.Get<NativeIntType>()))
            {
                throw new SigilVerificationException("CopyBlock expects the source value to be a pointer, reference, or native int; found " + source, IL.Instructions(LocalsByIndex), Stack, 1);
            }

            if (count != TypeOnStack.Get<int>())
            {
                throw new SigilVerificationException("CopyBlock expects the count value to be an int; found " + count, IL.Instructions(LocalsByIndex), Stack, 0);
            }

            if (isVolatile)
            {
                UpdateState(OpCodes.Volatile);
            }

            if (unaligned.HasValue)
            {
                UpdateState(OpCodes.Unaligned, unaligned.Value);
            }

            UpdateState(OpCodes.Cpblk, pop: 3);

            return this;
        }
    }
}
