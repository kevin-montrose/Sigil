using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Convert a value on the stack to the given non-character primitive type.
        /// 
        /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr). 
        /// </summary>
        public Emit Convert<PrimitiveType>()
            where PrimitiveType : struct
        {
            InnerEmit.Convert<PrimitiveType>();
            return this;
        }

        /// <summary>
        /// Convert a value on the stack to the given non-character primitive type.
        /// 
        /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr). 
        /// </summary>
        public Emit Convert(Type primitiveType)
        {
            InnerEmit.Convert(primitiveType);
            return this;
        }

        /// <summary>
        /// Convert a value on the stack to the given non-character, non-float, non-double primitive type.
        /// If the conversion would overflow at runtime, an OverflowException is thrown.
        /// 
        /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr). 
        /// </summary>
        public Emit ConvertOverflow<PrimitiveType>()
        {
            InnerEmit.ConvertOverflow<PrimitiveType>();
            return this;
        }

        /// <summary>
        /// Convert a value on the stack to the given non-character, non-float, non-double primitive type.
        /// If the conversion would overflow at runtime, an OverflowException is thrown.
        /// 
        /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr). 
        /// </summary>
        public Emit ConvertOverflow(Type primitiveType)
        {
            InnerEmit.ConvertOverflow(primitiveType);
            return this;
        }

        /// <summary>
        /// Convert a value on the stack to the given non-character, non-float, non-double primitive type as if it were unsigned.
        /// If the conversion would overflow at runtime, an OverflowException is thrown.
        /// 
        /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr). 
        /// </summary>
        public Emit UnsignedConvertOverflow<PrimitiveType>()
        {
            InnerEmit.UnsignedConvertOverflow<PrimitiveType>();
            return this;
        }

        /// <summary>
        /// Convert a value on the stack to the given non-character, non-float, non-double primitive type as if it were unsigned.
        /// If the conversion would overflow at runtime, an OverflowException is thrown.
        /// 
        /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr). 
        /// </summary>
        public Emit UnsignedConvertOverflow(Type primitiveType)
        {
            InnerEmit.UnsignedConvertOverflow(primitiveType);
            return this;
        }

        /// <summary>
        /// Converts a primitive type on the stack to a float, as if it were unsigned.
        /// 
        /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr).
        /// </summary>
        public Emit UnsignedConvertToFloat()
        {
            InnerEmit.UnsignedConvertToFloat();
            return this;
        }
    }
}
