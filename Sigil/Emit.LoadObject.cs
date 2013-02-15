using Sigil.Impl;
using System;
using System.Reflection.Emit;

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
                FailStackUnderflow(1);
            }

            var ptr = onStack[0];

            if (!ptr.IsReference && !ptr.IsPointer && ptr != TypeOnStack.Get<NativeIntType>())
            {
                throw new SigilVerificationException("LoadObject expected a reference, pointer, or native int; found " + ptr, IL.Instructions(LocalsByIndex), Stack, 0);
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
