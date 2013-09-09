using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Takes a destination pointer, a source pointer as arguments.  Pops both off the stack.
        /// 
        /// Copies the given value type from the source to the destination.
        /// </summary>
        public Emit CopyObject<ValueType>()
            where ValueType : struct
        {
            InnerEmit.CopyObject<ValueType>();
            return this;
        }

        /// <summary>
        /// Takes a destination pointer, a source pointer as arguments.  Pops both off the stack.
        /// 
        /// Copies the given value type from the source to the destination.
        /// </summary>
        public Emit CopyObject(Type valueType)
        {
            InnerEmit.CopyObject(valueType);
            return this;
        }
    }
}
