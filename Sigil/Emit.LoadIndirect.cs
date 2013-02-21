using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pops a pointer from the stack and pushes the value (of the given type) at that address onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadIndirect<Type>(bool isVolatile = false, int? unaligned = null)
        {
            return LoadIndirect(typeof(Type), isVolatile, unaligned);
        }

        /// <summary>
        /// Pops a pointer from the stack and pushes the value (of the given type) at that address onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadIndirect(Type type, bool isVolatile = false, int? unaligned = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (unaligned.HasValue && (unaligned != 1 && unaligned != 2 && unaligned != 4))
            {
                throw new ArgumentException("unaligned must be null, 1, 2, or 4");
            }

            /*var onStack = Stack.Top();

            if (onStack == null)
            {
                FailStackUnderflow(1);
            }

            var ptr = onStack[0];

            if (!ptr.IsPointer && !ptr.IsReference && ptr != TypeOnStack.Get<NativeIntType>())
            {
                throw new SigilVerificationException("LoadIndirect expects a pointer, reference, or native int on the stack, found " + ptr, IL.Instructions(LocalsByIndex), Stack, 0);
            }

            if (ptr.IsPointer || ptr.IsReference)
            {
                if (!type.IsAssignableFrom(TypeOnStack.Get(ptr.Type.GetElementType())))
                {
                    throw new SigilVerificationException("LoadIndirect expected a pointer or reference to type " + type + ", but found " + ptr, IL.Instructions(LocalsByIndex), Stack, 0);
                }
            }*/

            OpCode? instr = null;
            IEnumerable<StackTransition> transitions = null;

            if (type.IsPointer)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType) }, new [] { type }),
                        new StackTransition(new [] { type.MakePointerType() }, new[] { type }),
                        new StackTransition(new [] { type.MakeByRefType() }, new[] { type })
                    };


                instr = OpCodes.Ldind_I;
            }

            if (!type.IsValueType && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType) }, new [] { type }),
                        new StackTransition(new [] { type.MakePointerType() }, new[] { type }),
                        new StackTransition(new [] { type.MakeByRefType() }, new[] { type })
                    };

                instr = OpCodes.Ldind_Ref;
            }

            if (type == typeof(sbyte) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType) }, new [] { typeof(int) }),
                        new StackTransition(new [] { typeof(sbyte*) }, new[] { typeof(int) }),
                        new StackTransition(new [] { typeof(sbyte).MakeByRefType() }, new[] { typeof(int) })
                    };

                instr = OpCodes.Ldind_I1;
            }

            if (type == typeof(byte) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType) }, new [] { typeof(int) }),
                        new StackTransition(new [] { typeof(byte*) }, new[] { typeof(int) }),
                        new StackTransition(new [] { typeof(byte).MakeByRefType() }, new[] { typeof(int) })
                    };

                instr = OpCodes.Ldind_U1;
            }

            if (type == typeof(short) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType) }, new [] { typeof(int) }),
                        new StackTransition(new [] { typeof(short*) }, new[] { typeof(int) }),
                        new StackTransition(new [] { typeof(short).MakeByRefType() }, new[] { typeof(int) })
                    };

                instr = OpCodes.Ldind_I2;
            }

            if (type == typeof(ushort) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType) }, new [] { typeof(int) }),
                        new StackTransition(new [] { typeof(ushort*) }, new[] { typeof(int) }),
                        new StackTransition(new [] { typeof(ushort).MakeByRefType() }, new[] { typeof(int) })
                    };

                instr = OpCodes.Ldind_U2;
            }

            if (type == typeof(int) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType) }, new [] { typeof(int) }),
                        new StackTransition(new [] { typeof(int*) }, new[] { typeof(int) }),
                        new StackTransition(new [] { typeof(int).MakeByRefType() }, new[] { typeof(int) })
                    };

                instr = OpCodes.Ldind_I4;
            }

            if (type == typeof(uint) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType) }, new [] { typeof(int) }),
                        new StackTransition(new [] { typeof(uint*) }, new[] { typeof(int) }),
                        new StackTransition(new [] { typeof(uint).MakeByRefType() }, new[] { typeof(int) })
                    };

                instr = OpCodes.Ldind_U4;
            }

            if ((type == typeof(long) || type == typeof(ulong)) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType) }, new [] { typeof(long) }),
                        new StackTransition(new [] { typeof(long*) }, new[] { typeof(long) }),
                        new StackTransition(new [] { typeof(long).MakeByRefType() }, new[] { typeof(long) }),
                        new StackTransition(new [] { typeof(ulong*) }, new[] { typeof(long) }),
                        new StackTransition(new [] { typeof(ulong).MakeByRefType() }, new[] { typeof(long) })
                    };

                instr = OpCodes.Ldind_I8;
            }

            if (type == typeof(float) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType) }, new [] { typeof(float) }),
                        new StackTransition(new [] { typeof(float*) }, new[] { typeof(float) }),
                        new StackTransition(new [] { typeof(float).MakeByRefType() }, new[] { typeof(float) })
                    };

                instr = OpCodes.Ldind_R4;
            }

            if (type == typeof(double) && !instr.HasValue)
            {
                transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(NativeIntType) }, new [] { typeof(double) }),
                        new StackTransition(new [] { typeof(double*) }, new[] { typeof(double) }),
                        new StackTransition(new [] { typeof(double).MakeByRefType() }, new[] { typeof(double) })
                    };

                instr = OpCodes.Ldind_R8;
            }

            if (!instr.HasValue)
            {
                throw new InvalidOperationException("LoadIndirect cannot be used with " + type + ", LoadObject may be more appropriate");
            }

            if (isVolatile)
            {
                UpdateState(OpCodes.Volatile, StackTransition.None().Wrap("LoadIndirect"));
            }

            if (unaligned.HasValue)
            {
                UpdateState(OpCodes.Unaligned, unaligned.Value, StackTransition.None().Wrap("LoadIndirect"));
            }

            UpdateState(instr.Value, transitions.Wrap("LoadIndirect"));

            return this;
        }
    }
}
