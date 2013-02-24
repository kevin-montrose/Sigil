using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pops a pointer from the stack, and pushes the given value type it points to onto the stack.
        /// 
        /// For primitive and reference types, use LoadIndirect().
        /// </summary>
        public Emit<DelegateType> LoadObject<ValueType>(bool isVolatile = false, int? unaligned = null)
            where ValueType : struct
        {
            return LoadObject(typeof(ValueType), isVolatile, unaligned);
        }

        /// <summary>
        /// Pops a pointer from the stack, and pushes the given value type it points to onto the stack.
        /// 
        /// For primitive and reference types, use LoadIndirect().
        /// </summary>
        public Emit<DelegateType> LoadObject(Type valueType, bool isVolatile = false, int? unaligned = null)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException("valueType");
            }

            if (!valueType.IsValueType)
            {
                throw new ArgumentException("valueType must be a ValueType");
            }

            if (unaligned.HasValue && (unaligned != 1 && unaligned != 2 && unaligned != 4))
            {
                throw new ArgumentException("unaligned must be null, 1, 2, or 4");
            }

            if (isVolatile)
            {
                UpdateState(OpCodes.Volatile, StackTransition.None().Wrap("LoadObject"));
            }

            if (unaligned.HasValue)
            {
                UpdateState(OpCodes.Unaligned, (byte)unaligned.Value, StackTransition.None().Wrap("LoadObject"));
            }

            var transitions =
                    new[]
                    {
                        new StackTransition(new [] { typeof(NativeIntType) }, new [] { valueType }),
                        new StackTransition(new [] { valueType.MakePointerType() }, new [] { valueType }),
                        new StackTransition(new [] { valueType.MakeByRefType() }, new [] { valueType })
                    };

            UpdateState(OpCodes.Ldobj, valueType, transitions.Wrap("LoadObject"));

            return this;
        }
    }
}
