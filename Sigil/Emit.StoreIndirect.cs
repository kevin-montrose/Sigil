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
                throw new SigilVerificationException("StoreIndirect expected a " + type + " on the stack, found " + val, IL.Instructions(Locals), Stack, 0);
            }

            if (!addr.IsPointer && !addr.IsReference && addr != TypeOnStack.Get<NativeIntType>())
            {
                throw new SigilVerificationException("StoreIndirect expected a reference, pointer, or native int on the stack; found " + addr, IL.Instructions(Locals), Stack, 1);
            }

            if (isVolatile)
            {
                UpdateState(OpCodes.Volatile);
            }

            if (unaligned.HasValue)
            {
                UpdateState(OpCodes.Unaligned, unaligned.Value);
            }

            if (type.IsPointer)
            {
                UpdateState(OpCodes.Stind_I, pop: 2);
                return this;
            }

            if (!type.IsValueType)
            {
                UpdateState(OpCodes.Stind_Ref, pop: 2);
                return this;
            }

            if (type == typeof(sbyte) || type == typeof(byte))
            {
                UpdateState(OpCodes.Stind_I1, pop:2);
                return this;
            }

            if (type == typeof(short) || type == typeof(ushort))
            {
                UpdateState(OpCodes.Stind_I2, pop: 2);
                return this;
            }

            if (type == typeof(int) || type == typeof(uint))
            {
                UpdateState(OpCodes.Stind_I4, pop: 2);
                return this;
            }

            if (type == typeof(long) || type == typeof(ulong))
            {
                UpdateState(OpCodes.Stind_I8, pop: 2);
                return this;
            }

            if (type == typeof(float))
            {
                UpdateState(OpCodes.Stind_R4, pop: 2);
                return this;
            }

            if (type == typeof(double))
            {
                UpdateState(OpCodes.Stind_R8, pop: 2);
                return this;
            }

            throw new InvalidOperationException("StoreIndirect cannot be used with " + type + ", StoreObject may be more appropriate");
        }
    }
}
