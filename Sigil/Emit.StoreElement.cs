using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pops a value, an index, and a reference to an array off the stack.  Places the given value into the given array at the given index.
        /// </summary>
        public Emit<DelegateType> StoreElement(Type arrayType = null)
        {
            // TODO: Like some other places, we may not be able to infer this; deal with that case

            var onStack = Stack.Top(3);

            if (onStack == null)
            {
                FailStackUnderflow(3);
            }

            var value = onStack[0];
            var index = onStack[1];
            var arr = onStack[2];

            if (arr.IsPointer || arr.IsReference || !arr.Type.IsArray || arr.Type.GetArrayRank() != 1)
            {
                throw new SigilVerificationException("StoreElement expects a rank one array, found " + arr, IL.Instructions(LocalsByIndex), Stack, 2);
            }

            if (index != TypeOnStack.Get<int>() && index != TypeOnStack.Get<NativeIntType>())
            {
                throw new SigilVerificationException("StoreElement expects an index of type int or native int, found " + index, IL.Instructions(LocalsByIndex), Stack, 1);
            }

            var elemType = arr.Type.GetElementType();

            if (!elemType.IsAssignableFrom(value))
            {
                throw new SigilVerificationException("StoreElement expects a value assignable to " + elemType + ", found " + value, IL.Instructions(LocalsByIndex), Stack, 2);
            }

            IEnumerable<StackTransition> transitions = null;

            OpCode? instr = null;

            if (elemType.IsPointer)
            {
                transitions =
                    new[] {
                        new StackTransition(new [] { elemType, typeof(NativeIntType), arr.Type }, Type.EmptyTypes),
                        new StackTransition(new [] { elemType, typeof(int), arr.Type }, Type.EmptyTypes)
                    };

                instr = OpCodes.Stelem_I;
            }

            if (!elemType.IsValueType && !instr.HasValue)
            {
                transitions =
                    new[] {
                        new StackTransition(new [] { elemType, typeof(NativeIntType), arr.Type }, Type.EmptyTypes),
                        new StackTransition(new [] { elemType, typeof(int), arr.Type }, Type.EmptyTypes)
                    };

                instr = OpCodes.Stelem_Ref;
            }

            if ((elemType == typeof(sbyte) || elemType == typeof(byte))  && !instr.HasValue)
            {
                transitions =
                    new[] {
                        new StackTransition(new [] { typeof(int), typeof(NativeIntType), arr.Type }, Type.EmptyTypes),
                        new StackTransition(new [] { typeof(int), typeof(int), arr.Type }, Type.EmptyTypes)
                    };

                instr = OpCodes.Stelem_I1;
            }

            if ((elemType == typeof(short) || elemType == typeof(ushort))  && !instr.HasValue)
            {
                transitions =
                    new[] {
                        new StackTransition(new [] { typeof(int), typeof(NativeIntType), arr.Type }, Type.EmptyTypes),
                        new StackTransition(new [] { typeof(int), typeof(int), arr.Type }, Type.EmptyTypes)
                    };

                instr = OpCodes.Stelem_I2;
            }

            if ((elemType == typeof(int) || elemType == typeof(uint))  && !instr.HasValue)
            {
                transitions =
                    new[] {
                        new StackTransition(new [] { typeof(int), typeof(NativeIntType), arr.Type }, Type.EmptyTypes),
                        new StackTransition(new [] { typeof(int), typeof(int), arr.Type }, Type.EmptyTypes)
                    };

                instr = OpCodes.Stelem_I4;
            }

            if ((elemType == typeof(long) || elemType == typeof(ulong))  && !instr.HasValue)
            {
                transitions =
                    new[] {
                        new StackTransition(new [] { typeof(long), typeof(NativeIntType), arr.Type }, Type.EmptyTypes),
                        new StackTransition(new [] { typeof(long), typeof(int), arr.Type }, Type.EmptyTypes)
                    };

                instr = OpCodes.Stelem_I8;
            }

            if (elemType == typeof(float) && !instr.HasValue)
            {
                transitions =
                    new[] {
                        new StackTransition(new [] { typeof(float), typeof(NativeIntType), arr.Type }, Type.EmptyTypes),
                        new StackTransition(new [] { typeof(float), typeof(int), arr.Type }, Type.EmptyTypes)
                    };

                instr = OpCodes.Stelem_R4;
            }

            if (elemType == typeof(double) && !instr.HasValue)
            {
                transitions =
                    new[] {
                        new StackTransition(new [] { typeof(double), typeof(NativeIntType), arr.Type }, Type.EmptyTypes),
                        new StackTransition(new [] { typeof(double), typeof(int), arr.Type }, Type.EmptyTypes)
                    };

                instr = OpCodes.Stelem_R8;
            }

            if (!instr.HasValue)
            {
                transitions =
                    new[] {
                        new StackTransition(new [] { elemType, typeof(NativeIntType), arr.Type }, Type.EmptyTypes),
                        new StackTransition(new [] { elemType, typeof(int), arr.Type }, Type.EmptyTypes)
                    };

                UpdateState(OpCodes.Stelem, elemType, transitions, pop:3);
                return this;
            }

            UpdateState(instr.Value, transitions, pop: 3);

            return this;
        }
    }
}
