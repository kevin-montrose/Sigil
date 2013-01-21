using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public void CallIndirect(CallingConventions callConventions)
        {
            CallIndirect(callConventions, typeof(void), Type.EmptyTypes);
        }

        #region Generic CallIndirect Finder Helpers

        public void CallIndirect<ReturnType>(CallingConventions callConventions)
        {
            CallIndirect(callConventions, typeof(ReturnType));
        }

        public void CallIndirect<ReturnType, ParameterType1>(CallingConventions callConventions)
        {
            CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1));
        }

        public void CallIndirect<ReturnType, ParameterType1, ParameterType2>(CallingConventions callConventions)
        {
            CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2));
        }

        public void CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3>(CallingConventions callConventions)
        {
            CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3));
        }

        public void CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4>(CallingConventions callConventions)
        {
            CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4));
        }

        public void CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5>(CallingConventions callConventions)
        {
            CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5));
        }

        public void CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6>(CallingConventions callConventions)
        {
            CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6));
        }

        public void CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7>(CallingConventions callConventions)
        {
            CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7));
        }

        public void CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8>(CallingConventions callConventions)
        {
            CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8));
        }

        public void CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9>(CallingConventions callConventions)
        {
            CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9));
        }

        public void CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10>(CallingConventions callConventions)
        {
            CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10));
        }

        public void CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11>(CallingConventions callConventions)
        {
            CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11));
        }

        public void CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12>(CallingConventions callConventions)
        {
            CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12));
        }

        public void CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13>(CallingConventions callConventions)
        {
            CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12), typeof(ParameterType13));
        }

        public void CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14>(CallingConventions callConventions)
        {
            CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12), typeof(ParameterType13), typeof(ParameterType14));
        }

        public void CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14, ParameterType15>(CallingConventions callConventions)
        {
            CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12), typeof(ParameterType13), typeof(ParameterType14), typeof(ParameterType15));
        }

        public void CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14, ParameterType15, ParameterType16>(CallingConventions callConventions)
        {
            CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12), typeof(ParameterType13), typeof(ParameterType14), typeof(ParameterType15), typeof(ParameterType16));
        }

        #endregion

        public void CallIndirect(CallingConventions callConventions, Type returnType, params Type[] parameterTypes)
        {
            if (returnType == null)
            {
                throw new ArgumentNullException("returnType");
            }

            if (parameterTypes == null)
            {
                throw new ArgumentNullException("parameterTypes");
            }

            if ((callConventions & CallingConventions.Any) == 0 &&
                (callConventions & CallingConventions.ExplicitThis) == 0 &&
                (callConventions & CallingConventions.HasThis) == 0 &&
                (callConventions & CallingConventions.Standard) == 0 &&
                (callConventions & CallingConventions.VarArgs) == 0)
            {
                throw new ArgumentException("callConventions");
            }

            var takeExtra = 1;

            if (callConventions.HasFlag(CallingConventions.HasThis))
            {
                takeExtra++;
            }

            var onStack = Stack.Top(parameterTypes.Length + takeExtra);

            if (onStack == null)
            {
                throw new SigilException("CallIndirect expected " + (parameterTypes.Length + takeExtra) + " values on the stack", Stack);
            }

            var reversed = onStack.Reverse().ToList();
            var funcPtr = reversed[reversed.Count - 1];
            reversed.RemoveAt(reversed.Count - 1);

            TypeOnStack thisType = null;
            if (callConventions.HasFlag(CallingConventions.HasThis))
            {
                thisType = reversed[0];
                reversed.RemoveAt(0);
            }

            if (funcPtr != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilException("CallIndirect expects a native int to be on the top of the stack, found " + funcPtr, Stack);
            }

            // We can only do this if the native int got on the stack because of a call to LoadFunctionPointer or LoadVirtualFunctionPointer
            //   If someone got an IntPtr or similar on the stack from a method call, we can't validate jack
            if (funcPtr.HasAttachedMethodInfo)
            {
                if (funcPtr.CallingConvention != callConventions)
                {
                    throw new SigilException("CallIndirect expects method calling conventions to match, found "+funcPtr.CallingConvention+" on the stack", Stack);
                }

                if (funcPtr.InstanceType != null && !funcPtr.InstanceType.IsAssignableFrom(thisType))
                {
                    throw new SigilException("CallIndirect expects a 'this' value assignable to " + funcPtr.InstanceType + ", found " + thisType, Stack);
                }

                if (funcPtr.ReturnType != returnType)
                {
                    throw new SigilException("CallIndirect expects method return types to match, found " + funcPtr.ReturnType + " on the stack", Stack);
                }

                for (var i = 0; i < parameterTypes.Length; i++)
                {
                    var shouldBe = parameterTypes[i];
                    var actuallyIs = reversed[i];

                    if (!shouldBe.IsAssignableFrom(actuallyIs))
                    {
                        throw new SigilException("CallIndirect expected a value assignable to " + shouldBe + ", found " + actuallyIs, Stack);
                    }
                }
            }

            UpdateState(OpCodes.Calli, callConventions, returnType, parameterTypes, pop: (parameterTypes.Length + takeExtra));
        }
    }
}
