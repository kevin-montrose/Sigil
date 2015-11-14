
using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Expects a reference to an array and an index on the stack.
        /// 
        /// Pops both, and pushes the element in the array at the index onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadElement<ElementType>()
        {
            return LoadElement(typeof(ElementType));
        }

        /// <summary>
        /// Expects a reference to an array and an index on the stack.
        /// 
        /// Pops both, and pushes the element in the array at the index onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadElement(Type elementType)
        {
            if (elementType == null)
            {
                throw new ArgumentNullException("elementType");
            }

            OpCode? instr = null;

            IEnumerable<StackTransition> transitions = null;

            var arrType = elementType.MakeArrayType();

            if (elementType.IsPointer)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType), arrType }, new [] { elementType }),
                        new StackTransition(new [] { typeof(int), arrType }, new [] { elementType }),
                    };

                instr = OpCodes.Ldelem_I;
            }

            if (!TypeHelpers.IsValueType(elementType) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType), arrType }, new [] { elementType }),
                        new StackTransition(new [] { typeof(int), arrType }, new [] { elementType }),
                    };

                instr = OpCodes.Ldelem_Ref;
            }

            if (elementType == typeof(sbyte) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType), arrType }, new [] { typeof(int) }),
                        new StackTransition(new [] { typeof(int), arrType }, new [] { typeof(int) }),
                    };

                instr = OpCodes.Ldelem_I1;
            }

            if (elementType == typeof(byte) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType), arrType }, new [] { typeof(int) }),
                        new StackTransition(new [] { typeof(int), arrType }, new [] { typeof(int) }),
                    };

                instr = OpCodes.Ldelem_U1;
            }

            if (elementType == typeof(short) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType), arrType }, new [] { typeof(int) }),
                        new StackTransition(new [] { typeof(int), arrType }, new [] { typeof(int) }),
                    };

                instr = OpCodes.Ldelem_I2;
            }

            if (elementType == typeof(ushort) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType), arrType }, new [] { typeof(int) }),
                        new StackTransition(new [] { typeof(int), arrType }, new [] { typeof(int) }),
                    };

                instr = OpCodes.Ldelem_U2;
            }

            if (elementType == typeof(int) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType), arrType }, new [] { typeof(int) }),
                        new StackTransition(new [] { typeof(int), arrType }, new [] { typeof(int) }),
                    };

                instr = OpCodes.Ldelem_I4;
            }

            if (elementType == typeof(uint) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType), arrType }, new [] { typeof(int) }),
                        new StackTransition(new [] { typeof(int), arrType }, new [] { typeof(int) }),
                    };

                instr = OpCodes.Ldelem_U4;
            }

            if ((elementType == typeof(long) || elementType == typeof(ulong)) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType), arrType }, new [] { typeof(long) }),
                        new StackTransition(new [] { typeof(int), arrType }, new [] { typeof(long) }),
                    };

                instr = OpCodes.Ldelem_I8;
            }

            if (elementType == typeof(float) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType), arrType }, new [] { typeof(float) }),
                        new StackTransition(new [] { typeof(int), arrType }, new [] { typeof(float) }),
                    };

                instr = OpCodes.Ldelem_R4;
            }

            if (elementType == typeof(double) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType), arrType }, new [] { typeof(double) }),
                        new StackTransition(new [] { typeof(int), arrType }, new [] { typeof(double) }),
                    };

                instr = OpCodes.Ldelem_R8;
            }

            if (!instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType), arrType }, new [] { elementType }),
                        new StackTransition(new [] { typeof(int), arrType }, new [] { elementType }),
                    };

                UpdateState(OpCodes.Ldelem, elementType, Wrap(transitions, "LoadElement"));
                return this;
            }

            UpdateState(instr.Value, Wrap(transitions, "LoadElement"));

            return this;
        }
    }
}
