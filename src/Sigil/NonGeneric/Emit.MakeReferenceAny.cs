using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Converts a pointer or reference to a value on the stack into a TypedReference of the given type.</para>
        /// <para>TypedReferences can be used with ReferenceAnyType and ReferenceAnyValue to pass arbitrary types as parameters.</para>
        /// </summary>
        public Emit MakeReferenceAny<Type>()
        {
            InnerEmit.MakeReferenceAny<Type>();
            return this;
        }

        /// <summary>
        /// <para>Converts a pointer or reference to a value on the stack into a TypedReference of the given type.</para>
        /// <para>TypedReferences can be used with ReferenceAnyType and ReferenceAnyValue to pass arbitrary types as parameters.</para>
        /// </summary>
        public Emit MakeReferenceAny(Type type)
        {
            InnerEmit.MakeReferenceAny(type);
            return this;
        }
    }
}