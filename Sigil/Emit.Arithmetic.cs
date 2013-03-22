
using Sigil.Impl;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        private void VerifyAndDoArithmetic(string name, OpCode addOp, bool allowReference = false)
        {
            // See: http://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.add.aspx
            //   For legal arguments table
            IEnumerable<StackTransition> transitions;
            if (allowReference)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(int), typeof(int) }, new [] { typeof(int) }),
                        new StackTransition(new [] { typeof(int), typeof(NativeIntType) }, new [] { typeof(NativeIntType) }),
                        new StackTransition(new [] { typeof(long), typeof(long) }, new [] { typeof(long) }),
                        new StackTransition(new [] { typeof(NativeIntType), typeof(int) }, new [] { typeof(NativeIntType) }),
                        new StackTransition(new [] { typeof(NativeIntType), typeof(NativeIntType) }, new [] { typeof(NativeIntType) }),
                        new StackTransition(new [] { typeof(float), typeof(float) }, new [] { typeof(float) }),
                        new StackTransition(new [] { typeof(double), typeof(double) }, new [] { typeof(double) }),

                        new StackTransition(new [] { typeof(AnyPointerType), typeof(int) }, new [] { typeof(SamePointerType) }),
                        new StackTransition(new [] { typeof(AnyPointerType), typeof(NativeIntType) }, new [] { typeof(SamePointerType) }),
                        new StackTransition(new [] { typeof(AnyByRefType), typeof(int) }, new [] { typeof(SameByRefType) }),
                        new StackTransition(new [] { typeof(AnyByRefType), typeof(NativeIntType) }, new [] { typeof(SameByRefType) })
                    };
            }
            else
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(int), typeof(int) }, new [] { typeof(int) }),
                        new StackTransition(new [] { typeof(int), typeof(NativeIntType) }, new [] { typeof(NativeIntType) }),
                        new StackTransition(new [] { typeof(long), typeof(long) }, new [] { typeof(long) }),
                        new StackTransition(new [] { typeof(NativeIntType), typeof(int) }, new [] { typeof(NativeIntType) }),
                        new StackTransition(new [] { typeof(NativeIntType), typeof(NativeIntType) }, new [] { typeof(NativeIntType) }),
                        new StackTransition(new [] { typeof(float), typeof(float) }, new [] { typeof(float) }),
                        new StackTransition(new [] { typeof(double), typeof(double) }, new [] { typeof(double) })
                    };
            }

            UpdateState(addOp, Wrap(transitions, name));
        }

        /// <summary>
        /// Pops two arguments off the stack, adds them, and pushes the result.
        /// </summary>
        public Emit<DelegateType> Add()
        {
            VerifyAndDoArithmetic("Add", OpCodes.Add, allowReference: true);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, adds them, and pushes the result.
        /// 
        /// Throws an OverflowException if the result overflows the destination type.
        /// </summary>
        public Emit<DelegateType> AddOverflow()
        {
            VerifyAndDoArithmetic("AddOverflow", OpCodes.Add_Ovf, allowReference: true);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, adds them as if they were unsigned, and pushes the result.
        /// 
        /// Throws an OverflowException if the result overflows the destination type.
        /// </summary>
        public Emit<DelegateType> UnsignedAddOverflow()
        {
            VerifyAndDoArithmetic("UnsignedAddOverflow", OpCodes.Add_Ovf_Un, allowReference: true);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, divides the second by the first, and pushes the result.
        /// </summary>
        public Emit<DelegateType> Divide()
        {
            VerifyAndDoArithmetic("Divide", OpCodes.Div);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, divides the second by the first as if they were unsigned, and pushes the result.
        /// </summary>
        public Emit<DelegateType> UnsignedDivide()
        {
            VerifyAndDoArithmetic("UnsignedDivide", OpCodes.Div_Un);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, multiplies them, and pushes the result.
        /// </summary>
        public Emit<DelegateType> Multiply()
        {
            VerifyAndDoArithmetic("Multiply", OpCodes.Mul);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, multiplies them, and pushes the result.
        /// 
        /// Throws an OverflowException if the result overflows the destination type.
        /// </summary>
        public Emit<DelegateType> MultiplyOverflow()
        {
            VerifyAndDoArithmetic("MultiplyOverflow", OpCodes.Mul_Ovf);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, multiplies them as if they were unsigned, and pushes the result.
        /// 
        /// Throws an OverflowException if the result overflows the destination type.
        /// </summary>
        public Emit<DelegateType> UnsignedMultiplyOverflow()
        {
            VerifyAndDoArithmetic("UnsignedMultiplyOverflow", OpCodes.Mul_Ovf_Un);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, calculates the remainder of the second divided by the first, and pushes the result.
        /// </summary>
        public Emit<DelegateType> Remainder()
        {
            VerifyAndDoArithmetic("Remainder", OpCodes.Rem);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, calculates the remainder of the second divided by the first as if both were unsigned, and pushes the result.
        /// </summary>
        public Emit<DelegateType> UnsignedRemainder()
        {
            VerifyAndDoArithmetic("UnsignedRemainder", OpCodes.Rem_Un);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, subtracts the first from the second, and pushes the result.
        /// </summary>
        public Emit<DelegateType> Subtract()
        {
            VerifyAndDoArithmetic("Subtract", OpCodes.Sub);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, subtracts the first from the second, and pushes the result.
        /// 
        /// Throws an OverflowException if the result overflows the destination type.
        /// </summary>
        public Emit<DelegateType> SubtractOverflow()
        {
            VerifyAndDoArithmetic("SubtractOverflow", OpCodes.Sub_Ovf);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, subtracts the first from the second as if they were unsigned, and pushes the result.
        /// 
        /// Throws an OverflowException if the result overflows the destination type.
        /// </summary>
        public Emit<DelegateType> UnsignedSubtractOverflow()
        {
            VerifyAndDoArithmetic("UnsignedSubtractOverflow", OpCodes.Sub_Ovf_Un);

            return this;
        }

        /// <summary>
        /// Pops an argument off the stack, negates it, and pushes the result.
        /// </summary>
        public Emit<DelegateType> Negate()
        {
            var transitions =
                new[]
                {
                    new StackTransition(new [] { typeof(int) }, new [] { typeof(int) }),
                    new StackTransition(new [] { typeof(NativeIntType) }, new [] { typeof(NativeIntType) }),
                    new StackTransition(new [] { typeof(long) }, new [] { typeof(long) }),
                    new StackTransition(new [] { typeof(float) }, new [] { typeof(float) }),
                    new StackTransition(new [] { typeof(double) }, new [] { typeof(double) })
                };

            UpdateState(OpCodes.Neg, Wrap(transitions, "Negate"));

            return this;
        }
    }
}
