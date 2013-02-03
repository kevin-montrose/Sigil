using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Loads the address of the given field onto the stack.
        /// 
        /// If the field is an instance field, a `this` reference is expected on the stack and will be popped.
        /// </summary>
        public void LoadFieldAddress(FieldInfo field)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }

            if (!field.IsStatic)
            {
                var onStack = Stack.Top();

                if (onStack == null)
                {
                    throw new SigilException("LoadFieldAddress expected a value on the stack, but it was empty", Stack);
                }

                var val = onStack[0];

                if (!field.DeclaringType.IsAssignableFrom(val))
                {
                    throw new SigilException("LoadFieldAddress expected a " + field.DeclaringType + ", found " + val, Stack);
                }

                UpdateState(OpCodes.Ldflda, field, TypeOnStack.Get(field.FieldType.MakeByRefType()), pop: 1);
            }
            else
            {
                UpdateState(OpCodes.Ldsflda, field, TypeOnStack.Get(field.FieldType.MakeByRefType()));
            }
        }
    }
}
