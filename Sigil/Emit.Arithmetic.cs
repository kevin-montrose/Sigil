
using Sigil.Impl;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        private void VerifyAndDoArithmetic(string name, OpCode addOp, TypeOnStack val1, TypeOnStack val2, bool allowReference = false)
        {
            // See: http://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.add.aspx
            //   For legal arguments table
            IEnumerable<StackTransition> transitions;
            if (allowReference)
            {
                // TODO: figure out how to represent pointer/reference addition rules
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

            if (val1 == TypeOnStack.Get<int>())
            {
                if (val2 == TypeOnStack.Get<int>())
                {
                    UpdateState(addOp, transitions.Wrap(name), TypeOnStack.Get<int>(), pop: 2);

                    return;
                }

                if (val2 == TypeOnStack.Get<NativeIntType>())
                {
                    UpdateState(addOp, transitions.Wrap(name), TypeOnStack.Get<NativeIntType>(), pop: 2);

                    return;
                }

                if (allowReference)
                {
                    if (val2.IsReference || val2.IsPointer)
                    {
                        UpdateState(addOp, transitions.Wrap(name), val2, pop: 2);

                        return;
                    }
                }
                else
                {
                    throw new SigilVerificationException(name + " with an int32 expects an int32 or native int as a second value; found " + val2, IL.Instructions(LocalsByIndex), Stack, 0);
                }

                throw new SigilVerificationException(name + " with an int32 expects an int32, native int, reference, or pointer as a second value; found " + val2, IL.Instructions(LocalsByIndex), Stack, 0);
            }

            if (val1 == TypeOnStack.Get<long>())
            {
                if (val2 == TypeOnStack.Get<long>())
                {
                    UpdateState(addOp, transitions.Wrap(name), TypeOnStack.Get<long>(), pop: 2);

                    return;
                }

                throw new SigilVerificationException(name + " with to an int64 expects an in64 as second value; found " + val2, IL.Instructions(LocalsByIndex), Stack, 0);
            }

            if (val1 == TypeOnStack.Get<float>())
            {
                if (val2 == TypeOnStack.Get<float>())
                {
                    UpdateState(addOp, transitions.Wrap(name), TypeOnStack.Get<float>(), pop: 2);

                    return;
                }

                throw new SigilVerificationException(name + " with a float expects a float as second value; found " + val2, IL.Instructions(LocalsByIndex), Stack, 0);
            }

            if (val1 == TypeOnStack.Get<double>())
            {
                if (val2 == TypeOnStack.Get<double>())
                {
                    UpdateState(addOp, transitions.Wrap(name), TypeOnStack.Get<double>(), pop: 2);

                    return;
                }

                throw new SigilVerificationException(name + " with a double expects a double as second value; found " + val2, IL.Instructions(LocalsByIndex), Stack, 0);
            }

            if (allowReference)
            {
                if (val1 == TypeOnStack.Get<NativeIntType>())
                {
                    if (val2 == TypeOnStack.Get<int>())
                    {
                        UpdateState(addOp, transitions.Wrap(name), TypeOnStack.Get<NativeIntType>(), pop: 2);

                        return;
                    }

                    if (val2 == TypeOnStack.Get<NativeIntType>())
                    {
                        UpdateState(addOp, transitions.Wrap(name), TypeOnStack.Get<NativeIntType>(), pop: 2);

                        return;
                    }

                    if (val2.IsReference || val2.IsPointer)
                    {
                        UpdateState(addOp, transitions.Wrap(name), val2, pop: 2);

                        return;
                    }

                    throw new SigilVerificationException(name + " with a native int expects an int32, native int, reference, or pointer as a second value; found " + val2, IL.Instructions(LocalsByIndex), Stack, 0);
                }

                if (val1.IsReference || val1.IsPointer)
                {
                    if (val2 == TypeOnStack.Get<int>() || val2 == TypeOnStack.Get<NativeIntType>())
                    {
                        UpdateState(addOp, transitions.Wrap(name), val1, pop: 2);

                        return;
                    }

                    throw new SigilVerificationException(name + " with a reference or pointer expects an int32, or a native int as second value; found " + val2, IL.Instructions(LocalsByIndex), Stack, 0);
                }
            }
            else
            {
                throw new SigilVerificationException(name + " expects an int32, int64, native int, or float as a first value; found " + val1, IL.Instructions(LocalsByIndex), Stack, 1);
            }

            throw new SigilVerificationException(name + " expects an int32, int64, native int, float, reference, or pointer as first value; found " + val1, IL.Instructions(LocalsByIndex), Stack, 1);
        }

        /// <summary>
        /// Pops two arguments off the stack, adds them, and pushes the result.
        /// </summary>
        public Emit<DelegateType> Add()
        {
            var args = Stack.Top(2);

            if (args == null)
            {
                FailStackUnderflow(2);
            }

            var val2 = args[0];
            var val1 = args[1];

            VerifyAndDoArithmetic("Add", OpCodes.Add, val1, val2, allowReference: true);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, adds them, and pushes the result.
        /// 
        /// Throws an OverflowException if the result overflows the destination type.
        /// </summary>
        public Emit<DelegateType> AddOverflow()
        {
            var args = Stack.Top(2);

            if (args == null)
            {
                FailStackUnderflow(2);
            }

            var val2 = args[0];
            var val1 = args[1];

            VerifyAndDoArithmetic("AddOverflow", OpCodes.Add_Ovf, val1, val2, allowReference: true);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, adds them as if they were unsigned, and pushes the result.
        /// 
        /// Throws an OverflowException if the result overflows the destination type.
        /// </summary>
        public Emit<DelegateType> UnsignedAddOverflow()
        {
            var args = Stack.Top(2);

            if (args == null)
            {
                FailStackUnderflow(2);
            }

            var val2 = args[0];
            var val1 = args[1];

            VerifyAndDoArithmetic("UnsignedAddOverflow", OpCodes.Add_Ovf_Un, val1, val2, allowReference: true);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, divides the second by the first, and pushes the result.
        /// </summary>
        public Emit<DelegateType> Divide()
        {
            var args = Stack.Top(2);

            if (args == null)
            {
                FailStackUnderflow(2);
            }

            var val2 = args[0];
            var val1 = args[1];

            VerifyAndDoArithmetic("Divide", OpCodes.Div, val1, val2);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, divides the second by the first as if they were unsigned, and pushes the result.
        /// </summary>
        public Emit<DelegateType> UnsignedDivide()
        {
            var args = Stack.Top(2);

            if (args == null)
            {
                FailStackUnderflow(2);
            }

            var val2 = args[0];
            var val1 = args[1];

            VerifyAndDoArithmetic("UnsignedDivide", OpCodes.Div_Un, val1, val2);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, multiplies them, and pushes the result.
        /// </summary>
        public Emit<DelegateType> Multiply()
        {
            var args = Stack.Top(2);

            if (args == null)
            {
                FailStackUnderflow(2);
            }

            var val2 = args[0];
            var val1 = args[1];

            VerifyAndDoArithmetic("Multiply", OpCodes.Mul, val1, val2);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, multiplies them, and pushes the result.
        /// 
        /// Throws an OverflowException if the result overflows the destination type.
        /// </summary>
        public Emit<DelegateType> MultiplyOverflow()
        {
            var args = Stack.Top(2);

            if (args == null)
            {
                FailStackUnderflow(2);
            }

            var val2 = args[0];
            var val1 = args[1];

            VerifyAndDoArithmetic("MultiplyOverflow", OpCodes.Mul_Ovf, val1, val2);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, multiplies them as if they were unsigned, and pushes the result.
        /// 
        /// Throws an OverflowException if the result overflows the destination type.
        /// </summary>
        public Emit<DelegateType> UnsignedMultiplyOverflow()
        {
            var args = Stack.Top(2);

            if (args == null)
            {
                FailStackUnderflow(2);
            }

            var val2 = args[0];
            var val1 = args[1];

            VerifyAndDoArithmetic("UnsignedMultiplyOverflow", OpCodes.Mul_Ovf_Un, val1, val2);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, calculates the remainder of the second divided by the first, and pushes the result.
        /// </summary>
        public Emit<DelegateType> Remainder()
        {
            var args = Stack.Top(2);

            if (args == null)
            {
                FailStackUnderflow(2);
            }

            var val2 = args[0];
            var val1 = args[1];

            VerifyAndDoArithmetic("Remainder", OpCodes.Rem, val1, val2);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, calculates the remainder of the second divided by the first as if both were unsigned, and pushes the result.
        /// </summary>
        public Emit<DelegateType> UnsignedRemainder()
        {
            var args = Stack.Top(2);

            if (args == null)
            {
                FailStackUnderflow(2);
            }

            var val2 = args[0];
            var val1 = args[1];

            VerifyAndDoArithmetic("UnsignedRemainder", OpCodes.Rem_Un, val1, val2);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, subtracts the first from the second, and pushes the result.
        /// </summary>
        public Emit<DelegateType> Subtract()
        {
            var args = Stack.Top(2);

            if (args == null)
            {
                FailStackUnderflow(2);
            }

            var val2 = args[0];
            var val1 = args[1];

            VerifyAndDoArithmetic("Subtract", OpCodes.Sub, val1, val2);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, subtracts the first from the second, and pushes the result.
        /// 
        /// Throws an OverflowException if the result overflows the destination type.
        /// </summary>
        public Emit<DelegateType> SubtractOverflow()
        {
            var args = Stack.Top(2);

            if (args == null)
            {
                FailStackUnderflow(2);
            }

            var val2 = args[0];
            var val1 = args[1];

            VerifyAndDoArithmetic("SubtractOverflow", OpCodes.Sub_Ovf, val1, val2);

            return this;
        }

        /// <summary>
        /// Pops two arguments off the stack, subtracts the first from the second as if they were unsigned, and pushes the result.
        /// 
        /// Throws an OverflowException if the result overflows the destination type.
        /// </summary>
        public Emit<DelegateType> UnsignedSubtractOverflow()
        {
            var args = Stack.Top(2);

            if (args == null)
            {
                FailStackUnderflow(2);
            }

            var val2 = args[0];
            var val1 = args[1];

            VerifyAndDoArithmetic("UnsignedSubtractOverflow", OpCodes.Sub_Ovf_Un, val1, val2);

            return this;
        }

        /// <summary>
        /// Pops an argument off the stack, negates it, and pushes the result.
        /// </summary>
        public Emit<DelegateType> Negate()
        {
            var onStack = Stack.Top();

            if (onStack == null)
            {
                FailStackUnderflow(1);
            }

            var val = onStack[0];

            if (val != TypeOnStack.Get<long>() && val != TypeOnStack.Get<int>() && val != TypeOnStack.Get<float>() && val != TypeOnStack.Get<double>() && val != TypeOnStack.Get<NativeIntType>())
            {
                throw new SigilVerificationException("Negate expects an int, long, float, double, or native int; found " + val, IL.Instructions(LocalsByIndex), Stack, 0);
            }

            var transitions =
                new[]
                {
                    new StackTransition(new [] { typeof(int) }, new [] { typeof(int) }),
                    new StackTransition(new [] { typeof(NativeIntType) }, new [] { typeof(NativeIntType) }),
                    new StackTransition(new [] { typeof(long) }, new [] { typeof(long) }),
                    new StackTransition(new [] { typeof(float) }, new [] { typeof(float) }),
                    new StackTransition(new [] { typeof(double) }, new [] { typeof(double) })
                };

            UpdateState(OpCodes.Neg, transitions.Wrap("Negate"), val, pop: 1);

            return this;
        }
    }
}
