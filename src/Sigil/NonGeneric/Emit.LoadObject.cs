using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Pops a pointer from the stack, and pushes the given value type it points to onto the stack.</para>
        /// <para>For primitive and reference types, use LoadIndirect().</para>
        /// </summary>
        public Emit LoadObject<ValueType>(bool isVolatile = false, int? unaligned = null)
            where ValueType : struct
        {
            InnerEmit.LoadObject<ValueType>(isVolatile, unaligned);
            return this;
        }

        /// <summary>
        /// <para>Pops a pointer from the stack, and pushes the given value type it points to onto the stack.</para>
        /// <para>For primitive and reference types, use LoadIndirect().</para>
        /// </summary>
        public Emit LoadObject(Type valueType, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.LoadObject(valueType, isVolatile, unaligned);
            return this;
        }
    }
}
