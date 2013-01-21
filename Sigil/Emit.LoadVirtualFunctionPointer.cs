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
        public void LoadVirtualFunctionPointer(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            var parameters = method.GetParameters();
            if (parameters.Any(p => p.IsOptional))
            {
                throw new ArgumentException("Methods with optional parameters are not supported");
            }

            if (method.IsStatic)
            {
                throw new SigilException("Only non-static methods can be passed to LoadVirtualFunctionPointer, found " + method, Stack);
            }

            var thisType =
                method.CallingConvention.HasFlag(CallingConventions.HasThis) ?
                    method.DeclaringType :
                    null;

            var paramList = parameters.Select(p => p.ParameterType).ToList();
            paramList.Insert(0, method.DeclaringType);

            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("LoadVirtualFunctionPointer expects a value to be on the stack, but it was empty", Stack);
            }

            var objPtr = top[0];

            if (!method.DeclaringType.IsAssignableFrom(objPtr))
            {
                throw new SigilException("Expected a value assignable to " + method.DeclaringType + " to be on the stack, found " + objPtr, Stack);
            }

            var type =
                TypeOnStack.GetKnownFunctionPointer(
                    method.CallingConvention,
                    thisType,
                    method.ReturnType,
                    paramList.ToArray()
                );

            UpdateState(OpCodes.Ldvirtftn, method, type, pop: 1);
        }
    }
}
