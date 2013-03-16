using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public Emit<DelegateType> ReferenceAnyType()
        {
            var transitions =
                new[] {
                    new StackTransition(new[] { typeof(TypedReference) }, new [] { typeof(RuntimeTypeHandle) })
                };

            UpdateState(OpCodes.Refanytype, transitions.Wrap("ReferenceAnyType"));

            return this;
        }
    }
}
