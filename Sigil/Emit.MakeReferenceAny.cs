using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public Emit<DelegateType> MakeReferenceAny<Type>()
        {
            return MakeReferenceAny(typeof(Type));
        }

        public Emit<DelegateType> MakeReferenceAny(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            var transitions =
                new[] {
                    new StackTransition(new [] { typeof(NativeIntType) }, new [] { typeof(TypedReference) }),
                    new StackTransition(new[] { typeof(AnyPointerType) }, new [] { typeof(TypedReference) }),
                    new StackTransition(new [] { typeof(AnyByRefType) }, new [] { typeof(TypedReference) })
                };

            UpdateState(OpCodes.Mkrefany, type, transitions.Wrap("MakeReferenceAny"));

            return this;
        }
    }
}
