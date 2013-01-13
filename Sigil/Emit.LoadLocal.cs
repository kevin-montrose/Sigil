using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public void LoadLocal(EmitLocal local)
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

            switch (local.Index)
            {
                case 0: UpdateState(OpCodes.Ldloc_0, local.StackType); return;
                case 1: UpdateState(OpCodes.Ldloc_1, local.StackType); return;
                case 2: UpdateState(OpCodes.Ldloc_2, local.StackType); return;
                case 3: UpdateState(OpCodes.Ldloc_3, local.StackType); return;
            }

            if (local.Index >= byte.MinValue && local.Index <= byte.MaxValue)
            {
                UpdateState(OpCodes.Ldloc_S, local.Index, local.StackType);
                return;
            }

            UpdateState(OpCodes.Ldloc, local.Builder, local.StackType);
        }
    }
}
