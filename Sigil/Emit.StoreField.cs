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
                var onStack = Stack.Top(2);

                if (onStack == null)
                {
                    FailStackUnderflow(2);
                }

                var type = onStack[1];
                var val = onStack[0];

                if (!field.DeclaringType.IsAssignableFrom(type))
                {
                    throw new SigilVerificationException("StoreField expected a type on the stack assignable to " + field.DeclaringType + ", found " + type, IL.Instructions(LocalsByIndex), Stack, 1);
                }

                if (!field.FieldType.IsAssignableFrom(val))
                {
                    throw new SigilVerificationException("StoreField expected a type on the stack assignable to " + field.FieldType + ", found " + val, IL.Instructions(LocalsByIndex), Stack, 0);
                }

                if (isVolatile)
                {
                    UpdateState(OpCodes.Volatile, StackTransition.None());
                }

                if (unaligned.HasValue)
                {
                    UpdateState(OpCodes.Unaligned, unaligned.Value, StackTransition.None());
                }

                var transitions =
                    new[]
                    {
                        new StackTransition(new [] { field.FieldType, field.DeclaringType }, Type.EmptyTypes)
                    };

                UpdateState(OpCodes.Stfld, field, transitions, pop: 2);
            }
            else
            {
                var onStack = Stack.Top();

                if (onStack == null)
                {
                    FailStackUnderflow(1);
                }

                var val = onStack[0];

                if (!field.FieldType.IsAssignableFrom(val))
                {
                    throw new SigilVerificationException("StoreField expected a type on the stack assignable to " + field.FieldType + ", found " + val, IL.Instructions(LocalsByIndex), Stack, 0);
                }

                if (isVolatile)
                {
                    UpdateState(OpCodes.Volatile, StackTransition.None());
                }

                var transitions =
                    new[]
                    {
                        new StackTransition(new [] { field.FieldType }, Type.EmptyTypes)
                    };

                UpdateState(OpCodes.Stsfld, field, transitions, pop: 1);
            }

            return this;
        }
    }
}
