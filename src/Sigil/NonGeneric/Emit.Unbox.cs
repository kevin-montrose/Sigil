using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Pops a boxed value from the stack and pushes a pointer to it's unboxed value.</para>
        /// <para>To load the value directly onto the stack, use UnboxAny().</para>
        /// </summary>
        public Emit Unbox<ValueType>()
        {
            InnerEmit.Unbox<ValueType>();
            return this;
        }

        /// <summary>
        /// <para>Pops a boxed value from the stack and pushes a pointer to it's unboxed value.</para>
        /// <para>To load the value directly onto the stack, use UnboxAny().</para>
        /// </summary>
        public Emit Unbox(Type valueType)
        {
            InnerEmit.Unbox(valueType);
            return this;
        }

        /// <summary>
        /// <para>Pops a boxed value from the stack, unboxes it and pushes the value onto the stack.</para>
        /// <para>To get an address for the unboxed value instead, use Unbox().</para>
        /// </summary>
        public Emit UnboxAny<ValueType>()
        {
            InnerEmit.UnboxAny<ValueType>();
            return this;
        }

        /// <summary>
        /// <para>Pops a boxed value from the stack, unboxes it and pushes the value onto the stack.</para>
        /// <para>To get an address for the unboxed value instead, use Unbox().</para>
        /// </summary>
        public Emit UnboxAny(Type valueType)
        {
            InnerEmit.UnboxAny(valueType);
            return this;
        }
    }
}
