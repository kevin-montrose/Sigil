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

        public void ConvertToNativeInt()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("ConvertToNativeInt expects a value to be on the stack", Stack);
            }

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

        public void ConvertToUnsignedNativeInt()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("ConvertToUnsignedNativeInt expects a value to be on the stack", Stack);
            }

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

        public void ConvertToSByte()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("ConvertToSByte expects a value to be on the stack", Stack);
            }

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

        public void ConvertToInt16()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("ConvertToInt16 expects a value to be on the stack", Stack);
            }

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

        public void ConvertToInt32()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("ConvertToInt32 expects a value to be on the stack", Stack);
            }

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

        public void ConvertToInt64()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("ConvertToInt64 expects a value to be on the stack", Stack);
            }

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

        public void ConvertToFloat()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("ConvertToFloat expects a value to be on the stack", Stack);
            }

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

        public void ConvertToDouble()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("ConvertToDouble expects a value to be on the stack", Stack);
            }

            UpdateState(OpCodes.Conv_R8, TypeOnStack.Get<double>(), pop: 1);
        }

        public void ConvertToByte()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("ConvertToByte expects a value to be on the stack", Stack);
            }

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

        public void ConvertToUInt16()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("ConvertToUInt16 expects a value to be on the stack", Stack);
            }

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

        public void ConvertToUInt32()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("ConvertToUInt32 expects a value to be on the stack", Stack);
            }

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

        public void ConvertToUInt64()
        {
            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("ConvertToUInt64 expects a value to be on the stack", Stack);
            }

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
