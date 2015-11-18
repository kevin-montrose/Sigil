#if !COREFX // see https://github.com/dotnet/corefx/issues/4543 item 4
using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {

        /// <summary>
        /// Converts a TypedReference on the stack into a reference to the contained object, given the type contained in the TypedReference.
        /// 
        /// __makeref(int) on the stack would become an int&amp;, for example.
        /// </summary>
        public Emit ReferenceAnyValue<Type>()
        {
            InnerEmit.ReferenceAnyValue<Type>();
            return this;
        }

        /// <summary>
        /// Converts a TypedReference on the stack into a reference to the contained object, given the type contained in the TypedReference.
        /// 
        /// __makeref(int) on the stack would become an int&amp;, for example.
        /// </summary>
        public Emit ReferenceAnyValue(Type type)
        {
            InnerEmit.ReferenceAnyValue(type);
            return this;
        }
    }
}
#endif