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

            if (!valueType.IsValueType || valueType == typeof(void))
            {
                throw new ArgumentException("Only ValueTypes can be boxed, found " + valueType, "valueType");
            }

            if (!AllowsUnverifiableCIL && valueType.IsByRef)
            {
                throw new InvalidOperationException("Box with by-ref types is not verifiable");
            }

            var top = Stack.Top();

            if (top == null)
            {
                FailStackUnderflow(1);
            }

            var onStack = top.Single();

            var actually32Bit = valueType.IsPrimitive && valueType.IsAssignableFrom(TypeOnStack.Get<int>());

            if (actually32Bit)
            {
                if (!TypeOnStack.Get<int>().IsAssignableFrom(onStack))
                {
                    throw new SigilVerificationException(onStack + " cannot be boxed as an " + valueType, IL, Stack, 0);
                }
            }
            else
            {
                if (onStack != TypeOnStack.Get(valueType))
                {
                    throw new SigilVerificationException("Expected " + valueType + " to be on the stack, found " + onStack, IL, Stack, 0);
                }
            }

            UpdateState(OpCodes.Box, valueType, TypeOnStack.Get(typeof(object)), pop: 1);

            return this;
        }
    }
}
