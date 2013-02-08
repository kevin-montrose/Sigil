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
        /// <summary>
        /// Pops a value type and a pointer off of the stack and copies the given value to the given address.
        /// 
        /// For primitive and reference types use StoreIndirect.
        /// </summary>
        public Emit<DelegateType> StoreObject<ValueType>(bool isVolatile = false, int? unaligned = null)
            where ValueType : struct
        {
            return StoreObject(typeof(ValueType), isVolatile, unaligned);
        }

        /// <summary>
        /// Pops a value type and a pointer off of the stack and copies the given value to the given address.
        /// 
        /// For primitive and reference types use StoreIndirect.
        /// </summary>
        public Emit<DelegateType> StoreObject(Type valueType, bool isVolatile = false, int? unaligned = null)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException("valueType");
            }

            if (!valueType.IsValueType)
            {
                throw new ArgumentException("valueType must be a ValueType");
            }

            if (unaligned.HasValue && (unaligned != 1 && unaligned != 2 && unaligned != 4))
            {
                throw new ArgumentException("unaligned must be null, 1, 2, or 4");
            }

            var onStack = Stack.Top(2);

            if (onStack == null)
            {
                throw new SigilVerificationException("StoreObject expects two values on the stack", IL, Stack);
            }

            var val = onStack[0];
            var addr = onStack[1];

            if (TypeOnStack.Get(valueType) != val)
            {
                throw new SigilVerificationException("StoreObject expected a " + valueType + " to be on the stack, found " + val, IL, Stack);
            }

            if (!addr.IsPointer && !addr.IsReference && addr != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilVerificationException("StoreObject expected a reference, pointer, or native int; found " + addr, IL, Stack);
            }

            if (isVolatile)
            {
                UpdateState(OpCodes.Volatile);
            }

            if (unaligned.HasValue)
            {
                UpdateState(OpCodes.Unaligned, unaligned.Value);
            }

            UpdateState(OpCodes.Stobj, valueType, pop: 2);

            return this;
        }
    }
}
