#if !COREFX // see https://github.com/dotnet/corefx/issues/4543 item 4
using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {

        /// <summary>
        /// Converts a pointer or reference to a value on the stack into a TypedReference of the given type.
        /// 
        /// TypedReferences can be used with ReferenceAnyType and ReferenceAnyValue to pass arbitrary types as parameters.
        /// </summary>
        public Emit MakeReferenceAny<Type>()
        {
            InnerEmit.MakeReferenceAny<Type>();
            return this;
        }

        /// <summary>
        /// Converts a pointer or reference to a value on the stack into a TypedReference of the given type.
        /// 
        /// TypedReferences can be used with ReferenceAnyType and ReferenceAnyValue to pass arbitrary types as parameters.
        /// </summary>
        public Emit MakeReferenceAny(Type type)
        {
            InnerEmit.MakeReferenceAny(type);
            return this;
        }
    }
}
#endif