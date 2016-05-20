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
            
            var useVolatile = isVolatile ?? ExtensionMethods.IsVolatile(field);

            if (!field.IsStatic)
            {
                if (useVolatile)
                {
                    UpdateState(OpCodes.Volatile, Wrap(StackTransition.None(), "LoadField"));
                }

                if (unaligned.HasValue)
                {
                    UpdateState(OpCodes.Unaligned, (byte)unaligned.Value, Wrap(StackTransition.None(), "LoadField"));
                }

                StackTransition[] transitions;
                if (TypeHelpers.IsValueType(field.DeclaringType))
                {
                    transitions =
                        new[]
                        {
                            // This transition isn't really to spec... but it seems to work consistently in .NET soooo.... yeah
                            new StackTransition(new [] { field.DeclaringType }, new [] { field.FieldType }),
                            new StackTransition(new [] { field.DeclaringType.MakePointerType() }, new [] { field.FieldType })
                        };
                }
                else
                {
                    transitions =
                        new[]
                        {
                            new StackTransition(new [] { field.DeclaringType }, new [] { field.FieldType })
                        };
                }

                UpdateState(OpCodes.Ldfld, field, Wrap(transitions, "LoadField"));
            }
            else
            {
                if (useVolatile)
                {
                    UpdateState(OpCodes.Volatile, Wrap(StackTransition.None(), "LoadField"));
                }

                UpdateState(OpCodes.Ldsfld, field, Wrap(StackTransition.Push(field.FieldType), "LoadField"));
            }

            return this;
        }
    }
}
