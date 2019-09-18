using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Boxes the given value type on the stack, converting it into a reference.
        /// </summary>
        public Emit<DelegateType> Box<ValueType>()
            where ValueType : struct
        {
            return Box(typeof(ValueType));
        }

        /// <summary>
        /// Boxes the given value type on the stack, converting it into a reference.
        /// </summary>
        public Emit<DelegateType> Box(Type valueType)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException("valueType");
            }

            if (!TypeHelpers.IsValueType(valueType) || valueType == typeof(void))
            {
                throw new ArgumentException("Only ValueTypes can be boxed, found " + valueType, "valueType");
            }

            if (!AllowsUnverifiableCIL && valueType.IsByRef)
            {
                throw new InvalidOperationException("Box with by-ref types is not verifiable");
            }

            var transitions =
                new[] 
                {
                    new StackTransition(new [] { valueType }, new [] { typeof(object) })
                };

            UpdateState(OpCodes.Box, valueType, Wrap(transitions, "Box"));

            return this;
        }
    }
}
