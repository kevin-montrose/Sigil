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
        /// <summary>
        /// Pops a value off the stack and stores it into the given local.
        /// 
        /// To create a local, use DeclareLocal().
        /// </summary>
        public Emit<DelegateType> StoreLocal(Local local)
        {
            if (local == null)
            {
                throw new ArgumentNullException("local");
            }

            if (((IOwned)local).Owner != this)
            {
                FailOwnership(local);
            }

            var top = Stack.Top();

            if (top == null)
            {
                FailStackUnderflow(1);
            }

            if (!local.LocalType.IsAssignableFrom(top[0]))
            {
                throw new SigilVerificationException("StoreLocal expects a value assignable to " + local.LocalType.FullName + " to be on the stack; found " + top[0], IL, Stack, 0);
            }

            UnusedLocals.Remove(local);

            switch (local.Index)
            {
                case 0: UpdateState(OpCodes.Stloc_0, pop: 1); return this;
                case 1: UpdateState(OpCodes.Stloc_1, pop: 1); return this;
                case 2: UpdateState(OpCodes.Stloc_2, pop: 1); return this;
                case 3: UpdateState(OpCodes.Stloc_3, pop: 1); return this;
            }

            if (local.Index >= byte.MinValue && local.Index <= byte.MaxValue)
            {
                UpdateState(OpCodes.Stloc_S, local.Index, pop: 1);
                return this;
            }

            UpdateState(OpCodes.Stloc, local, pop: 1);

            return this;
        }
    }
}
