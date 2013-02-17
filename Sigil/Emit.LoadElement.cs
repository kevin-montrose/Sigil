
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
        public Emit<DelegateType> LoadElement(Type arrayType = null)
        {
            // TODO: We can't always infer the array type, handle the case where we can't

            var top = Stack.Top(2);

            if (top == null)
            {
                FailStackUnderflow(2);
            }

            var index = top[0];
            var array = top[1];

            if (index != TypeOnStack.Get<int>() && index != TypeOnStack.Get<NativeIntType>())
            {
                throw new SigilVerificationException("LoadElement expects an int or native int on the top of the stack, found " + index, IL.Instructions(LocalsByIndex), Stack, 0);
            }

            if (array.IsReference || array.IsPointer || !array.Type.IsArray)
            {
                throw new SigilVerificationException("LoadElement expects an array as the second element on the stack, found " + array, IL.Instructions(LocalsByIndex), Stack, 1);
            }

            if (array.Type.GetArrayRank() != 1)
            {
                throw new SigilVerificationException("LoadElement expects a 1-dimensional array, found " + array, IL.Instructions(LocalsByIndex), Stack, 1);
            }

            OpCode? instr = null;
            var elemType = array.Type.GetElementType();

            IEnumerable<StackTransition> transitions = null;

            if (elemType.IsPointer)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType), array.Type }, new [] { elemType }),
                        new StackTransition(new [] { typeof(int), array.Type }, new [] { elemType }),
                    };

                instr = OpCodes.Ldelem_I;
            }

            if (!elemType.IsValueType && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType), array.Type }, new [] { elemType }),
                        new StackTransition(new [] { typeof(int), array.Type }, new [] { elemType }),
                    };

                instr = OpCodes.Ldelem_Ref;
            }

            if (elemType == typeof(sbyte) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType), array.Type }, new [] { typeof(int) }),
                        new StackTransition(new [] { typeof(int), array.Type }, new [] { typeof(int) }),
                    };

                instr = OpCodes.Ldelem_I1;
            }

            if (elemType == typeof(byte) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType), array.Type }, new [] { typeof(int) }),
                        new StackTransition(new [] { typeof(int), array.Type }, new [] { typeof(int) }),
                    };

                instr = OpCodes.Ldelem_U1;
            }

            if (elemType == typeof(short) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType), array.Type }, new [] { typeof(int) }),
                        new StackTransition(new [] { typeof(int), array.Type }, new [] { typeof(int) }),
                    };

                instr = OpCodes.Ldelem_I2;
            }

            if (elemType == typeof(ushort) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType), array.Type }, new [] { typeof(int) }),
                        new StackTransition(new [] { typeof(int), array.Type }, new [] { typeof(int) }),
                    };

                instr = OpCodes.Ldelem_U2;
            }

            if (elemType == typeof(int) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType), array.Type }, new [] { typeof(int) }),
                        new StackTransition(new [] { typeof(int), array.Type }, new [] { typeof(int) }),
                    };

                instr = OpCodes.Ldelem_I4;
            }

            if (elemType == typeof(uint) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType), array.Type }, new [] { typeof(int) }),
                        new StackTransition(new [] { typeof(int), array.Type }, new [] { typeof(int) }),
                    };

                instr = OpCodes.Ldelem_U4;
            }

            if ((elemType == typeof(long) || elemType == typeof(ulong)) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType), array.Type }, new [] { typeof(long) }),
                        new StackTransition(new [] { typeof(int), array.Type }, new [] { typeof(long) }),
                    };

                instr = OpCodes.Ldelem_I8;
            }

            if (elemType == typeof(float) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType), array.Type }, new [] { typeof(float) }),
                        new StackTransition(new [] { typeof(int), array.Type }, new [] { typeof(float) }),
                    };

                instr = OpCodes.Ldelem_R4;
            }

            if (elemType == typeof(double) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType), array.Type }, new [] { typeof(double) }),
                        new StackTransition(new [] { typeof(int), array.Type }, new [] { typeof(double) }),
                    };

                instr = OpCodes.Ldelem_R8;
            }

            if (!instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType), array.Type }, new [] { elemType }),
                        new StackTransition(new [] { typeof(int), array.Type }, new [] { elemType }),
                    };

                UpdateState(OpCodes.Ldelem, elemType, transitions, TypeOnStack.Get(elemType), pop: 2);
                return this;
            }

            UpdateState(instr.Value, transitions, TypeOnStack.Get(elemType), pop: 2);

            return this;
        }
    }
}
