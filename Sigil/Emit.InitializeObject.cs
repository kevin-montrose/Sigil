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

            var transitions =
                new[]
                {
                    new StackTransition(new [] { typeof(NativeIntType) }, TypeHelpers.EmptyTypes),
                    new StackTransition(new [] { valueType.MakePointerType() }, TypeHelpers.EmptyTypes),
                    new StackTransition(new [] { valueType.MakeByRefType() }, TypeHelpers.EmptyTypes),
                };

            UpdateState(OpCodes.Initobj, valueType, Wrap(transitions, "InitializeObject"));

            return this;
        }
    }
}
