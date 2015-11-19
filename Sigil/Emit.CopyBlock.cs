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
                FailUnverifiable("CopyBlock");
            }

            if (isVolatile)
            {
                UpdateState(OpCodes.Volatile, Wrap(StackTransition.None(),"CopyBlock"));
            }

            if (unaligned.HasValue)
            {
                UpdateState(OpCodes.Unaligned, (byte)unaligned.Value, Wrap(StackTransition.None(), "CopyBlock"));
            }

            var transition =
                new[]
                {
                    new StackTransition(new[] { typeof(int), typeof(NativeIntType), typeof(NativeIntType) }, TypeHelpers.EmptyTypes),
                    new StackTransition(new[] { typeof(int), typeof(byte*), typeof(NativeIntType) }, TypeHelpers.EmptyTypes),
                    new StackTransition(new[] { typeof(int), typeof(byte).MakeByRefType(), typeof(NativeIntType) }, TypeHelpers.EmptyTypes),
                    new StackTransition(new[] { typeof(int), typeof(NativeIntType), typeof(byte*) }, TypeHelpers.EmptyTypes),
                    new StackTransition(new[] { typeof(int), typeof(byte*), typeof(byte*) }, TypeHelpers.EmptyTypes),
                    new StackTransition(new[] { typeof(int), typeof(byte).MakeByRefType(), typeof(byte*) }, TypeHelpers.EmptyTypes),
                    new StackTransition(new[] { typeof(int), typeof(NativeIntType), typeof(byte).MakeByRefType() }, TypeHelpers.EmptyTypes),
                    new StackTransition(new[] { typeof(int), typeof(byte*), typeof(byte).MakeByRefType() }, TypeHelpers.EmptyTypes),
                    new StackTransition(new[] { typeof(int), typeof(byte).MakeByRefType(), typeof(byte).MakeByRefType() }, TypeHelpers.EmptyTypes)
                };

            UpdateState(OpCodes.Cpblk, Wrap(transition, "CopyBlock"));

            return this;
        }
    }
}
