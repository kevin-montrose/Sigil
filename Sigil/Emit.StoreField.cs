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

                if (isVolatile)
                {
                    UpdateState(OpCodes.Volatile);
                }

                if (unaligned.HasValue)
                {
                    UpdateState(OpCodes.Unaligned, unaligned.Value);
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

                if (isVolatile)
                {
                    UpdateState(OpCodes.Volatile);
                }

                UpdateState(OpCodes.Stsfld, field, pop: 1);
            }

            return this;
        }
    }
}
