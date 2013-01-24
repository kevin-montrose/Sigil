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
        public void StoreIndirect<Type>()
        {
            StoreIndirect(typeof(Type));
        }

        public void StoreIndirect(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            var onStack = Stack.Top(2);

            if (onStack == null)
            {
                throw new SigilException("StoreIndirect expected two values on the stack", Stack);
            }

            var val = onStack[0];
            var addr = onStack[1];

            if (!val.IsAssignableFrom(type))
            {
                throw new SigilException("StoreIndirect expected a " + type + " on the stack, found " + val, Stack);
            }

            if (!addr.IsPointer && !addr.IsReference && addr != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilException("StoreIndirected expected a reference, pointer, or native int on the stack; found " + addr, Stack);
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

            UpdateState(OpCodes.Stind_Ref, pop: 2);
        }
    }
}
