#if !COREFX // see https://github.com/dotnet/corefx/issues/4543 item 4
using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Converts a pointer or reference to a value on the stack into a TypedReference of the given type.
        /// 
        /// TypedReferences can be used with ReferenceAnyType and ReferenceAnyValue to pass arbitrary types as parameters.
        /// </summary>
        public Emit<DelegateType> MakeReferenceAny<Type>()
        {
            return MakeReferenceAny(typeof(Type));
        }

        /// <summary>
        /// Converts a pointer or reference to a value on the stack into a TypedReference of the given type.
        /// 
        /// TypedReferences can be used with ReferenceAnyType and ReferenceAnyValue to pass arbitrary types as parameters.
        /// </summary>
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

            UpdateState(OpCodes.Mkrefany, type, Wrap(transitions, "MakeReferenceAny"));

            return this;
        }
    }
}
#endif
