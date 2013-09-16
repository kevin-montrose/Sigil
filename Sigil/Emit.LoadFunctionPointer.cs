using Sigil.Impl;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pushes a pointer to the given function onto the stack, as a native int.
        /// 
        /// To resolve a method at runtime using an object, use LoadVirtualFunctionPointer instead.
        /// </summary>
        public Emit<DelegateType> LoadFunctionPointer(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            var parameters = method.GetParameters();

            var paramList = ((LinqArray<ParameterInfo>)parameters).Select(p => p.ParameterType).ToList();

            var type =
                TypeOnStack.GetKnownFunctionPointer(
                    method.CallingConvention,
                    HasFlag(method.CallingConvention, CallingConventions.HasThis) ? method.DeclaringType : null,
                    method.ReturnType,
                    paramList.ToArray()
                );

            UpdateState(OpCodes.Ldftn, method, paramList.AsEnumerable(), Wrap(new[] { new StackTransition(new TypeOnStack[0], new[] { type }) }, "LoadFunctionPointer"));

            return this;
        }
    }
}
