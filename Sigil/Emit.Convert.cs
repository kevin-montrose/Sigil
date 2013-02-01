using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Convert a value on the stack to the given non-character primitive type.
        /// 
        /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr). 
        /// </summary>
        public void Convert<PrimitiveType>()
        {
            Convert(typeof(PrimitiveType));
        }

        /// <summary>
        /// Convert a value on the stack to the given non-character primitive type.
        /// 
        /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr). 
        /// </summary>
        public void Convert(Type primitiveType)
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
                throw new SigilException("Convert expected a value on the stack, but it was empty", Stack);
            }

            if (primitiveType == typeof(byte))
            {
                ConvertToByte();
                return;
            }

            if (primitiveType == typeof(sbyte))
            {
                ConvertToSByte();
                return;
            }

            if (primitiveType == typeof(short))
            {
                ConvertToInt16();
                return;
            }

            if (primitiveType == typeof(ushort))
            {
                ConvertToUInt16();
                return;
            }

            if (primitiveType == typeof(int))
            {
                ConvertToInt32();
                return;
            }

            if (primitiveType == typeof(uint))
            {
                ConvertToUInt32();
                return;
            }

            if (primitiveType == typeof(long))
            {
                ConvertToInt64();
                return;
            }

            if (primitiveType == typeof(ulong))
            {
                ConvertToUInt64();
                return;
            }

            if (primitiveType == typeof(IntPtr))
            {
                ConvertToNativeInt();
                return;
            }

            if (primitiveType == typeof(UIntPtr))
            {
                ConvertToUnsignedNativeInt();
                return;
            }

            if (primitiveType == typeof(float))
            {
                ConvertToFloat();
                return;
            }

            if (primitiveType == typeof(double))
            {
                ConvertToDouble();
                return;
            }
        }

        /// <summary>
        /// Convert a value on the stack to the given non-character primitive type.
        /// If the conversion would overflow at runtime, an OverflowException is thrown.
        /// 
        /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr). 
        /// </summary>
        public void ConvertOverflow<PrimitiveType>()
        {
            ConvertOverflow(typeof(PrimitiveType));
        }

        /// <summary>
        /// Convert a value on the stack to the given non-character primitive type.
        /// If the conversion would overflow at runtime, an OverflowException is thrown.
        /// 
        /// Primitives are int8, uint8, int16, uint16, int32, uint32, int64, uint64, float, double, native int (IntPtr), and unsigned native int (UIntPtr). 
        /// </summary>
        public void ConvertOverflow(Type primitiveType)
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
                throw new SigilException("ConvertOverflow expected a value on the stack, but it was empty", Stack);
            }

            if (primitiveType == typeof(byte))
            {
                ConvertToByteOverflow();
                return;
            }

            if (primitiveType == typeof(sbyte))
            {
                ConvertToSByteOverflow();
                return;
            }

            if (primitiveType == typeof(short))
            {
                ConvertToInt16Overflow();
                return;
            }

            if (primitiveType == typeof(ushort))
            {
                ConvertToUInt16Overflow();
                return;
            }

            if (primitiveType == typeof(int))
            {
                ConvertToInt32Overflow();
                return;
            }

            if (primitiveType == typeof(uint))
            {
                ConvertToUInt32Overflow();
                return;
            }

            if (primitiveType == typeof(long))
            {
                ConvertToInt64Overflow();
                return;
            }

            if (primitiveType == typeof(ulong))
            {
                ConvertToUInt64Overflow();
                return;
            }

            if (primitiveType == typeof(IntPtr))
            {
                ConvertToNativeIntOverflow();
                return;
            }

            if (primitiveType == typeof(UIntPtr))
            {
                ConvertToUnsignedNativeIntOverflow();
                return;
            }
        }

        private void ConvertToNativeInt()
        {
            UpdateState(OpCodes.Conv_I, TypeOnStack.Get<NativeInt>(), pop: 1);
        }

        public void ConvertToNativeIntOverflow()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("ConvertToNativeIntOverflow expects a value to be on the stack", Stack);
            }

            UpdateState(OpCodes.Conv_Ovf_I, TypeOnStack.Get<NativeInt>(), pop: 1);
        }

        public void UnsignedConvertToNativeIntOverflow()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("UnsignedConvertToNativeIntOverflow expects a value to be on the stack", Stack);
            }

            UpdateState(OpCodes.Conv_Ovf_I_Un, TypeOnStack.Get<NativeInt>(), pop: 1);
        }

        private void ConvertToUnsignedNativeInt()
        {
            UpdateState(OpCodes.Conv_U, TypeOnStack.Get<NativeInt>(), pop: 1);
        }

        public void ConvertToUnsignedNativeIntOverflow()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("ConvertToUnsignedNativeInt expects a value to be on the stack", Stack);
            }

            UpdateState(OpCodes.Conv_Ovf_U, TypeOnStack.Get<NativeInt>(), pop: 1);
        }

        public void UnsignedConvertToUnsignedNativeIntOverflow()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("UnsignedConvertToUnsignedNativeInt expects a value to be on the stack", Stack);
            }

            UpdateState(OpCodes.Conv_Ovf_U_Un, TypeOnStack.Get<NativeInt>(), pop: 1);
        }

        private void ConvertToSByte()
        {
            UpdateState(OpCodes.Conv_I1, TypeOnStack.Get<int>(), pop: 1);
        }

        public void ConvertToSByteOverflow()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("ConvertToSByteOverflow expects a value to be on the stack", Stack);
            }

            UpdateState(OpCodes.Conv_Ovf_I1, TypeOnStack.Get<int>(), pop: 1);
        }

        public void UnsignedConvertToSByteOverflow()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("UnsignedConvertToSByteOverflow expects a value to be on the stack", Stack);
            }

            UpdateState(OpCodes.Conv_Ovf_I1_Un, TypeOnStack.Get<int>(), pop: 1);
        }

        private void ConvertToInt16()
        {
            UpdateState(OpCodes.Conv_I2, TypeOnStack.Get<int>(), pop: 1);
        }

        public void ConvertToInt16Overflow()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("ConvertToInt16Overflow expects a value to be on the stack", Stack);
            }

            UpdateState(OpCodes.Conv_Ovf_I2, TypeOnStack.Get<int>(), pop: 1);
        }

        public void UnsignedConvertToInt16Overflow()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("UnsignedConvertToInt16Overflow expects a value to be on the stack", Stack);
            }

            UpdateState(OpCodes.Conv_Ovf_I2_Un, TypeOnStack.Get<int>(), pop: 1);
        }

        private void ConvertToInt32()
        {
            UpdateState(OpCodes.Conv_I4, TypeOnStack.Get<int>(), pop: 1);
        }

        public void ConvertToInt32Overflow()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("ConvertToInt32Overflow expects a value to be on the stack", Stack);
            }

            UpdateState(OpCodes.Conv_Ovf_I4, TypeOnStack.Get<int>(), pop: 1);
        }

        public void UnsignedConvertToInt32Overflow()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("UnsignedConvertToInt32Overflow expects a value to be on the stack", Stack);
            }

            UpdateState(OpCodes.Conv_Ovf_I4_Un, TypeOnStack.Get<int>(), pop: 1);
        }

        private void ConvertToInt64()
        {
            UpdateState(OpCodes.Conv_I8, TypeOnStack.Get<long>(), pop: 1);
        }

        public void ConvertToInt64Overflow()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("ConvertToInt64Overflow expects a value to be on the stack", Stack);
            }

            UpdateState(OpCodes.Conv_Ovf_I8, TypeOnStack.Get<long>(), pop: 1);
        }

        public void UnsignedConvertToInt64Overflow()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("UnsignedConvertToInt64Overflow expects a value to be on the stack", Stack);
            }

            UpdateState(OpCodes.Conv_Ovf_I8_Un, TypeOnStack.Get<long>(), pop: 1);
        }

        private void ConvertToFloat()
        {
            UpdateState(OpCodes.Conv_R4, TypeOnStack.Get<float>(), pop: 1);
        }

        public void UnsignedConvertToFloat()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("UnsignedConvertToFloat expects a value to be on the stack", Stack);
            }

            UpdateState(OpCodes.Conv_R_Un, TypeOnStack.Get<float>(), pop: 1);
        }

        private void ConvertToDouble()
        {
            UpdateState(OpCodes.Conv_R8, TypeOnStack.Get<double>(), pop: 1);
        }

        private void ConvertToByte()
        {
            UpdateState(OpCodes.Conv_U1, TypeOnStack.Get<int>(), pop: 1);
        }

        public void ConvertToByteOverflow()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("ConvertToByteOverflow expects a value to be on the stack", Stack);
            }

            UpdateState(OpCodes.Conv_Ovf_U1, TypeOnStack.Get<int>(), pop: 1);
        }

        public void UnsignedConvertToByteOverflow()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("UnsignedConvertToByteOverflow expects a value to be on the stack", Stack);
            }

            UpdateState(OpCodes.Conv_Ovf_U1_Un, TypeOnStack.Get<int>(), pop: 1);
        }

        private void ConvertToUInt16()
        {
            UpdateState(OpCodes.Conv_U2, TypeOnStack.Get<int>(), pop: 1);
        }

        public void ConvertToUInt16Overflow()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("ConvertToUInt16Overflow expects a value to be on the stack", Stack);
            }

            UpdateState(OpCodes.Conv_Ovf_U2, TypeOnStack.Get<int>(), pop: 1);
        }

        public void UnsignedConvertToUInt16Overflow()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("UnsignedConvertToUInt16Overflow expects a value to be on the stack", Stack);
            }

            UpdateState(OpCodes.Conv_Ovf_U2_Un, TypeOnStack.Get<int>(), pop: 1);
        }

        private void ConvertToUInt32()
        {
            UpdateState(OpCodes.Conv_U4, TypeOnStack.Get<int>(), pop: 1);
        }

        public void ConvertToUInt32Overflow()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("ConvertToUInt32Overflow expects a value to be on the stack", Stack);
            }

            UpdateState(OpCodes.Conv_Ovf_U4, TypeOnStack.Get<int>(), pop: 1);
        }

        public void UnsignedConvertToUInt32Overflow()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("UnsignedConvertToUInt32Overflow expects a value to be on the stack", Stack);
            }

            UpdateState(OpCodes.Conv_Ovf_U4_Un, TypeOnStack.Get<int>(), pop: 1);
        }

        private void ConvertToUInt64()
        {
            UpdateState(OpCodes.Conv_U4, TypeOnStack.Get<long>(), pop: 1);
        }

        public void ConvertToUInt64Overflow()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("ConvertToUInt64Overflow expects a value to be on the stack", Stack);
            }

            UpdateState(OpCodes.Conv_Ovf_U4, TypeOnStack.Get<long>(), pop: 1);
        }

        public void UnsignedConvertToUInt64Overflow()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("UnsignedConvertToUInt64Overflow expects a value to be on the stack", Stack);
            }

            UpdateState(OpCodes.Conv_Ovf_U4_Un, TypeOnStack.Get<long>(), pop: 1);
        }
    }
}
