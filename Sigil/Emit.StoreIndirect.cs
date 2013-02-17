using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pops a value of the given type and a pointer off the stack, and stores the value at the address in the pointer.
        /// </summary>
        public Emit<DelegateType> StoreIndirect<Type>(bool isVolatile = false, int? unaligned = null)
        {
            return StoreIndirect(typeof(Type), isVolatile, unaligned);
        }

        /// <summary>
        /// Pops a value of the given type and a pointer off the stack, and stores the value at the address in the pointer.
        /// </summary>
        public Emit<DelegateType> StoreIndirect(Type type, bool isVolatile = false, int? unaligned = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (unaligned.HasValue && (unaligned != 1 && unaligned != 2 && unaligned != 4))
            {
                throw new ArgumentException("unaligned must be null, 1, 2, or 4");
            }

            var onStack = Stack.Top(2);

            if (onStack == null)
            {
                FailStackUnderflow(2);
            }

            var val = onStack[0];
            var addr = onStack[1];

            if (!type.IsAssignableFrom(val))
            {
                throw new SigilVerificationException("StoreIndirect expected a " + type + " on the stack, found " + val, IL.Instructions(LocalsByIndex), Stack, 0);
            }

            if (!addr.IsPointer && !addr.IsReference && addr != TypeOnStack.Get<NativeIntType>())
            {
                throw new SigilVerificationException("StoreIndirect expected a reference, pointer, or native int on the stack; found " + addr, IL.Instructions(LocalsByIndex), Stack, 1);
            }

            if (isVolatile)
            {
                UpdateState(OpCodes.Volatile, StackTransition.None());
            }

            if (unaligned.HasValue)
            {
                UpdateState(OpCodes.Unaligned, unaligned.Value, StackTransition.None());
            }

            if (type.IsPointer)
            {
                var transition = new[] { new StackTransition(new[] { typeof(NativeIntType), typeof(NativeIntType) }, Type.EmptyTypes) };

                UpdateState(OpCodes.Stind_I, transition, pop: 2);
                return this;
            }

            if (!type.IsValueType)
            {
                var transition = 
                    new[] 
                    { 
                        new StackTransition(new[] { type, type.MakePointerType() }, Type.EmptyTypes),
                        new StackTransition(new[] { type, type.MakeByRefType() }, Type.EmptyTypes),
                        new StackTransition(new[] { type, typeof(NativeIntType) }, Type.EmptyTypes),
                    };

                UpdateState(OpCodes.Stind_Ref, transition, pop: 2);
                return this;
            }

            if (type == typeof(sbyte) || type == typeof(byte))
            {
                var transition =
                    new[] 
                    { 
                        new StackTransition(new[] { typeof(int), typeof(byte*) }, Type.EmptyTypes),
                        new StackTransition(new[] { typeof(int), typeof(byte).MakeByRefType() }, Type.EmptyTypes),
                        new StackTransition(new[] { typeof(int), typeof(sbyte*) }, Type.EmptyTypes),
                        new StackTransition(new[] { typeof(int), typeof(sbyte).MakeByRefType() }, Type.EmptyTypes),
                        new StackTransition(new[] { typeof(int), typeof(NativeIntType) }, Type.EmptyTypes)
                    };

                UpdateState(OpCodes.Stind_I1, transition, pop:2);
                return this;
            }

            if (type == typeof(short) || type == typeof(ushort))
            {
                var transition =
                    new[] 
                    { 
                        new StackTransition(new[] { typeof(int), typeof(short*) }, Type.EmptyTypes),
                        new StackTransition(new[] { typeof(int), typeof(short).MakeByRefType() }, Type.EmptyTypes),
                        new StackTransition(new[] { typeof(int), typeof(ushort*) }, Type.EmptyTypes),
                        new StackTransition(new[] { typeof(int), typeof(ushort).MakeByRefType() }, Type.EmptyTypes),
                        new StackTransition(new[] { typeof(int), typeof(NativeIntType) }, Type.EmptyTypes)
                    };

                UpdateState(OpCodes.Stind_I2, transition, pop: 2);
                return this;
            }

            if (type == typeof(int) || type == typeof(uint))
            {
                var transition =
                    new[] 
                    { 
                        new StackTransition(new[] { typeof(int), typeof(int*) }, Type.EmptyTypes),
                        new StackTransition(new[] { typeof(int), typeof(int).MakeByRefType() }, Type.EmptyTypes),
                        new StackTransition(new[] { typeof(int), typeof(uint*) }, Type.EmptyTypes),
                        new StackTransition(new[] { typeof(int), typeof(uint).MakeByRefType() }, Type.EmptyTypes),
                        new StackTransition(new[] { typeof(int), typeof(NativeIntType) }, Type.EmptyTypes)
                    };

                UpdateState(OpCodes.Stind_I4, transition, pop: 2);
                return this;
            }

            if (type == typeof(long) || type == typeof(ulong))
            {
                var transition =
                    new[] 
                    { 
                        new StackTransition(new[] { typeof(long), typeof(long*) }, Type.EmptyTypes),
                        new StackTransition(new[] { typeof(long), typeof(long).MakeByRefType() }, Type.EmptyTypes),
                        new StackTransition(new[] { typeof(long), typeof(ulong*) }, Type.EmptyTypes),
                        new StackTransition(new[] { typeof(long), typeof(ulong).MakeByRefType() }, Type.EmptyTypes),
                        new StackTransition(new[] { typeof(long), typeof(NativeIntType) }, Type.EmptyTypes)
                    };

                UpdateState(OpCodes.Stind_I8, transition, pop: 2);
                return this;
            }

            if (type == typeof(float))
            {
                var transition =
                    new[] 
                    { 
                        new StackTransition(new[] { typeof(float), typeof(float*) }, Type.EmptyTypes),
                        new StackTransition(new[] { typeof(float), typeof(float).MakeByRefType() }, Type.EmptyTypes),
                        new StackTransition(new[] { typeof(float), typeof(NativeIntType) }, Type.EmptyTypes)
                    };

                UpdateState(OpCodes.Stind_R4, transition, pop: 2);
                return this;
            }

            if (type == typeof(double))
            {
                var transition =
                    new[] 
                    { 
                        new StackTransition(new[] { typeof(double), typeof(double*) }, Type.EmptyTypes),
                        new StackTransition(new[] { typeof(double), typeof(double).MakeByRefType() }, Type.EmptyTypes),
                        new StackTransition(new[] { typeof(double), typeof(NativeIntType) }, Type.EmptyTypes)
                    };

                UpdateState(OpCodes.Stind_R8, transition, pop: 2);
                return this;
            }

            throw new InvalidOperationException("StoreIndirect cannot be used with " + type + ", StoreObject may be more appropriate");
        }
    }
}
