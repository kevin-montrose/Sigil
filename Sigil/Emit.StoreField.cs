using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public void StoreField(FieldInfo field)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }

            if (!field.IsStatic)
            {
                var onStack = Stack.Top(2);

                if (onStack == null)
                {
                    throw new SigilException("StoreField expects two values on the stack for instance fields", Stack);
                }

                var type = onStack[1];
                var val = onStack[0];

                if (!field.DeclaringType.IsAssignableFrom(type))
                {
                    throw new SigilException("StoreField expected a type on the stack assignable to " + field.DeclaringType + ", found " + type, Stack);
                }

                if (!field.FieldType.IsAssignableFrom(val))
                {
                    throw new SigilException("StoreField expected a type on the stack assignable to " + field.FieldType + ", found " + val, Stack);
                }

                UpdateState(OpCodes.Stfld, field, pop: 2);
            }
            else
            {
                var onStack = Stack.Top();

                if (onStack == null)
                {
                    throw new SigilException("StoreField expected a value on the stack, but it was empty", Stack);
                }

                var val = onStack[0];

                if (!field.FieldType.IsAssignableFrom(val))
                {
                    throw new SigilException("StoreField expected a type on the stack assignable to " + field.FieldType + ", found " + val, Stack);
                }

                UpdateState(OpCodes.Stsfld, field, pop: 1);
            }
        }
    }
}
