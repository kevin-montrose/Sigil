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
        /// Pops a boxed value from the stack and pushes a pointer to it's unboxed value.
        /// 
        /// To load the value directly onto the stack, use UnboxAny().
        /// </summary>
        public void Unbox<ValueType>()
        {
            Unbox(typeof(ValueType));
        }

        /// <summary>
        /// Pops a boxed value from the stack and pushes a pointer to it's unboxed value.
        /// 
        /// To load the value directly onto the stack, use UnboxAny().
        /// </summary>
        public void Unbox(Type valueType)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException("valueType");
            }

            if (!valueType.IsValueType || valueType.IsByRef || valueType.IsPointer)
            {
                throw new ArgumentException("Unbox expects a ValueType, found " + valueType);
            }

            if (valueType == typeof(void))
            {
                throw new ArgumentException("Void cannot be boxed, and thus cannot be unboxed");
            }

            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("Unbox expects a value on the stack, but it is empty", Stack);
            }

            var onStack = top.Single();

            if (onStack != TypeOnStack.Get<object>())
            {
                throw new SigilException("Unbox expects an object on the stack, but found " + onStack, Stack);
            }

            UpdateState(OpCodes.Unbox, valueType, TypeOnStack.Get(valueType.MakeByRefType()), pop: 1);
        }

        /// <summary>
        /// Pops a boxed value from the stack, unboxes it and pushes the value onto the stack.
        /// 
        /// To get an address for the unboxed value instead, use Unbox().
        /// </summary>
        public void UnboxAny<ValueType>()
        {
            UnboxAny(typeof(ValueType));
        }

        /// <summary>
        /// Pops a boxed value from the stack, unboxes it and pushes the value onto the stack.
        /// 
        /// To get an address for the unboxed value instead, use Unbox().
        /// </summary>
        public void UnboxAny(Type valueType)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException("valueType");
            }

            if (!valueType.IsValueType || valueType.IsByRef || valueType.IsPointer)
            {
                throw new ArgumentException("UnboxAny expects a ValueType, found " + valueType);
            }

            if (valueType == typeof(void))
            {
                throw new ArgumentException("Void cannot be boxed, and thus cannot be unboxed");
            }

            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("UnboxAny expects a value on the stack, but it is empty", Stack);
            }

            var onStack = top.Single();

            if (onStack != TypeOnStack.Get<object>())
            {
                throw new SigilException("UnboxAny expects an object on the stack, but found " + onStack, Stack);
            }

            UpdateState(OpCodes.Unbox_Any, valueType, TypeOnStack.Get(valueType), pop: 1);
        }
    }
}
