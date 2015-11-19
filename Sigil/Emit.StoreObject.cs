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

            if (!TypeHelpers.IsValueType(valueType))
            {
                throw new ArgumentException("valueType must be a ValueType");
            }

            if (unaligned.HasValue && (unaligned != 1 && unaligned != 2 && unaligned != 4))
            {
                throw new ArgumentException("unaligned must be null, 1, 2, or 4");
            }

            if (isVolatile)
            {
                UpdateState(OpCodes.Volatile, Wrap(StackTransition.None(), "StoreObject"));
            }

            if (unaligned.HasValue)
            {
                UpdateState(OpCodes.Unaligned, (byte)unaligned.Value, Wrap(StackTransition.None(), "StoreObject"));
            }

            var transitions =
                    new[]
                    {
                        new StackTransition(new [] { valueType, typeof(NativeIntType) }, TypeHelpers.EmptyTypes),
                        new StackTransition(new [] { valueType, valueType.MakePointerType() }, TypeHelpers.EmptyTypes),
                        new StackTransition(new [] { valueType, valueType.MakeByRefType() }, TypeHelpers.EmptyTypes)
                    };

            UpdateState(OpCodes.Stobj, valueType, Wrap(transitions, "StoreObject"));

            return this;
        }
    }
}
