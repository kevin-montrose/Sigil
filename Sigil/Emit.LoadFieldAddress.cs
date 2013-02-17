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
                var onStack = Stack.Top();

                if (onStack == null)
                {
                    FailStackUnderflow(1);
                }

                var val = onStack[0];

                if (!field.DeclaringType.IsAssignableFrom(val))
                {
                    throw new SigilVerificationException("LoadFieldAddress expected a " + field.DeclaringType + ", found " + val, IL.Instructions(LocalsByIndex), Stack, 0);
                }

                var transitions =
                    new[] {
                        new StackTransition(new [] { field.DeclaringType }, new [] { field.FieldType.MakeByRefType() })
                    };

                UpdateState(OpCodes.Ldflda, field, transitions, TypeOnStack.Get(field.FieldType.MakeByRefType()), pop: 1);
            }
            else
            {
                UpdateState(OpCodes.Ldsflda, field, StackTransition.Push(field.FieldType.MakeByRefType()), TypeOnStack.Get(field.FieldType.MakeByRefType()));
            }

            return this;
        }
    }
}
