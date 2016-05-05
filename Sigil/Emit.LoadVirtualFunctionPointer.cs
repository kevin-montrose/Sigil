using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pops an object reference off the stack, and pushes a pointer to the given method's implementation on that object.
        /// 
        /// For static or non-virtual functions, use LoadFunctionPointer
        /// </summary>
        public Emit<DelegateType> LoadVirtualFunctionPointer(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            if (method.IsStatic)
            {
                throw new ArgumentException("Only non-static methods can be passed to LoadVirtualFunctionPointer, found " + method);
            }


            var parameters = method.GetParameters();
            var paramList = LinqAlternative.Select(parameters, p => p.ParameterType).ToArray();

            return InnerLoadVirtualFunctionPointer(method, paramList);
        }

        /// <summary>
        /// Pops an object reference off the stack, and pushes a pointer to the given method's implementation on that object.
        /// 
        /// For static or non-virtual functions, use LoadFunctionPointer
        /// 
        /// This method is provided as MethodBuilder cannot be inspected for parameter information at runtime.  If the passed parameterTypes
        /// do not match the given method, the produced code will be invalid.
        /// </summary>
        public Emit<DelegateType> LoadVirtualFunctionPointer(MethodBuilder method, Type[] parameterTypes)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (method.IsStatic)
            {
                throw new ArgumentException("Only non-static methods can be passed to LoadVirtualFunctionPointer, found " + method);
            }

            if(parameterTypes == null)
            {
                throw new ArgumentNullException(nameof(parameterTypes));
            }

            return InnerLoadVirtualFunctionPointer(method, parameterTypes);
        }

        Emit<DelegateType> InnerLoadVirtualFunctionPointer(MethodInfo method, Type[] parameterTypes)
        {
            var thisType =
               HasFlag(method.CallingConvention, CallingConventions.HasThis) ?
                   method.DeclaringType :
                   null;

            var paramList = new List<Type>(parameterTypes);

            var declaring = method.DeclaringType;

            if (TypeHelpers.IsValueType(declaring))
            {
                declaring = declaring.MakePointerType();
            }

            paramList.Insert(0, declaring);

            var type =
                TypeOnStack.GetKnownFunctionPointer(
                    method.CallingConvention,
                    thisType,
                    method.ReturnType,
                    paramList.ToArray()
                );

            var transitions =
                new[]
                {
                    new StackTransition(new [] { declaring }, new [] { typeof(NativeIntType) })
                };

            UpdateState(OpCodes.Ldvirtftn, method, parameterTypes, Wrap(transitions, "LoadVirtualFunctionPointer"));

            return this;
        }
    }
}
