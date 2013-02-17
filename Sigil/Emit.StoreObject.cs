using Sigil.Impl;
using System;
using System.Reflection.Emit;

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
                FailStackUnderflow(2);
            }

            var val = onStack[0];
            var addr = onStack[1];

            if (TypeOnStack.Get(valueType) != val)
            {
                throw new SigilVerificationException("StoreObject expected a " + valueType + " to be on the stack, found " + val, IL.Instructions(LocalsByIndex), Stack, 0);
            }

            if (!addr.IsPointer && !addr.IsReference && addr != TypeOnStack.Get<NativeIntType>())
            {
                throw new SigilVerificationException("StoreObject expected a reference, pointer, or native int; found " + addr, IL.Instructions(LocalsByIndex), Stack, 1);
            }

            if (isVolatile)
            {
                UpdateState(OpCodes.Volatile, StackTransition.None().Wrap("StoreObject"));
            }

            if (unaligned.HasValue)
            {
                UpdateState(OpCodes.Unaligned, unaligned.Value, StackTransition.None().Wrap("StoreObject"));
            }

            var transitions =
                    new[]
                    {
                        new StackTransition(new [] { valueType, typeof(NativeIntType) }, Type.EmptyTypes),
                        new StackTransition(new [] { valueType, valueType.MakePointerType() }, Type.EmptyTypes),
                        new StackTransition(new [] { valueType, valueType.MakeByRefType() }, Type.EmptyTypes)
                    };

            UpdateState(OpCodes.Stobj, valueType, transitions.Wrap("StoreObject"), pop: 2);

            return this;
        }
    }
}
