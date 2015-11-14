using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        private TransitionWrapper CheckConvertible(string method, Type toType)
        {
            return
                Wrap(
                    new[]
                    {
                        new StackTransition(new [] { typeof(int) }, new [] { toType }),
                        new StackTransition(new [] { typeof(NativeIntType) }, new [] { toType }),
                        new StackTransition(new [] { typeof(long) }, new [] { toType }),
                        new StackTransition(new [] { typeof(float) }, new [] { toType }),
                        new StackTransition(new [] { typeof(double) }, new [] { toType }),
                        new StackTransition(new [] { typeof(AnyPointerType) }, new [] { toType }),
                        new StackTransition(new [] { typeof(AnyPointerType) }, new [] { toType }),
                        new StackTransition(new [] { typeof(AnyPointerType) }, new [] { toType }),
                        new StackTransition(new [] { typeof(AnyPointerType) }, new [] { toType }),
                        new StackTransition(new [] { typeof(AnyPointerType) }, new [] { toType }),
                        new StackTransition(new [] { typeof(AnyByRefType) }, new [] { toType })
                    
                    },
                    method
                );
        }

        /// <summary>
        /// Convert a value on the stack to the given non-character primitive type.
        /// 
        /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr). 
        /// </summary>
        public Emit<DelegateType> Convert<PrimitiveType>()
            where PrimitiveType : struct
        {
            return Convert(typeof(PrimitiveType));
        }

        /// <summary>
        /// Convert a value on the stack to the given non-character primitive type.
        /// 
        /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr). 
        /// </summary>
        public Emit<DelegateType> Convert(Type primitiveType)
        {
            if (primitiveType == null)
            {
                throw new ArgumentNullException("primitiveType");
            }

            if (!TypeHelpers.IsPrimitive(primitiveType) || primitiveType == typeof(char))
            {
                throw new ArgumentException("Convert expects a non-character primitive type");
            }

            var transitions = CheckConvertible("Convert", primitiveType);

            if (primitiveType == typeof(byte))
            {
                ConvertToByte(transitions);
                return this;
            }

            if (primitiveType == typeof(sbyte) || primitiveType == typeof(bool)) // bool is an int8 on the stack
            {
                ConvertToSByte(transitions);
                return this;
            }

            if (primitiveType == typeof(short))
            {
                ConvertToInt16(transitions);
                return this;
            }

            if (primitiveType == typeof(ushort))
            {
                ConvertToUInt16(transitions);
                return this;
            }

            if (primitiveType == typeof(int))
            {
                ConvertToInt32(transitions);
                return this;
            }

            if (primitiveType == typeof(uint))
            {
                ConvertToUInt32(transitions);
                return this;
            }

            if (primitiveType == typeof(long))
            {
                ConvertToInt64(transitions);
                return this;
            }

            if (primitiveType == typeof(ulong))
            {
                ConvertToUInt64(transitions);
                return this;
            }

            if (primitiveType == typeof(IntPtr))
            {
                ConvertToNativeInt(transitions);
                return this;
            }

            if (primitiveType == typeof(UIntPtr))
            {
                ConvertToUnsignedNativeInt(transitions);
                return this;
            }

            if (primitiveType == typeof(float))
            {
                ConvertToFloat(transitions);
                return this;
            }

            if (primitiveType == typeof(double))
            {
                ConvertToDouble(transitions);
                return this;
            }

            throw new Exception("Shouldn't be possible");
        }

        /// <summary>
        /// Convert a value on the stack to the given non-character, non-float, non-double primitive type.
        /// If the conversion would overflow at runtime, an OverflowException is thrown.
        /// 
        /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr). 
        /// </summary>
        public Emit<DelegateType> ConvertOverflow<PrimitiveType>()
        {
            return ConvertOverflow(typeof(PrimitiveType));
        }

        /// <summary>
        /// Convert a value on the stack to the given non-character, non-float, non-double primitive type.
        /// If the conversion would overflow at runtime, an OverflowException is thrown.
        /// 
        /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr). 
        /// </summary>
        public Emit<DelegateType> ConvertOverflow(Type primitiveType)
        {
            if (primitiveType == null)
            {
                throw new ArgumentNullException("primitiveType");
            }

            if (!TypeHelpers.IsPrimitive(primitiveType) || primitiveType == typeof(char))
            {
                throw new ArgumentException("ConvertOverflow expects a non-character primitive type");
            }

            if (primitiveType == typeof(float))
            {
                throw new InvalidOperationException("There is no operation for converting to a float with overflow checking");
            }

            if (primitiveType == typeof(double))
            {
                throw new InvalidOperationException("There is no operation for converting to a double with overflow checking");
            }

            var transitions = CheckConvertible("ConvertOverflow", primitiveType);

            if (primitiveType == typeof(byte))
            {
                ConvertToByteOverflow(transitions);
                return this;
            }

            if (primitiveType == typeof(sbyte) || primitiveType == typeof(bool)) // bool is an int8 on the stack
            {
                ConvertToSByteOverflow(transitions);
                return this;
            }

            if (primitiveType == typeof(short))
            {
                ConvertToInt16Overflow(transitions);
                return this;
            }

            if (primitiveType == typeof(ushort))
            {
                ConvertToUInt16Overflow(transitions);
                return this;
            }

            if (primitiveType == typeof(int))
            {
                ConvertToInt32Overflow(transitions);
                return this;
            }

            if (primitiveType == typeof(uint))
            {
                ConvertToUInt32Overflow(transitions);
                return this;
            }

            if (primitiveType == typeof(long))
            {
                ConvertToInt64Overflow(transitions);
                return this;
            }

            if (primitiveType == typeof(ulong))
            {
                ConvertToUInt64Overflow(transitions);
                return this;
            }

            if (primitiveType == typeof(IntPtr))
            {
                ConvertToNativeIntOverflow(transitions);
                return this;
            }

            if (primitiveType == typeof(UIntPtr))
            {
                ConvertToUnsignedNativeIntOverflow(transitions);
                return this;
            }

            throw new Exception("Shouldn't be possible");
        }

        /// <summary>
        /// Convert a value on the stack to the given non-character, non-float, non-double primitive type as if it were unsigned.
        /// If the conversion would overflow at runtime, an OverflowException is thrown.
        /// 
        /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr). 
        /// </summary>
        public Emit<DelegateType> UnsignedConvertOverflow<PrimitiveType>()
        {
            return UnsignedConvertOverflow(typeof(PrimitiveType));
        }

        /// <summary>
        /// Convert a value on the stack to the given non-character, non-float, non-double primitive type as if it were unsigned.
        /// If the conversion would overflow at runtime, an OverflowException is thrown.
        /// 
        /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr). 
        /// </summary>
        public Emit<DelegateType> UnsignedConvertOverflow(Type primitiveType)
        {
            if (primitiveType == null)
            {
                throw new ArgumentNullException("primitiveType");
            }

            if (!TypeHelpers.IsPrimitive(primitiveType) || primitiveType == typeof(char))
            {
                throw new ArgumentException("UnsignedConvertOverflow expects a non-character primitive type");
            }

            if (primitiveType == typeof(float))
            {
                throw new InvalidOperationException("There is no operation for converting to a float with overflow checking");
            }

            if (primitiveType == typeof(double))
            {
                throw new InvalidOperationException("There is no operation for converting to a double with overflow checking");
            }

            var transitions = CheckConvertible("UnsignedConvertOverflow", primitiveType);

            if (primitiveType == typeof(byte))
            {
                UnsignedConvertToByteOverflow(transitions);
                return this;
            }

            if (primitiveType == typeof(sbyte))
            {
                UnsignedConvertToSByteOverflow(transitions);
                return this;
            }

            if (primitiveType == typeof(short))
            {
                UnsignedConvertToInt16Overflow(transitions);
                return this;
            }

            if (primitiveType == typeof(ushort))
            {
                UnsignedConvertToUInt16Overflow(transitions);
                return this;
            }

            if (primitiveType == typeof(int))
            {
                UnsignedConvertToInt32Overflow(transitions);
                return this;
            }

            if (primitiveType == typeof(uint))
            {
                UnsignedConvertToUInt32Overflow(transitions);
                return this;
            }

            if (primitiveType == typeof(long))
            {
                UnsignedConvertToInt64Overflow(transitions);
                return this;
            }

            if (primitiveType == typeof(ulong))
            {
                UnsignedConvertToUInt64Overflow(transitions);
                return this;
            }

            if (primitiveType == typeof(IntPtr))
            {
                UnsignedConvertToNativeIntOverflow(transitions);
                return this;
            }

            if (primitiveType == typeof(UIntPtr))
            {
                UnsignedConvertToUnsignedNativeIntOverflow(transitions);
                return this;
            }

            throw new Exception("Shouldn't be possible");
        }

        /// <summary>
        /// Converts a primitive type on the stack to a float, as if it were unsigned.
        /// 
        /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr).
        /// </summary>
        public Emit<DelegateType> UnsignedConvertToFloat()
        {
            var transitions = CheckConvertible("UnsignedConvertToFloat", typeof(float));

            UpdateState(OpCodes.Conv_R_Un, transitions);

            return this;
        }

        private void ConvertToNativeInt(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_I, transitions);
        }

        private void ConvertToNativeIntOverflow(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_Ovf_I, transitions);
        }

        private void UnsignedConvertToNativeIntOverflow(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_Ovf_I_Un, transitions);
        }

        private void UnsignedConvertToUnsignedNativeIntOverflow(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_Ovf_U_Un, transitions);
        }

        private void ConvertToUnsignedNativeInt(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_U, transitions);
        }

        private void ConvertToUnsignedNativeIntOverflow(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_Ovf_U, transitions);
        }

        private void ConvertToSByte(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_I1, transitions);
        }

        private void ConvertToSByteOverflow(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_Ovf_I1, transitions);
        }

        private void UnsignedConvertToSByteOverflow(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_Ovf_I1_Un, transitions);
        }

        private void ConvertToInt16(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_I2, transitions);
        }

        private void ConvertToInt16Overflow(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_Ovf_I2, transitions);
        }

        private void UnsignedConvertToInt16Overflow(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_Ovf_I2_Un, transitions);
        }

        private void ConvertToInt32(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_I4, transitions);
        }

        private void ConvertToInt32Overflow(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_Ovf_I4, transitions);
        }

        private void UnsignedConvertToInt32Overflow(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_Ovf_I4_Un, transitions);
        }

        private void ConvertToInt64(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_I8, transitions);
        }

        private void ConvertToInt64Overflow(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_Ovf_I8, transitions);
        }

        private void UnsignedConvertToInt64Overflow(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_Ovf_I8_Un, transitions);
        }

        private void ConvertToFloat(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_R4, transitions);
        }

        private void ConvertToDouble(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_R8, transitions);
        }

        private void ConvertToByte(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_U1, transitions);
        }

        private void ConvertToByteOverflow(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_Ovf_U1, transitions);
        }

        private void UnsignedConvertToByteOverflow(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_Ovf_U1_Un, transitions);
        }

        private void ConvertToUInt16(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_U2, transitions);
        }

        private void ConvertToUInt16Overflow(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_Ovf_U2, transitions);
        }

        private void UnsignedConvertToUInt16Overflow(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_Ovf_U2_Un, transitions);
        }

        private void ConvertToUInt32(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_U4, transitions);
        }

        private void ConvertToUInt32Overflow(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_Ovf_U4, transitions);
        }

        private void UnsignedConvertToUInt32Overflow(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_Ovf_U4_Un, transitions);
        }

        private void ConvertToUInt64(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_U8, transitions);
        }

        private void ConvertToUInt64Overflow(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_Ovf_U8, transitions);
        }

        private void UnsignedConvertToUInt64Overflow(TransitionWrapper transitions)
        {
            UpdateState(OpCodes.Conv_Ovf_U8_Un, transitions);
        }
    }
}
