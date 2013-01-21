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
        public void LoadFunctionPointer(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            var parameters = method.GetParameters();
            if(parameters.Any(p => p.IsOptional))
            {
                throw new ArgumentException("Methods with optional parameters are not supported");
            }

            var paramList = parameters.Select(p => p.ParameterType).ToList();

            var thisType =
                method.CallingConvention.HasFlag(CallingConventions.HasThis) ?
                    method.DeclaringType :
                    null;

            var type = 
                TypeOnStack.GetKnownFunctionPointer(
                    method.CallingConvention,
                    thisType,
                    method.ReturnType,
                    paramList.ToArray()
                );

            UpdateState(OpCodes.Ldftn, method, type);
        }
    }
}
