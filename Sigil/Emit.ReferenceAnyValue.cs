using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public Emit<DelegateType> ReferenceAnyValue<Type>()
        {
            return ReferenceAnyValue(typeof(Type));
        }

        public Emit<DelegateType> ReferenceAnyValue(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            var transitions =
                new[] 
                {
                    new StackTransition(new[] { typeof(TypedReference) }, new[] { type.MakeByRefType() })
                };

            UpdateState(OpCodes.Refanyval, type, transitions.Wrap("ReferenceAnyValue"));

            return this;
        }
    }
}
