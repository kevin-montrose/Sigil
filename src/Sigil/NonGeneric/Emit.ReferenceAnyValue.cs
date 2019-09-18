using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {

        /// <summary>
        /// <para>Converts a TypedReference on the stack into a reference to the contained object, given the type contained in the TypedReference.</para>
        /// <para>__makeref(int) on the stack would become an int&amp;, for example.</para>
        /// </summary>
        public Emit ReferenceAnyValue<Type>()
        {
            InnerEmit.ReferenceAnyValue<Type>();
            return this;
        }

        /// <summary>
        /// <para>Converts a TypedReference on the stack into a reference to the contained object, given the type contained in the TypedReference.</para>
        /// <para>__makeref(int) on the stack would become an int&amp;, for example.</para>
        /// </summary>
        public Emit ReferenceAnyValue(Type type)
        {
            InnerEmit.ReferenceAnyValue(type);
            return this;
        }
    }
}