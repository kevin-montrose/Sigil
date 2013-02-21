using Sigil.Impl;
using System;
using System.Linq;
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
            var paramList = parameters.Select(p => p.ParameterType).ToList();
            paramList.Insert(0, method.DeclaringType);

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
                    new StackTransition(new [] { method.DeclaringType }, new [] { typeof(NativeIntType) })
                };

            UpdateState(OpCodes.Ldvirtftn, method, transitions.Wrap("LoadVirtualFunctionPointer"));

            return this;
        }
    }
}
