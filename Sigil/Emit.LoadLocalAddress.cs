using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public void LoadLocalAddress(Local local)
        {
            if (local == null)
            {
                throw new ArgumentNullException("local");
            }

            if (local.Owner != this)
            {
                throw new ArgumentException("local is not owned by this Emit, and thus cannot be used");
            }

            UnusedLocals.Remove(local);

            var type = local.StackType.Type;
            if (local.StackType.IsPointer) type = type.MakePointerType();
            if (local.StackType.IsReference) type = type.MakeByRefType();

            var ptrType = type.MakePointerType();

            if (local.Index >= byte.MinValue && local.Index <= byte.MaxValue)
            {
                UpdateState(OpCodes.Ldloca_S, local.Index, TypeOnStack.Get(ptrType));
                return;
            }

            UpdateState(OpCodes.Ldloca, local.Index, TypeOnStack.Get(ptrType));
        }
    }
}
