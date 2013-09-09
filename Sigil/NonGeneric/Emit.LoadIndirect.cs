using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Pops a pointer from the stack and pushes the value (of the given type) at that address onto the stack.
        /// </summary>
        public Emit LoadIndirect<Type>(bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.LoadIndirect<Type>(isVolatile, unaligned);
            return this;
        }

        /// <summary>
        /// Pops a pointer from the stack and pushes the value (of the given type) at that address onto the stack.
        /// </summary>
        public Emit LoadIndirect(Type type, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.LoadIndirect(type, isVolatile, unaligned);
            return this;
        }
    }
}
