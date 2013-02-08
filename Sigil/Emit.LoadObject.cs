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
        /// Pops a pointer from the stack, and pushes the given value type it points to onto the stack.
        /// 
        /// For primitive and reference types, use LoadIndirect().
        /// </summary>
        public Emit<DelegateType> LoadObject<ValueType>(bool isVolatile = false, int? unaligned = null)
            where ValueType : struct
        {
            return LoadObject(typeof(ValueType), isVolatile, unaligned);
        }

        /// <summary>
        /// Pops a pointer from the stack, and pushes the given value type it points to onto the stack.
        /// 
        /// For primitive and reference types, use LoadIndirect().
        /// </summary>
        public Emit<DelegateType> LoadObject(Type valueType, bool isVolatile = false, int? unaligned = null)
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

            var onStack = Stack.Top();
            if (onStack == null)
            {
                throw new SigilVerificationException("LoadObject expected a value on the stack, but it was empty", IL, Stack);
            }

            var ptr = onStack[0];

            if (!ptr.IsReference && !ptr.IsPointer && ptr != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilVerificationException("LoadObject expected a reference, pointer, or native int; found " + ptr, IL, Stack);
            }

            if (isVolatile)
            {
                UpdateState(OpCodes.Volatile);
            }

            if (unaligned.HasValue)
            {
                UpdateState(OpCodes.Unaligned, unaligned.Value);
            }

            UpdateState(OpCodes.Ldobj, valueType, TypeOnStack.Get(valueType), pop: 1);

            return this;
        }
    }
}
