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
        private void VerifyAndBinaryBitwise(string name, OpCode op)
        {
            var onStack = Stack.Top(2);

            if (onStack == null)
            {
                throw new SigilException(name+" expects two values on the stack", Stack);
            }

            var val2 = onStack[0];
            var val1 = onStack[1];

            if (val1 != TypeOnStack.Get<int>() && val1 != TypeOnStack.Get<long>() && val1 != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilException(name+" expects integral types, but the first value type was " + val1, Stack);
            }

            if (val2 != TypeOnStack.Get<int>() && val2 != TypeOnStack.Get<long>() && val2 != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilException(name+" expects integral types, but the second value type was " + val2, Stack);
            }

            if (val1 != val2)
            {
                throw new SigilException(name+" expects the same types for values, found " + val1 + " and " + val2, Stack);
            }

            UpdateState(op, val1, pop: 2);
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
            var onStack = Stack.Top();

            if (onStack == null)
            {
                throw new SigilException("Not expects a value to be on the stack, but it was empty", Stack);
            }

            var val = onStack[0];

            if (val != TypeOnStack.Get<int>() && val != TypeOnStack.Get<long>() && val != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilException("Not expects integral types, but found " + val, Stack);
            }

            UpdateState(OpCodes.Not, val, pop: 1);

            return this;
        }

        private void VerifyAndShift(string name, OpCode op)
        {
            var onStack = Stack.Top(2);

            if (onStack == null)
            {
                throw new SigilException(name + " expects two values on the stack", Stack);
            }

            var shift = onStack[0];
            var value = onStack[1];

            if (value != TypeOnStack.Get<int>() && value != TypeOnStack.Get<long>() && value != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilException(name + " expects the value to be shifted to be an int, long, or native int; found " + value, Stack);
            }

            if (shift != TypeOnStack.Get<int>() && shift != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilException(name + " expects the shift to be an int or native int; found " + shift, Stack);
            }

            UpdateState(op, value, pop: 2);
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
