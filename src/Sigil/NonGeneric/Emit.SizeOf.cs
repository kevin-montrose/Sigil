using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Pushes the size of the given value type onto the stack.
        /// </summary>
        public Emit SizeOf<ValueType>()
            where ValueType : struct
        {
            InnerEmit.SizeOf<ValueType>();
            return this;
        }

        /// <summary>
        /// Pushes the size of the given value type onto the stack.
        /// </summary>
        public Emit SizeOf(Type valueType)
        {
            InnerEmit.SizeOf(valueType);
            return this;
        }
    }
}
