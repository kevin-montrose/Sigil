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
                    new StackTransition(new [] { typeof(NativeIntType) }, Type.EmptyTypes),
                    new StackTransition(new [] { valueType.MakePointerType() }, Type.EmptyTypes),
                    new StackTransition(new [] { valueType.MakeByRefType() }, Type.EmptyTypes),
                };

            UpdateState(OpCodes.Initobj, valueType, transitions.Wrap("InitializeObject"), pop: 1);

            return this;
        }
    }
}
