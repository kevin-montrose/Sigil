using Sigil.Impl;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pops a value from the stack and stores it in the given field.
        /// 
        /// If the field is an instance member, both a value and a reference to the instance are popped from the stack.
        /// </summary>
        public Emit<DelegateType> StoreField(FieldInfo field, bool isVolatile = false, int? unaligned = null)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }

            if (unaligned.HasValue && (unaligned != 1 && unaligned != 2 && unaligned != 4))
            {
                throw new ArgumentException("unaligned must be null, 1, 2, or 4");
            }

            if (unaligned.HasValue && field.IsStatic)
            {
                throw new ArgumentException("unaligned cannot be used with static fields");
            }

            if (!field.IsStatic)
            {
                if (isVolatile)
                {
                    UpdateState(OpCodes.Volatile, Wrap(StackTransition.None(), "StoreField"));
                }

                if (unaligned.HasValue)
                {
                    UpdateState(OpCodes.Unaligned, (byte)unaligned.Value, Wrap(StackTransition.None(), "StoreField"));
                }

                var onType = field.DeclaringType;
                if (TypeHelpers.IsValueType(onType))
                {
                    onType = onType.MakePointerType();
                }

                var transitions =
                    new[]
                    {
                        new StackTransition(new [] { field.FieldType, onType }, TypeHelpers.EmptyTypes)
                    };

                UpdateState(OpCodes.Stfld, field, Wrap(transitions, "StoreField"));
            }
            else
            {
                if (isVolatile)
                {
                    UpdateState(OpCodes.Volatile, Wrap(StackTransition.None(), "StoreField"));
                }

                var transitions =
                    new[]
                    {
                        new StackTransition(new [] { field.FieldType }, TypeHelpers.EmptyTypes)
                    };

                UpdateState(OpCodes.Stsfld, field, Wrap(transitions, "StoreField"));
            }

            return this;
        }
    }
}
