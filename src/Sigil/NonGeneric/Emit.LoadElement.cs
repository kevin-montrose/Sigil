using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Expects a reference to an array and an index on the stack.</para>
        /// <para>Pops both, and pushes the element in the array at the index onto the stack.</para>
        /// </summary>
        public Emit LoadElement<ElementType>()
        {
            InnerEmit.LoadElement<ElementType>();
            return this;
        }

        /// <summary>
        /// <para>Expects a reference to an array and an index on the stack.</para>
        /// <para>Pops both, and pushes the element in the array at the index onto the stack.</para>
        /// </summary>
        public Emit LoadElement(Type elementType)
        {
            InnerEmit.LoadElement(elementType);
            return this;
        }
    }
}
