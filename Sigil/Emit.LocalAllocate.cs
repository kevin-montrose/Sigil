using Sigil.Impl;
using System;
using System.Linq;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pops a size from the stack, allocates size bytes on the local dynamic memory pool, and pushes a pointer to the allocated block.
        /// 
        /// LocalAllocate can only be called if the stack is empty aside from the size value.
        /// 
        /// Memory allocated with LocalAllocate is released when the current method ends execution.
        /// </summary>
        public Emit<DelegateType> LocalAllocate()
        {
            if (CatchBlocks.Any(c => c.Value.Item2 == -1))
            {
                throw new InvalidOperationException("LocalAllocate cannot be used in a catch block");
            }

            if (FinallyBlocks.Any(f => f.Value.Item2 == -1))
            {
                throw new InvalidOperationException("LocalAllocate cannot be used in a finally block");
            }

            if (!AllowsUnverifiableCIL)
            {
                FailUnverifiable();
            }

            var top = Stack.Top();

            if (top == null)
            {
                FailStackUnderflow(1);
            }

            if (Stack.Count() > 1)
            {
                throw new SigilVerificationException("LocalAllocate requires the stack only contain the size value", IL.Instructions(LocalsByIndex), Stack);
            }

            var numBytes = top[0];

            if (numBytes != TypeOnStack.Get<int>() && numBytes != TypeOnStack.Get<NativeIntType>())
            {
                throw new SigilVerificationException("LocalAllocate expected an int or native int, found " + numBytes, IL.Instructions(LocalsByIndex), Stack, 0);
            }

            var transitions =
                new[] {
                    new StackTransition(new [] { typeof(int) }, new [] { typeof(NativeIntType) }),
                    new StackTransition(new [] { typeof(NativeIntType) }, new [] { typeof(NativeIntType) })
                };

            UpdateState(OpCodes.Localloc, transitions.Wrap("LocalAllocate"), TypeOnStack.Get<NativeIntType>(), pop: 1);

            return this;
        }
    }
}
