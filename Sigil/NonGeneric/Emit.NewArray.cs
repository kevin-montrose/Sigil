using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Pops a size from the stack, allocates a rank-1 array of the given type, and pushes a reference to the new array onto the stack.
        /// </summary>
        public Emit NewArray<ElementType>()
        {
            InnerEmit.NewArray<ElementType>();
            return this;
        }

        /// <summary>
        /// Pops a size from the stack, allocates a rank-1 array of the given type, and pushes a reference to the new array onto the stack.
        /// </summary>
        public Emit NewArray(Type elementType)
        {
            InnerEmit.NewArray(elementType);
            return this;
        }
    }
}
