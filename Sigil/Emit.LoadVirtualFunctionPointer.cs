using Sigil.Impl;
using System;
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

            var thisType =
                HasFlag(method.CallingConvention, CallingConventions.HasThis) ?
                    method.DeclaringType :
                    null;

            var parameters = method.GetParameters();
            var paramList = LinqAlternative.Select(parameters, p => p.ParameterType).ToList();

            var declaring = method.DeclaringType;

            if (declaring.IsValueType)
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

            UpdateState(OpCodes.Ldvirtftn, method, Wrap(transitions, "LoadVirtualFunctionPointer"));

            return this;
        }
    }
}
