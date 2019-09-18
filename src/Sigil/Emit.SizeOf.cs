using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pushes the size of the given value type onto the stack.
        /// </summary>
        public Emit<DelegateType> SizeOf<ValueType>()
            where ValueType : struct
        {
            return SizeOf(typeof(ValueType));
        }

        /// <summary>
        /// Pushes the size of the given value type onto the stack.
        /// </summary>
        public Emit<DelegateType> SizeOf(Type valueType)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException("valueType");
            }

            if (!TypeHelpers.IsValueType(valueType))
            {
                throw new ArgumentException("valueType must be a ValueType");
            }

            UpdateState(OpCodes.Sizeof, valueType, Wrap(StackTransition.Push<int>(), "SizeOf"));

            return this;
        }
    }
}
