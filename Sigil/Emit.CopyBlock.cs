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

            if (isVolatile)
            {
                UpdateState(OpCodes.Volatile, StackTransition.None().Wrap("CopyBlock"));
            }

            if (unaligned.HasValue)
            {
                UpdateState(OpCodes.Unaligned, unaligned.Value, StackTransition.None().Wrap("CopyBlock"));
            }

            var transition =
                new[]
                {
                    new StackTransition(new[] { typeof(int), typeof(NativeIntType), typeof(NativeIntType) }, Type.EmptyTypes),
                    new StackTransition(new[] { typeof(int), typeof(byte*), typeof(NativeIntType) }, Type.EmptyTypes),
                    new StackTransition(new[] { typeof(int), typeof(byte).MakeByRefType(), typeof(NativeIntType) }, Type.EmptyTypes),
                    new StackTransition(new[] { typeof(int), typeof(NativeIntType), typeof(byte*) }, Type.EmptyTypes),
                    new StackTransition(new[] { typeof(int), typeof(byte*), typeof(byte*) }, Type.EmptyTypes),
                    new StackTransition(new[] { typeof(int), typeof(byte).MakeByRefType(), typeof(byte*) }, Type.EmptyTypes),
                    new StackTransition(new[] { typeof(int), typeof(NativeIntType), typeof(byte).MakeByRefType() }, Type.EmptyTypes),
                    new StackTransition(new[] { typeof(int), typeof(byte*), typeof(byte).MakeByRefType() }, Type.EmptyTypes),
                    new StackTransition(new[] { typeof(int), typeof(byte).MakeByRefType(), typeof(byte).MakeByRefType() }, Type.EmptyTypes)
                };

            UpdateState(OpCodes.Cpblk, transition.Wrap("CopyBlock"), pop: 3);

            return this;
        }
    }
}
