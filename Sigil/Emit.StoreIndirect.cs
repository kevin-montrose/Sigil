using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pops a value of the given type and a pointer off the stack, and stores the value at the address in the pointer.
        /// </summary>
        public void StoreIndirect<Type>(bool isVolatile = false, int? unaligned = null)
        {
            StoreIndirect(typeof(Type), isVolatile, unaligned);
        }

        /// <summary>
        /// Pops a value of the given type and a pointer off the stack, and stores the value at the address in the pointer.
        /// </summary>
        public void StoreIndirect(Type type, bool isVolatile = false, int? unaligned = null)
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
                throw new SigilException("StoreIndirect expected two values on the stack", Stack);
            }

            var val = onStack[0];
            var addr = onStack[1];

            if (!type.IsAssignableFrom(val))
            {
                throw new SigilException("StoreIndirect expected a " + type + " on the stack, found " + val, Stack);
            }

            if (!addr.IsPointer && !addr.IsReference && addr != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilException("StoreIndirect expected a reference, pointer, or native int on the stack; found " + addr, Stack);
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
                return;
            }

            if (!type.IsValueType)
            {
                UpdateState(OpCodes.Stind_Ref, pop: 2);
                return;
            }

            if (type == typeof(sbyte) || type == typeof(byte))
            {
                UpdateState(OpCodes.Stind_I1, pop:2);
                return;
            }

            if (type == typeof(short) || type == typeof(ushort))
            {
                UpdateState(OpCodes.Stind_I2, pop: 2);
                return;
            }

            if (type == typeof(int) || type == typeof(uint))
            {
                UpdateState(OpCodes.Stind_I4, pop: 2);
                return;
            }

            if (type == typeof(long) || type == typeof(ulong))
            {
                UpdateState(OpCodes.Stind_I8, pop: 2);
                return;
            }

            if (type == typeof(float))
            {
                UpdateState(OpCodes.Stind_R4, pop: 2);
                return;
            }

            if (type == typeof(double))
            {
                UpdateState(OpCodes.Stind_R8, pop: 2);
                return;
            }

            throw new InvalidOperationException("StoreIndirect cannot be used with " + type + ", StoreObject may be more appropriate");
        }
    }
}
