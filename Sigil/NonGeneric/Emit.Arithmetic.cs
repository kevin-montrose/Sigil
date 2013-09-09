

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Pops two arguments off the stack, adds them, and pushes the result.
        /// </summary>
        public Emit Add()
        {
            InnerEmit.Add();
            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, adds them, and pushes the result.
        /// 
        /// Throws an OverflowException if the result overflows the destination type.
        /// </summary>
        public Emit AddOverflow()
        {
            InnerEmit.AddOverflow();
            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, adds them as if they were unsigned, and pushes the result.
        /// 
        /// Throws an OverflowException if the result overflows the destination type.
        /// </summary>
        public Emit UnsignedAddOverflow()
        {
            InnerEmit.UnsignedAddOverflow();
            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, divides the second by the first, and pushes the result.
        /// </summary>
        public Emit Divide()
        {
            InnerEmit.Divide();
            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, divides the second by the first as if they were unsigned, and pushes the result.
        /// </summary>
        public Emit UnsignedDivide()
        {
            InnerEmit.UnsignedDivide();
            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, multiplies them, and pushes the result.
        /// </summary>
        public Emit Multiply()
        {
            InnerEmit.Multiply();
            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, multiplies them, and pushes the result.
        /// 
        /// Throws an OverflowException if the result overflows the destination type.
        /// </summary>
        public Emit MultiplyOverflow()
        {
            InnerEmit.MultiplyOverflow();
            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, multiplies them as if they were unsigned, and pushes the result.
        /// 
        /// Throws an OverflowException if the result overflows the destination type.
        /// </summary>
        public Emit UnsignedMultiplyOverflow()
        {
            InnerEmit.UnsignedMultiplyOverflow();
            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, calculates the remainder of the second divided by the first, and pushes the result.
        /// </summary>
        public Emit Remainder()
        {
            InnerEmit.Remainder();
            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, calculates the remainder of the second divided by the first as if both were unsigned, and pushes the result.
        /// </summary>
        public Emit UnsignedRemainder()
        {
            InnerEmit.UnsignedRemainder();
            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, subtracts the first from the second, and pushes the result.
        /// </summary>
        public Emit Subtract()
        {
            InnerEmit.Subtract();
            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, subtracts the first from the second, and pushes the result.
        /// 
        /// Throws an OverflowException if the result overflows the destination type.
        /// </summary>
        public Emit SubtractOverflow()
        {
            InnerEmit.SubtractOverflow();
            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, subtracts the first from the second as if they were unsigned, and pushes the result.
        /// 
        /// Throws an OverflowException if the result overflows the destination type.
        /// </summary>
        public Emit UnsignedSubtractOverflow()
        {
            InnerEmit.UnsignedSubtractOverflow();
            return this;
        }

        /// <summary>
        /// Pops an argument off the stack, negates it, and pushes the result.
        /// </summary>
        public Emit Negate()
        {
            InnerEmit.Negate();
            return this;
        }
    }
}
