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
                FailUnverifiable("InitializeBlock");
            }

            if(isVolatile)
            {
                UpdateState(OpCodes.Volatile, Wrap(StackTransition.None(), "InitializeBlock"));
            }

            if (unaligned.HasValue)
            {
                UpdateState(OpCodes.Unaligned, (byte)unaligned.Value, Wrap(StackTransition.None(), "InitializeBlock"));
            }

            var transition =
                new[] {
                    new StackTransition(new [] { typeof(int), typeof(int), typeof(NativeIntType) }, TypeHelpers.EmptyTypes),
                    new StackTransition(new [] { typeof(int), typeof(NativeIntType), typeof(NativeIntType) }, TypeHelpers.EmptyTypes),
                    new StackTransition(new [] { typeof(int), typeof(int), typeof(byte*) }, TypeHelpers.EmptyTypes),
                    new StackTransition(new [] { typeof(int), typeof(NativeIntType), typeof(byte*) }, TypeHelpers.EmptyTypes),
                    new StackTransition(new [] { typeof(int), typeof(int), typeof(byte).MakeByRefType() }, TypeHelpers.EmptyTypes),
                    new StackTransition(new [] { typeof(int), typeof(NativeIntType), typeof(byte).MakeByRefType() }, TypeHelpers.EmptyTypes)
                };

            UpdateState(OpCodes.Initblk, Wrap(transition, "InitializeBlock"));

            return this;
        }
    }
}
