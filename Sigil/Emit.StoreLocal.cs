using Sigil.Impl;
using System;
using System.Reflection.Emit;

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
                throw new SigilVerificationException("StoreLocal expects a value assignable to " + local.LocalType.FullName + " to be on the stack; found " + top[0], IL.Instructions(LocalsByIndex), Stack, 0);
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
                byte asByte;
                unchecked
                {
                    asByte = (byte)local.Index;
                }

                UpdateState(OpCodes.Stloc_S, asByte, pop: 1);
                return this;
            }

            UpdateState(OpCodes.Stloc, local, pop: 1);

            return this;
        }

        /// <summary>
        /// Pops a value off the stack and stores it in the local with the given name.
        /// </summary>
        public Emit<DelegateType> StoreLocal(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            return StoreLocal(Locals[name]);
        } 
    }
}
