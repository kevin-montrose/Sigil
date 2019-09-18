using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Pops a value of the given type and a pointer off the stack, and stores the value at the address in the pointer.
        /// </summary>
        public Emit StoreIndirect<Type>(bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.StoreIndirect<Type>(isVolatile, unaligned);
            return this;
        }

        /// <summary>
        /// Pops a value of the given type and a pointer off the stack, and stores the value at the address in the pointer.
        /// </summary>
        public Emit StoreIndirect(Type type, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.StoreIndirect(type, isVolatile, unaligned);
            return this;
        }
    }
}
