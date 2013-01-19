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
        public void Unbox<ValueType>()
        {
            Unbox(typeof(ValueType));
        }

        public void Unbox(Type valueType)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException("valueType");
            }

            if (!valueType.IsValueType || valueType.IsByRef || valueType.IsPointer)
            {
                throw new SigilException("Unbox expects a ValueType, found " + valueType, Stack);
            }

            if (valueType == typeof(void))
            {
                throw new SigilException("Void cannot be boxed, and thus cannot be unboxed", Stack);
            }

            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("Unbox expects a value on the stack, but it is empty", Stack);
            }

            var onStack = top.Single();

            if (onStack.IsPointer || onStack.IsReference || onStack.Type.IsValueType)
            {
                throw new SigilException("Unbox expects a ReferenceType on the stack, but found " + onStack, Stack);
            }

            UpdateState(OpCodes.Unbox, valueType, TypeOnStack.Get(valueType.MakeByRefType()), pop: 1);
        }

        public void UnboxAny<ValueType>()
        {
            UnboxAny(typeof(ValueType));
        }

        public void UnboxAny(Type valueType)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException("valueType");
            }

            if (!valueType.IsValueType || valueType.IsByRef || valueType.IsPointer)
            {
                throw new SigilException("UnboxAny expects a ValueType, found " + valueType, Stack);
            }

            if (valueType == typeof(void))
            {
                throw new SigilException("Void cannot be boxed, and thus cannot be unboxed", Stack);
            }

            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("UnboxAny expects a value on the stack, but it is empty", Stack);
            }

            var onStack = top.Single();

            if (onStack.IsPointer || onStack.IsReference || onStack.Type.IsValueType)
            {
                throw new SigilException("UnboxAny expects a ReferenceType on the stack, but found " + onStack, Stack);
            }

            UpdateState(OpCodes.Unbox_Any, valueType, TypeOnStack.Get(valueType), pop: 1);
        }
    }
}
