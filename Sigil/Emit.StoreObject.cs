using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public void StoreObject<ValueType>()
        {
            StoreObject(typeof(ValueType));
        }

        public void StoreObject(Type valueType)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException("valueType");
            }

            if (!valueType.IsValueType)
            {
                throw new ArgumentException("valueType must be a ValueType");
            }

            var onStack = Stack.Top(2);

            if (onStack == null)
            {
                throw new SigilException("StoreObject expects two values on the stack", Stack);
            }

            var val = onStack[0];
            var addr = onStack[1];

            if (TypeOnStack.Get(valueType) != val)
            {
                throw new SigilException("StoreObject expected a " + valueType + " to be on the stack, found " + val, Stack);
            }

            if (!addr.IsPointer && !addr.IsReference && addr != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilException("StoreObject expected a reference, pointer, or native int; found " + addr, Stack);
            }

            UpdateState(OpCodes.Stobj, pop: 2);
        }
    }
}
