using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Expects a reference to an array of the given element type and an index on the stack.</para>
        /// <para>Pops both, and pushes the address of the element at the given index.</para>
        /// </summary>
        public Emit LoadElementAddress<ElementType>()
        {
            InnerEmit.LoadElementAddress<ElementType>();
            return this;
        }

        /// <summary>
        /// <para>Expects a reference to an array of the given element type and an index on the stack.</para>
        /// <para>Pops both, and pushes the address of the element at the given index.</para>
        /// </summary>
        public Emit LoadElementAddress(Type elementType)
        {
            InnerEmit.LoadElementAddress(elementType);
            return this;
        }
    }
}
