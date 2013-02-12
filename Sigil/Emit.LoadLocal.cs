using Sigil.Impl;
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
        /// <summary>
        /// Loads the value in the given local onto the stack.
        /// 
        /// To create a local, use DeclareLocal().
        /// </summary>
        public Emit<DelegateType> LoadLocal(Local local)
        {
            if (local == null)
            {
                throw new ArgumentNullException("local");
            }

            if (((IOwned)local).Owner != this)
            {
                FailOwnership(local);
            }

            UnusedLocals.Remove(local);

            switch (local.Index)
            {
                case 0: UpdateState(OpCodes.Ldloc_0, local.StackType); return this;
                case 1: UpdateState(OpCodes.Ldloc_1, local.StackType); return this;
                case 2: UpdateState(OpCodes.Ldloc_2, local.StackType); return this;
                case 3: UpdateState(OpCodes.Ldloc_3, local.StackType); return this;
            }

            if (local.Index >= byte.MinValue && local.Index <= byte.MaxValue)
            {
                UpdateState(OpCodes.Ldloc_S, (byte)local.Index, local.StackType);
                return this;
            }

            UpdateState(OpCodes.Ldloc, local, local.StackType);

            return this;
        }
    }
}
