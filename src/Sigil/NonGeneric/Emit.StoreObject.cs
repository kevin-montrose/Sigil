using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Pops a value type and a pointer off of the stack and copies the given value to the given address.</para>
        /// <para>For primitive and reference types use StoreIndirect.</para>
        /// </summary>
        public Emit StoreObject<ValueType>(bool isVolatile = false, int? unaligned = null)
            where ValueType : struct
        {
            InnerEmit.StoreObject<ValueType>(isVolatile, unaligned);
            return this;
        }

        /// <summary>
        /// <para>Pops a value type and a pointer off of the stack and copies the given value to the given address.</para>
        /// <para>For primitive and reference types use StoreIndirect.</para>
        /// </summary>
        public Emit StoreObject(Type valueType, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.StoreObject(valueType, isVolatile, unaligned);
            return this;
        }
    }
}
