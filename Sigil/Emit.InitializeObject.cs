using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Expects an instance of the type to be initialized on the stack.
        /// 
        /// Initializes all the fields on a value type to null or an appropriate zero value.
        /// </summary>
        public Emit<DelegateType> InitializeObject<ValueType>()
        {
            return InitializeObject(typeof(ValueType));
        }

        /// <summary>
        /// Expects an instance of the type to be initialized on the stack.
        /// 
        /// Initializes all the fields on a value type to null or an appropriate zero value.
        /// </summary>
        public Emit<DelegateType> InitializeObject(Type valueType)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException("valueType");
            }

            var onStack = Stack.Top();

            if (onStack == null)
            {
                FailStackUnderflow(1);
            }

            var obj = onStack[0];

            if (obj != TypeOnStack.Get<NativeInt>() && obj != TypeOnStack.Get(valueType.MakePointerType()) && obj != TypeOnStack.Get(valueType.MakeByRefType()))
            {
                throw new SigilVerificationException("InitializeObject expected a reference or pointer to a " + valueType + ", or a native int, to be on the stack; found " + obj, IL.Instructions(Locals), Stack, 0);
            }

            UpdateState(OpCodes.Initobj, valueType, pop: 1);

            return this;
        }
    }
}
