using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Takes a destination pointer, a source pointer as arguments.  Pops both off the stack.
        /// 
        /// Copies the given value type from the source to the destination.
        /// </summary>
        public Emit<DelegateType> CopyObject<ValueType>()
            where ValueType : struct
        {
            return CopyObject(typeof(ValueType));
        }

        /// <summary>
        /// Takes a destination pointer, a source pointer as arguments.  Pops both off the stack.
        /// 
        /// Copies the given value type from the source to the destination.
        /// </summary>
        public Emit<DelegateType> CopyObject(Type valueType)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException("valueType");
            }

            if (!TypeHelpers.IsValueType(valueType))
            {
                throw new ArgumentException("CopyObject expects a ValueType; found " + valueType);
            }

            var transitions =
                new[] 
                {
                    new StackTransition(new [] { typeof(NativeIntType), typeof(NativeIntType) }, TypeHelpers.EmptyTypes),
                    new StackTransition(new [] { valueType.MakePointerType(), typeof(NativeIntType) }, TypeHelpers.EmptyTypes),
                    new StackTransition(new [] { valueType.MakeByRefType(), typeof(NativeIntType) }, TypeHelpers.EmptyTypes),
                    new StackTransition(new [] { typeof(NativeIntType), valueType.MakePointerType() }, TypeHelpers.EmptyTypes),
                    new StackTransition(new [] { valueType.MakePointerType(), valueType.MakePointerType() }, TypeHelpers.EmptyTypes),
                    new StackTransition(new [] { valueType.MakeByRefType(), valueType.MakePointerType() }, TypeHelpers.EmptyTypes),
                    new StackTransition(new [] { typeof(NativeIntType), valueType.MakeByRefType() }, TypeHelpers.EmptyTypes),
                    new StackTransition(new [] { valueType.MakePointerType(), valueType.MakeByRefType() }, TypeHelpers.EmptyTypes),
                    new StackTransition(new [] { valueType.MakeByRefType(), valueType.MakeByRefType() }, TypeHelpers.EmptyTypes),
                };

            UpdateState(OpCodes.Cpobj, valueType, Wrap(transitions, "CopyObject"));

            return this;
        }
    }
}
