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

            if (isVolatile)
            {
                UpdateState(OpCodes.Volatile, Wrap(StackTransition.None(), "StoreIndirect"));
            }

            if (unaligned.HasValue)
            {
                UpdateState(OpCodes.Unaligned, (byte)unaligned.Value, Wrap(StackTransition.None(), "StoreIndirect"));
            }

            if (type.IsPointer)
            {
                var transition = new[] { new StackTransition(new[] { typeof(NativeIntType), typeof(NativeIntType) }, TypeHelpers.EmptyTypes) };

                UpdateState(OpCodes.Stind_I, Wrap(transition, "StoreIndirect"));
                return this;
            }

            if (!TypeHelpers.IsValueType(type))
            {
                var transition = 
                    new[] 
                    { 
                        new StackTransition(new[] { type, type.MakePointerType() }, TypeHelpers.EmptyTypes),
                        new StackTransition(new[] { type, type.MakeByRefType() }, TypeHelpers.EmptyTypes),
                        new StackTransition(new[] { type, typeof(NativeIntType) }, TypeHelpers.EmptyTypes),
                    };

                UpdateState(OpCodes.Stind_Ref, Wrap(transition, "StoreIndirect"));
                return this;
            }

            if (type == typeof(sbyte) || type == typeof(byte))
            {
                var transition =
                    new[] 
                    { 
                        new StackTransition(new[] { typeof(int), typeof(byte*) }, TypeHelpers.EmptyTypes),
                        new StackTransition(new[] { typeof(int), typeof(byte).MakeByRefType() }, TypeHelpers.EmptyTypes),
                        new StackTransition(new[] { typeof(int), typeof(sbyte*) }, TypeHelpers.EmptyTypes),
                        new StackTransition(new[] { typeof(int), typeof(sbyte).MakeByRefType() }, TypeHelpers.EmptyTypes),
                        new StackTransition(new[] { typeof(int), typeof(NativeIntType) }, TypeHelpers.EmptyTypes)
                    };

                UpdateState(OpCodes.Stind_I1, Wrap(transition, "StoreIndirect"));
                return this;
            }

            if (type == typeof(short) || type == typeof(ushort))
            {
                var transition =
                    new[] 
                    { 
                        new StackTransition(new[] { typeof(int), typeof(short*) }, TypeHelpers.EmptyTypes),
                        new StackTransition(new[] { typeof(int), typeof(short).MakeByRefType() }, TypeHelpers.EmptyTypes),
                        new StackTransition(new[] { typeof(int), typeof(ushort*) }, TypeHelpers.EmptyTypes),
                        new StackTransition(new[] { typeof(int), typeof(ushort).MakeByRefType() }, TypeHelpers.EmptyTypes),
                        new StackTransition(new[] { typeof(int), typeof(NativeIntType) }, TypeHelpers.EmptyTypes)
                    };

                UpdateState(OpCodes.Stind_I2, Wrap(transition, "StoreIndirect"));
                return this;
            }

            if (type == typeof(int) || type == typeof(uint))
            {
                var transition =
                    new[] 
                    { 
                        new StackTransition(new[] { typeof(int), typeof(int*) }, TypeHelpers.EmptyTypes),
                        new StackTransition(new[] { typeof(int), typeof(int).MakeByRefType() }, TypeHelpers.EmptyTypes),
                        new StackTransition(new[] { typeof(int), typeof(uint*) }, TypeHelpers.EmptyTypes),
                        new StackTransition(new[] { typeof(int), typeof(uint).MakeByRefType() }, TypeHelpers.EmptyTypes),
                        new StackTransition(new[] { typeof(int), typeof(NativeIntType) }, TypeHelpers.EmptyTypes)
                    };

                UpdateState(OpCodes.Stind_I4, Wrap(transition, "StoreIndirect"));
                return this;
            }

            if (type == typeof(long) || type == typeof(ulong))
            {
                var transition =
                    new[] 
                    { 
                        new StackTransition(new[] { typeof(long), typeof(long*) }, TypeHelpers.EmptyTypes),
                        new StackTransition(new[] { typeof(long), typeof(long).MakeByRefType() }, TypeHelpers.EmptyTypes),
                        new StackTransition(new[] { typeof(long), typeof(ulong*) }, TypeHelpers.EmptyTypes),
                        new StackTransition(new[] { typeof(long), typeof(ulong).MakeByRefType() }, TypeHelpers.EmptyTypes),
                        new StackTransition(new[] { typeof(long), typeof(NativeIntType) }, TypeHelpers.EmptyTypes)
                    };

                UpdateState(OpCodes.Stind_I8, Wrap(transition, "StoreIndirect"));
                return this;
            }

            if (type == typeof(float))
            {
                var transition =
                    new[] 
                    { 
                        new StackTransition(new[] { typeof(float), typeof(float*) }, TypeHelpers.EmptyTypes),
                        new StackTransition(new[] { typeof(float), typeof(float).MakeByRefType() }, TypeHelpers.EmptyTypes),
                        new StackTransition(new[] { typeof(float), typeof(NativeIntType) }, TypeHelpers.EmptyTypes)
                    };

                UpdateState(OpCodes.Stind_R4, Wrap(transition, "StoreIndirect"));
                return this;
            }

            if (type == typeof(double))
            {
                var transition =
                    new[] 
                    { 
                        new StackTransition(new[] { typeof(double), typeof(double*) }, TypeHelpers.EmptyTypes),
                        new StackTransition(new[] { typeof(double), typeof(double).MakeByRefType() }, TypeHelpers.EmptyTypes),
                        new StackTransition(new[] { typeof(double), typeof(NativeIntType) }, TypeHelpers.EmptyTypes)
                    };

                UpdateState(OpCodes.Stind_R8, Wrap(transition, "StoreIndirect"));
                return this;
            }

            throw new InvalidOperationException("StoreIndirect cannot be used with " + type + ", StoreObject may be more appropriate");
        }
    }
}
