using Sigil.Impl;
using System;
using System.Linq;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pops a boxed value from the stack and pushes a pointer to it's unboxed value.
        /// 
        /// To load the value directly onto the stack, use UnboxAny().
        /// </summary>
        public Emit<DelegateType> Unbox<ValueType>()
        {
            return Unbox(typeof(ValueType));
        }

        /// <summary>
        /// Pops a boxed value from the stack and pushes a pointer to it's unboxed value.
        /// 
        /// To load the value directly onto the stack, use UnboxAny().
        /// </summary>
        public Emit<DelegateType> Unbox(Type valueType)
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
                FailStackUnderflow(1);
            }

            var onStack = top.Single();

            if (onStack != TypeOnStack.Get<object>())
            {
                throw new SigilVerificationException("Unbox expects an object on the stack, but found " + onStack, IL.Instructions(Locals), Stack, 0);
            }

            UpdateState(OpCodes.Unbox, valueType, TypeOnStack.Get(valueType.MakeByRefType()), pop: 1);

            return this;
        }

        /// <summary>
        /// Pops a boxed value from the stack, unboxes it and pushes the value onto the stack.
        /// 
        /// To get an address for the unboxed value instead, use Unbox().
        /// </summary>
        public Emit<DelegateType> UnboxAny<ValueType>()
        {
            return UnboxAny(typeof(ValueType));
        }

        /// <summary>
        /// Pops a boxed value from the stack, unboxes it and pushes the value onto the stack.
        /// 
        /// To get an address for the unboxed value instead, use Unbox().
        /// </summary>
        public Emit<DelegateType> UnboxAny(Type valueType)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException("valueType");
            }

            if (valueType.IsByRef || valueType.IsPointer)
            {
                throw new ArgumentException("UnboxAny cannot operate on pointers, found " + valueType);
            }

            if (valueType == typeof(void))
            {
                throw new ArgumentException("Void cannot be boxed, and thus cannot be unboxed");
            }

            var top = Stack.Top();

            if (top == null)
            {
                FailStackUnderflow(1);
            }

            var onStack = top.Single();

            if (onStack != TypeOnStack.Get<object>())
            {
                throw new SigilVerificationException("UnboxAny expects an object on the stack, but found " + onStack, IL.Instructions(Locals), Stack, 0);
            }

            UpdateState(OpCodes.Unbox_Any, valueType, TypeOnStack.Get(valueType), pop: 1);

            return this;
        }
    }
}
