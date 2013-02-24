using Sigil.Impl;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Loads a field onto the stack.
        /// 
        /// Instance fields expect a reference on the stack, which is popped.
        /// </summary>
        public Emit<DelegateType> LoadField(FieldInfo field, bool? isVolatile = null, int? unaligned = null)
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

            var useVolatile = isVolatile ?? field.IsVolatile();

            if (!field.IsStatic)
            {
                if (useVolatile)
                {
                    UpdateState(OpCodes.Volatile, StackTransition.None().Wrap("LoadField"));
                }

                if (unaligned.HasValue)
                {
                    UpdateState(OpCodes.Unaligned, (byte)unaligned.Value, StackTransition.None().Wrap("LoadField"));
                }

                var transitions =
                    new[]
                    {
                        new StackTransition(new [] { field.DeclaringType }, new [] { field.FieldType })
                    };

                UpdateState(OpCodes.Ldfld, field, transitions.Wrap("LoadField"));
            }
            else
            {
                if (useVolatile)
                {
                    UpdateState(OpCodes.Volatile, StackTransition.None().Wrap("LoadField"));
                }

                UpdateState(OpCodes.Ldsfld, field, StackTransition.Push(field.FieldType).Wrap("LoadField"));
            }

            return this;
        }
    }
}
