using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Expects a pointer, an initialization value, and a count on the stack.  Pops all three.
        /// 
        /// Writes the initialization value to count bytes at the passed pointer.
        /// </summary>
        public Emit<DelegateType> InitializeBlock(bool isVolatile = false, int? unaligned = null)
        {
            if (unaligned.HasValue && (unaligned != 1 && unaligned != 2 && unaligned != 4))
            {
                throw new ArgumentException("unaligned must be null, 1, 2, or 4");
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

            var start = onStack[2];
            var init = onStack[1];
            var count = onStack[0];

            if (!start.IsPointer && !start.IsReference && start != TypeOnStack.Get<NativeIntType>())
            {
                throw new SigilVerificationException("InitializeBlock expects the start value to be a pointer, reference, or native int; found " + start, IL.Instructions(LocalsByIndex), Stack, 2);
            }

            if (init != TypeOnStack.Get<int>() && init != TypeOnStack.Get<NativeIntType>())
            {
                throw new SigilVerificationException("InitBlock expects the initial value to be an int or native int; found " + init, IL.Instructions(LocalsByIndex), Stack, 1);
            }

            if (count != TypeOnStack.Get<int>())
            {
                throw new SigilVerificationException("InitBlock expects the count to be an int; found " + count, IL.Instructions(LocalsByIndex), Stack, 0);
            }

            if(isVolatile)
            {
                UpdateState(OpCodes.Volatile, StackTransition.None());
            }

            if (unaligned.HasValue)
            {
                UpdateState(OpCodes.Unaligned, unaligned.Value, StackTransition.None());
            }

            var transition =
                new[] {
                    new StackTransition(new [] { typeof(int), typeof(int), typeof(NativeIntType) }, Type.EmptyTypes),
                    new StackTransition(new [] { typeof(int), typeof(NativeIntType), typeof(NativeIntType) }, Type.EmptyTypes),
                    new StackTransition(new [] { typeof(int), typeof(int), typeof(byte*) }, Type.EmptyTypes),
                    new StackTransition(new [] { typeof(int), typeof(NativeIntType), typeof(byte*) }, Type.EmptyTypes),
                    new StackTransition(new [] { typeof(int), typeof(int), typeof(byte).MakeByRefType() }, Type.EmptyTypes),
                    new StackTransition(new [] { typeof(int), typeof(NativeIntType), typeof(byte).MakeByRefType() }, Type.EmptyTypes)
                };

            UpdateState(OpCodes.Initblk, transition, pop: 3);

            return this;
        }
    }
}
