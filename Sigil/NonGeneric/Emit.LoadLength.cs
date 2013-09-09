using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Pops a reference to a rank 1 array off the stack, and pushes it's length onto the stack.
        /// </summary>
        public Emit LoadLength<ElementType>()
        {
            InnerEmit.LoadLength<ElementType>();
            return this;
        }

        /// <summary>
        /// Pops a reference to a rank 1 array off the stack, and pushes it's length onto the stack.
        /// </summary>
        public Emit LoadLength(Type elementType)
        {
            InnerEmit.LoadLength(elementType);
            return this;
        }
    }
}
