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
        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes a void return and no parameters.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> CallIndirect(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(void), Type.EmptyTypes);
        }

        #region Generic CallIndirect Finder Helpers

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and no parameters.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> CallIndirect<ReturnType>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12), typeof(ParameterType13));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12), typeof(ParameterType13), typeof(ParameterType14));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14, ParameterType15>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12), typeof(ParameterType13), typeof(ParameterType14), typeof(ParameterType15));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14, ParameterType15, ParameterType16>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12), typeof(ParameterType13), typeof(ParameterType14), typeof(ParameterType15), typeof(ParameterType16));
        }

        #endregion

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// </summary>
        public Emit<DelegateType> CallIndirect(CallingConventions callConventions, Type returnType, params Type[] parameterTypes)
        {
            if (returnType == null)
            {
                throw new ArgumentNullException("returnType");
            }

            if (parameterTypes == null)
            {
                throw new ArgumentNullException("parameterTypes");
            }

            var known = CallingConventions.Any | CallingConventions.ExplicitThis | CallingConventions.HasThis | CallingConventions.Standard | CallingConventions.VarArgs;
            known = ~known;

            if ((callConventions & known) != 0)
            {
                throw new ArgumentException("Unexpected value not in CallingConventions", "callConventions");
            }

            if (!AllowsUnverifiableCIL)
            {
                throw new InvalidOperationException("CallIndirect isn't verifiable");
            }

            var takeExtra = 1;

            if (callConventions.HasFlag(CallingConventions.HasThis))
            {
                takeExtra++;
            }

            var onStack = Stack.Top(parameterTypes.Length + takeExtra);

            if (onStack == null)
            {
                FailStackUnderflow(parameterTypes.Length + takeExtra);
            }

            

            var reversed = onStack.Reverse().ToList();
            int fPtrLoc= reversed.Count - 1;
            var funcPtr = reversed[fPtrLoc];
            reversed.RemoveAt(reversed.Count - 1);

            TypeOnStack thisType = null;
            if (callConventions.HasFlag(CallingConventions.HasThis))
            {
                thisType = reversed[0];
                reversed.RemoveAt(0);
            }

            if (funcPtr != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilVerificationException("CallIndirect expects a native int to be on the top of the stack, found " + funcPtr, IL.Instructions(Locals), Stack, fPtrLoc);
            }

            // We can only do this if the native int got on the stack because of a call to LoadFunctionPointer or LoadVirtualFunctionPointer
            //   If someone got an IntPtr or similar on the stack from a method call, we can't validate jack
            if (funcPtr.HasAttachedMethodInfo)
            {
                if (funcPtr.CallingConvention != callConventions)
                {
                    throw new SigilVerificationException("CallIndirect expects method calling conventions to match, found " + funcPtr.CallingConvention + " on the stack", IL.Instructions(Locals), Stack, fPtrLoc);
                }

                if (funcPtr.InstanceType != null && !funcPtr.InstanceType.IsAssignableFrom(thisType))
                {
                    throw new SigilVerificationException("CallIndirect expects a 'this' value assignable to " + funcPtr.InstanceType + ", found " + thisType, IL.Instructions(Locals), Stack, fPtrLoc);
                }

                if (funcPtr.ReturnType != returnType)
                {
                    throw new SigilVerificationException("CallIndirect expects method return types to match, found " + funcPtr.ReturnType + " on the stack", IL.Instructions(Locals), Stack, fPtrLoc);
                }

                for (var i = 0; i < parameterTypes.Length; i++)
                {
                    var shouldBe = parameterTypes[i];
                    var actuallyIs = reversed[i];

                    if (!shouldBe.IsAssignableFrom(actuallyIs))
                    {
                        throw new SigilVerificationException("CallIndirect expected a value assignable to " + shouldBe + ", found " + actuallyIs, IL.Instructions(Locals), Stack, reversed.Count - 1 - i);
                    }
                }
            }

            UpdateState(OpCodes.Calli, callConventions, returnType, parameterTypes, pop: (parameterTypes.Length + takeExtra));

            return this;
        }
    }
}
