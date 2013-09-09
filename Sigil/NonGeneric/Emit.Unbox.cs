using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Pops a boxed value from the stack and pushes a pointer to it's unboxed value.
        /// 
        /// To load the value directly onto the stack, use UnboxAny().
        /// </summary>
        public Emit Unbox<ValueType>()
        {
            InnerEmit.Unbox<ValueType>();
            return this;
        }

        /// <summary>
        /// Pops a boxed value from the stack and pushes a pointer to it's unboxed value.
        /// 
        /// To load the value directly onto the stack, use UnboxAny().
        /// </summary>
        public Emit Unbox(Type valueType)
        {
            InnerEmit.Unbox(valueType);
            return this;
        }

        /// <summary>
        /// Pops a boxed value from the stack, unboxes it and pushes the value onto the stack.
        /// 
        /// To get an address for the unboxed value instead, use Unbox().
        /// </summary>
        public Emit UnboxAny<ValueType>()
        {
            InnerEmit.UnboxAny<ValueType>();
            return this;
        }

        /// <summary>
        /// Pops a boxed value from the stack, unboxes it and pushes the value onto the stack.
        /// 
        /// To get an address for the unboxed value instead, use Unbox().
        /// </summary>
        public Emit UnboxAny(Type valueType)
        {
            InnerEmit.UnboxAny(valueType);
            return this;
        }
    }
}
