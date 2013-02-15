using Sigil.Impl;
using System;
using System.Linq;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        private void CheckConvertible(string method, TypeOnStack item)
        {
            if (item != TypeOnStack.Get<int>() && item != TypeOnStack.Get<NativeIntType>() &&
                item != TypeOnStack.Get<long>() && item != TypeOnStack.Get<float>() &&
                item != TypeOnStack.Get<double>() && !item.IsPointer
               )
            {
                throw new SigilVerificationException(method + " expected an int, native int, long, float, double, or pointer on the stack; found " + item, IL.Instructions(Locals));
            }
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

            if (!primitiveType.IsPrimitive || primitiveType == typeof(char))
            {
                throw new ArgumentException("Convert expects a non-character primitive type");
            }

            var top = Stack.Top();
            if (top == null)
            {
                FailStackUnderflow(1);
            }

            CheckConvertible("Convert", top.Single());

            if (primitiveType == typeof(byte))
            {
                ConvertToByte();
                return this;
            }

            if (primitiveType == typeof(sbyte))
            {
                ConvertToSByte();
                return this;
            }

            if (primitiveType == typeof(short))
            {
                ConvertToInt16();
                return this;
            }

            if (primitiveType == typeof(ushort))
            {
                ConvertToUInt16();
                return this;
            }

            if (primitiveType == typeof(int))
            {
                ConvertToInt32();
                return this;
            }

            if (primitiveType == typeof(uint))
            {
                ConvertToUInt32();
                return this;
            }

            if (primitiveType == typeof(long))
            {
                ConvertToInt64();
                return this;
            }

            if (primitiveType == typeof(ulong))
            {
                ConvertToUInt64();
                return this;
            }

            if (primitiveType == typeof(IntPtr))
            {
                ConvertToNativeInt();
                return this;
            }

            if (primitiveType == typeof(UIntPtr))
            {
                ConvertToUnsignedNativeInt();
                return this;
            }

            if (primitiveType == typeof(float))
            {
                ConvertToFloat();
                return this;
            }

            if (primitiveType == typeof(double))
            {
                ConvertToDouble();
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

            if (!primitiveType.IsPrimitive || primitiveType == typeof(char))
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

            var top = Stack.Top();
            if (top == null)
            {
                FailStackUnderflow(1);
            }

            CheckConvertible("ConvertOverflow", top.Single());

            if (primitiveType == typeof(byte))
            {
                ConvertToByteOverflow();
                return this;
            }

            if (primitiveType == typeof(sbyte))
            {
                ConvertToSByteOverflow();
                return this;
            }

            if (primitiveType == typeof(short))
            {
                ConvertToInt16Overflow();
                return this;
            }

            if (primitiveType == typeof(ushort))
            {
                ConvertToUInt16Overflow();
                return this;
            }

            if (primitiveType == typeof(int))
            {
                ConvertToInt32Overflow();
                return this;
            }

            if (primitiveType == typeof(uint))
            {
                ConvertToUInt32Overflow();
                return this;
            }

            if (primitiveType == typeof(long))
            {
                ConvertToInt64Overflow();
                return this;
            }

            if (primitiveType == typeof(ulong))
            {
                ConvertToUInt64Overflow();
                return this;
            }

            if (primitiveType == typeof(IntPtr))
            {
                ConvertToNativeIntOverflow();
                return this;
            }

            if (primitiveType == typeof(UIntPtr))
            {
                ConvertToUnsignedNativeIntOverflow();
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

            if (!primitiveType.IsPrimitive || primitiveType == typeof(char))
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

            if (primitiveType == typeof(UIntPtr) || primitiveType == typeof(IntPtr))
            {
                throw new InvalidOperationException("There is no operation for converting to a pointer with overflow checking");
            }

            var top = Stack.Top();
            if (top == null)
            {
                FailStackUnderflow(1);
            }

            CheckConvertible("UnsignedConvertOverflow", top.Single());

            if (primitiveType == typeof(byte))
            {
                UnsignedConvertToByteOverflow();
                return this;
            }

            if (primitiveType == typeof(sbyte))
            {
                UnsignedConvertToSByteOverflow();
                return this;
            }

            if (primitiveType == typeof(short))
            {
                UnsignedConvertToInt16Overflow();
                return this;
            }

            if (primitiveType == typeof(ushort))
            {
                UnsignedConvertToUInt16Overflow();
                return this;
            }

            if (primitiveType == typeof(int))
            {
                UnsignedConvertToInt32Overflow();
                return this;
            }

            if (primitiveType == typeof(uint))
            {
                UnsignedConvertToUInt32Overflow();
                return this;
            }

            if (primitiveType == typeof(long))
            {
                UnsignedConvertToInt64Overflow();
                return this;
            }

            if (primitiveType == typeof(ulong))
            {
                UnsignedConvertToUInt64Overflow();
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
            var top = Stack.Top();

            if (top == null)
            {
                FailStackUnderflow(1);
            }

            CheckConvertible("UnsignedConvertToFloat", top.Single());

            UpdateState(OpCodes.Conv_R_Un, TypeOnStack.Get<float>(), pop: 1);

            return this;
        }

        private void ConvertToNativeInt()
        {
            UpdateState(OpCodes.Conv_I, TypeOnStack.Get<NativeIntType>(), pop: 1);
        }

        private void ConvertToNativeIntOverflow()
        {
            UpdateState(OpCodes.Conv_Ovf_I, TypeOnStack.Get<NativeIntType>(), pop: 1);
        }

        private void UnsignedConvertToNativeIntOverflow()
        {
            UpdateState(OpCodes.Conv_Ovf_I_Un, TypeOnStack.Get<NativeIntType>(), pop: 1);
        }

        private void ConvertToUnsignedNativeInt()
        {
            UpdateState(OpCodes.Conv_U, TypeOnStack.Get<NativeIntType>(), pop: 1);
        }

        private void ConvertToUnsignedNativeIntOverflow()
        {
            UpdateState(OpCodes.Conv_Ovf_U, TypeOnStack.Get<NativeIntType>(), pop: 1);
        }

        private void UnsignedConvertToUnsignedNativeIntOverflow()
        {
            UpdateState(OpCodes.Conv_Ovf_U_Un, TypeOnStack.Get<NativeIntType>(), pop: 1);
        }

        private void ConvertToSByte()
        {
            UpdateState(OpCodes.Conv_I1, TypeOnStack.Get<int>(), pop: 1);
        }

        private void ConvertToSByteOverflow()
        {
            UpdateState(OpCodes.Conv_Ovf_I1, TypeOnStack.Get<int>(), pop: 1);
        }

        private void UnsignedConvertToSByteOverflow()
        {
            UpdateState(OpCodes.Conv_Ovf_I1_Un, TypeOnStack.Get<int>(), pop: 1);
        }

        private void ConvertToInt16()
        {
            UpdateState(OpCodes.Conv_I2, TypeOnStack.Get<int>(), pop: 1);
        }

        private void ConvertToInt16Overflow()
        {
            UpdateState(OpCodes.Conv_Ovf_I2, TypeOnStack.Get<int>(), pop: 1);
        }

        private void UnsignedConvertToInt16Overflow()
        {
            UpdateState(OpCodes.Conv_Ovf_I2_Un, TypeOnStack.Get<int>(), pop: 1);
        }

        private void ConvertToInt32()
        {
            UpdateState(OpCodes.Conv_I4, TypeOnStack.Get<int>(), pop: 1);
        }

        private void ConvertToInt32Overflow()
        {
            UpdateState(OpCodes.Conv_Ovf_I4, TypeOnStack.Get<int>(), pop: 1);
        }

        private void UnsignedConvertToInt32Overflow()
        {
            UpdateState(OpCodes.Conv_Ovf_I4_Un, TypeOnStack.Get<int>(), pop: 1);
        }

        private void ConvertToInt64()
        {
            UpdateState(OpCodes.Conv_I8, TypeOnStack.Get<long>(), pop: 1);
        }

        private void ConvertToInt64Overflow()
        {
            UpdateState(OpCodes.Conv_Ovf_I8, TypeOnStack.Get<long>(), pop: 1);
        }

        private void UnsignedConvertToInt64Overflow()
        {
            UpdateState(OpCodes.Conv_Ovf_I8_Un, TypeOnStack.Get<long>(), pop: 1);
        }

        private void ConvertToFloat()
        {
            UpdateState(OpCodes.Conv_R4, TypeOnStack.Get<float>(), pop: 1);
        }

        private void ConvertToDouble()
        {
            UpdateState(OpCodes.Conv_R8, TypeOnStack.Get<double>(), pop: 1);
        }

        private void ConvertToByte()
        {
            UpdateState(OpCodes.Conv_U1, TypeOnStack.Get<int>(), pop: 1);
        }

        private void ConvertToByteOverflow()
        {
            UpdateState(OpCodes.Conv_Ovf_U1, TypeOnStack.Get<int>(), pop: 1);
        }

        private void UnsignedConvertToByteOverflow()
        {
            UpdateState(OpCodes.Conv_Ovf_U1_Un, TypeOnStack.Get<int>(), pop: 1);
        }

        private void ConvertToUInt16()
        {
            UpdateState(OpCodes.Conv_U2, TypeOnStack.Get<int>(), pop: 1);
        }

        private void ConvertToUInt16Overflow()
        {
            UpdateState(OpCodes.Conv_Ovf_U2, TypeOnStack.Get<int>(), pop: 1);
        }

        private void UnsignedConvertToUInt16Overflow()
        {
            UpdateState(OpCodes.Conv_Ovf_U2_Un, TypeOnStack.Get<int>(), pop: 1);
        }

        private void ConvertToUInt32()
        {
            UpdateState(OpCodes.Conv_U4, TypeOnStack.Get<int>(), pop: 1);
        }

        private void ConvertToUInt32Overflow()
        {
            UpdateState(OpCodes.Conv_Ovf_U4, TypeOnStack.Get<int>(), pop: 1);
        }

        private void UnsignedConvertToUInt32Overflow()
        {
            UpdateState(OpCodes.Conv_Ovf_U4_Un, TypeOnStack.Get<int>(), pop: 1);
        }

        private void ConvertToUInt64()
        {
            UpdateState(OpCodes.Conv_U8, TypeOnStack.Get<long>(), pop: 1);
        }

        private void ConvertToUInt64Overflow()
        {
            UpdateState(OpCodes.Conv_Ovf_U8, TypeOnStack.Get<long>(), pop: 1);
        }

        private void UnsignedConvertToUInt64Overflow()
        {
            UpdateState(OpCodes.Conv_Ovf_U8_Un, TypeOnStack.Get<long>(), pop: 1);
        }
    }
}
