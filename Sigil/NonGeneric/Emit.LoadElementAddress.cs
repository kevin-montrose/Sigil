using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Expects a reference to an array of the given element type and an index on the stack.
        /// 
        /// Pops both, and pushes the address of the element at the given index.
        /// </summary>
        public Emit LoadElementAddress<ElementType>()
        {
            InnerEmit.LoadElementAddress<ElementType>();
            return this;
        }

        /// <summary>
        /// Expects a reference to an array of the given element type and an index on the stack.
        /// 
        /// Pops both, and pushes the address of the element at the given index.
        /// </summary>
        public Emit LoadElementAddress(Type elementType)
        {
            InnerEmit.LoadElementAddress(elementType);
            return this;
        }
    }
}
