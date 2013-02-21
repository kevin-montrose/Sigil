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

            var paramList = parameters.Select(p => p.ParameterType).ToList();

            UpdateState(OpCodes.Ldftn, method, StackTransition.Push<NativeIntType>().Wrap("LoadFunctionPointer"));

            return this;
        }
    }
}
