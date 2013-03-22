using Sigil.Impl;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Loads the address of the given field onto the stack.
        /// 
        /// If the field is an instance field, a `this` reference is expected on the stack and will be popped.
        /// </summary>
        public Emit<DelegateType> LoadFieldAddress(FieldInfo field)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }

            if (!AllowsUnverifiableCIL && field.IsInitOnly)
            {
                throw new InvalidOperationException("LoadFieldAddress on InitOnly fields is not verifiable");
            }

            if (!field.IsStatic)
            {
                var transitions =
                    new[] {
                        new StackTransition(new [] { field.DeclaringType }, new [] { field.FieldType.MakeByRefType() })
                    };

                UpdateState(OpCodes.Ldflda, field, Wrap(transitions, "LoadFieldAddress"));
            }
            else
            {
                UpdateState(OpCodes.Ldsflda, field, Wrap(StackTransition.Push(field.FieldType.MakeByRefType()), "LoadFieldAddress"));
            }

            return this;
        }
    }
}
