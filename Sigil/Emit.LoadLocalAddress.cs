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
        /// Pushes a pointer to the given local onto the stack.
        /// 
        /// To create a local, use DeclareLocal.
        /// </summary>
        public Emit<DelegateType> LoadLocalAddress(Local local)
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

            var type = local.StackType.Type;
            if (local.StackType.IsPointer) type = type.MakePointerType();
            if (local.StackType.IsReference) type = type.MakeByRefType();

            var ptrType = type.MakePointerType();

            if (local.Index >= byte.MinValue && local.Index <= byte.MaxValue)
            {
                UpdateState(OpCodes.Ldloca_S, local.Index, TypeOnStack.Get(ptrType));
                return this;
            }

            UpdateState(OpCodes.Ldloca, local.Index, TypeOnStack.Get(ptrType));

            return this;
        }
    }
}
