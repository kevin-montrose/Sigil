using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Boxes the given value type on the stack, converting it into a reference.
        /// </summary>
        public Emit Box<ValueType>()
            where ValueType : struct
        {
            InnerEmit.Box<ValueType>();
            return this;
        }

        /// <summary>
        /// Boxes the given value type on the stack, converting it into a reference.
        /// </summary>
        public Emit Box(Type valueType)
        {
            InnerEmit.Box(valueType);
            return this;
        }
    }
}
