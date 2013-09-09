using System;
using System.Reflection;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes a void return and no parameters.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit CallIndirect(CallingConventions callConventions)
        {
            InnerEmit.CallIndirect(callConventions);
            return this;
        }

        #region Generic CallIndirect Finder Helpers

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and no parameters.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit CallIndirect<ReturnType>(CallingConventions callConventions)
        {
            InnerEmit.CallIndirect<ReturnType>(callConventions);
            return this;
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit CallIndirect<ReturnType, ParameterType1>(CallingConventions callConventions)
        {
            InnerEmit.CallIndirect<ReturnType, ParameterType1>(callConventions);
            return this;
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit CallIndirect<ReturnType, ParameterType1, ParameterType2>(CallingConventions callConventions)
        {
            InnerEmit.CallIndirect<ReturnType, ParameterType1, ParameterType2>(callConventions);
            return this;
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3>(CallingConventions callConventions)
        {
            InnerEmit.CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3>(callConventions);
            return this;
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4>(CallingConventions callConventions)
        {
            InnerEmit.CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4>(callConventions);
            return this;
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5>(CallingConventions callConventions)
        {
            InnerEmit.CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5>(callConventions);
            return this;
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6>(CallingConventions callConventions)
        {
            InnerEmit.CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6>(callConventions);
            return this;
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7>(CallingConventions callConventions)
        {
            InnerEmit.CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7>(callConventions);
            return this;
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8>(CallingConventions callConventions)
        {
            InnerEmit.CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8>(callConventions);
            return this;
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9>(CallingConventions callConventions)
        {
            InnerEmit.CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9>(callConventions);
            return this;
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10>(CallingConventions callConventions)
        {
            InnerEmit.CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10>(callConventions);
            return this;
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11>(CallingConventions callConventions)
        {
            InnerEmit.CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11>(callConventions);
            return this;
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12>(CallingConventions callConventions)
        {
            InnerEmit.CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12>(callConventions);
            return this;
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13>(CallingConventions callConventions)
        {
            InnerEmit.CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13>(callConventions);
            return this;
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14>(CallingConventions callConventions)
        {
            InnerEmit.CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14>(callConventions);
            return this;
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14, ParameterType15>(CallingConventions callConventions)
        {
            InnerEmit.CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14, ParameterType15>(callConventions);
            return this;
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14, ParameterType15, ParameterType16>(CallingConventions callConventions)
        {
            InnerEmit.CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14, ParameterType15, ParameterType16>(callConventions);
            return this;
        }

        #endregion

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This override allows an arglist to be passed for calling VarArgs methods.
        /// </summary>
        public Emit CallIndirect(CallingConventions callConventions, Type returnType, Type[] parameterTypes, Type[] arglist = null)
        {
            InnerEmit.CallIndirect(callConventions, returnType, parameterTypes, arglist);
            return this;
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// </summary>
        public Emit CallIndirect(CallingConventions callConventions, Type returnType, params Type[] parameterTypes)
        {
            InnerEmit.CallIndirect(callConventions, returnType, parameterTypes);
            return this;
        }
    }
}
