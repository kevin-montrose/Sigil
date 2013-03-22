
using Sigil.Impl;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        private void VerifyAndBinaryBitwise(string name, OpCode op)
        {
            var transitions =
                new[] 
                {
                    new StackTransition(new [] { typeof(int), typeof(int) }, new [] { typeof(int) }),
                    new StackTransition(new [] { typeof(long), typeof(long) }, new [] { typeof(long) }),
                    new StackTransition(new [] { typeof(NativeIntType), typeof(NativeIntType) }, new [] { typeof(NativeIntType) }),
                };

            UpdateState(op, Wrap(transitions, name));
        }

        /// <summary>
        /// Pops two arguments off the stack, performs a bitwise and, and pushes the result.
        /// </summary>
        public Emit<DelegateType> And()
        {
            VerifyAndBinaryBitwise("And", OpCodes.And);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, performs a bitwise or, and pushes the result.
        /// </summary>
        public Emit<DelegateType> Or()
        {
            VerifyAndBinaryBitwise("Or", OpCodes.Or);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, performs a bitwise xor, and pushes the result.
        /// </summary>
        public Emit<DelegateType> Xor()
        {
            VerifyAndBinaryBitwise("Xor", OpCodes.Xor);

            return this;
        }

        /// <summary>
        /// Pops one argument off the stack, performs a bitwise inversion, and pushes the result.
        /// </summary>
        public Emit<DelegateType> Not()
        {
            var transitions =
                new[]
                {
                    new StackTransition(new [] { typeof(int) }, new [] { typeof(int) }),
                    new StackTransition(new [] { typeof(long) }, new [] { typeof(long) }),
                    new StackTransition(new [] { typeof(NativeIntType) }, new [] { typeof(NativeIntType) }),
                };

            UpdateState(OpCodes.Not, Wrap(transitions, "Not"));

            return this;
        }

        private void VerifyAndShift(string name, OpCode op)
        {
            var transitions =
                new[] 
                {
                    new StackTransition(new [] { typeof(NativeIntType), typeof(NativeIntType) }, new [] { typeof(NativeIntType) }),
                    new StackTransition(new [] { typeof(int), typeof(NativeIntType) }, new [] { typeof(NativeIntType) }),
                    new StackTransition(new [] { typeof(NativeIntType), typeof(int) }, new [] { typeof(int) }),
                    new StackTransition(new [] { typeof(int), typeof(int) }, new [] { typeof(int) }),
                    new StackTransition(new [] { typeof(NativeIntType), typeof(long) }, new [] { typeof(long) }),
                    new StackTransition(new [] { typeof(int), typeof(long) }, new [] { typeof(long) })
                };

            UpdateState(op, Wrap(transitions, name));
        }

        /// <summary>
        /// Pops two arguments off the stack, shifts the second value left by the first value.
        /// </summary>
        public Emit<DelegateType> ShiftLeft()
        {
            VerifyAndShift("ShiftLeft", OpCodes.Shl);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, shifts the second value right by the first value.
        /// 
        /// Sign extends from the left.
        /// </summary>
        public Emit<DelegateType> ShiftRight()
        {
            VerifyAndShift("ShiftRight", OpCodes.Shr);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, shifts the second value right by the first value.
        /// 
        /// Acts as if the value were unsigned, zeros always coming in from the left.
        /// </summary>
        public Emit<DelegateType> UnsignedShiftRight()
        {
            VerifyAndShift("UnsignedShiftRight", OpCodes.Shr_Un);

            return this;
        }
    }
}
