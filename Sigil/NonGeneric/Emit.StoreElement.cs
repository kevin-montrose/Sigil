using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Pops a value, an index, and a reference to an array off the stack.  Places the given value into the given array at the given index.
        /// </summary>
        public Emit StoreElement<ElementType>()
        {
            InnerEmit.StoreElement<ElementType>();
            return this;
        }

        /// <summary>
        /// Pops a value, an index, and a reference to an array off the stack.  Places the given value into the given array at the given index.
        /// </summary>
        public Emit StoreElement(Type elementType)
        {
            InnerEmit.StoreElement(elementType);
            return this;
        }
    }
}
