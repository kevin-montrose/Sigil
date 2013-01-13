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
        public void StoreLocal(EmitLocal local)
        {
            if (local == null)
            {
                throw new ArgumentNullException("local");
            }

            if (local.Owner != this)
            {
                throw new ArgumentException("local is not owned by this Emit, and thus cannot be used");
            }

            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("StoreLocal expects a value on the stack, but it's empty", Stack);
            }

            if (!local.LocalType.IsAssignableFrom(top[0]))
            {
                throw new SigilException("StoreLocal expects a value assignable to " + local.LocalType.FullName + " to be on the stack; found " + top[0], Stack);
            }

            UnusedLocals.Remove(local);

            switch (local.Index)
            {
                case 0: UpdateState(OpCodes.Stloc_0, pop: 1); return;
                case 1: UpdateState(OpCodes.Stloc_1, pop: 1); return;
                case 2: UpdateState(OpCodes.Stloc_2, pop: 1); return;
                case 3: UpdateState(OpCodes.Stloc_3, pop: 1); return;
            }

            if (local.Index >= byte.MinValue && local.Index <= byte.MaxValue)
            {
                UpdateState(OpCodes.Stloc_S, local.Index, pop: 1);
                return;
            }

            UpdateState(OpCodes.Stloc, local.Builder, pop: 1);
        }
    }
}
