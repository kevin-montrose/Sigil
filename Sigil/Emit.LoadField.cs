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
        public void LoadField(FieldInfo field)
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
                    throw new SigilException("LoadField expects a value on the stack for instance fields", Stack);
                }
                var type = onStack[0];

                if (!field.DeclaringType.IsAssignableFrom(type))
                {
                    throw new SigilException("LoadField expected a type on the stack assignable to " + field.DeclaringType + ", found " + type, Stack);
                }

                UpdateState(OpCodes.Ldfld, field, TypeOnStack.Get(field.FieldType), pop: 1);
            }
            else
            {
                UpdateState(OpCodes.Ldsfld, field, TypeOnStack.Get(field.FieldType));
            }
        }
    }
}
