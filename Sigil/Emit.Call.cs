using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public void Call(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            var expectedParams = method.GetParameters().Select(s => TypeOnStack.Get(s.ParameterType)).ToList();

            var onStack = Stack.Top(expectedParams.Count).ToList();

            if (onStack == null)
            {
                throw new SigilException("Call to " + method + " expected parameters [" + string.Join(", ", expectedParams) + "] to be on the stack", Stack);
            }

            // Things come off the stack in "Reverse" order
            onStack.Reverse();

            for (var i = 0; i < expectedParams.Count; i++)
            {
                var shouldBe = expectedParams[i];
                var actuallyIs = onStack[i];

                if (!shouldBe.IsAssignableFrom(actuallyIs))
                {
                    throw new SigilException("Parameter #" + (i + 1) + " to " + method + " should be " + shouldBe + ", but found " + actuallyIs, Stack);
                }
            }

            var resultType = method.ReturnType == typeof(void) ? null : TypeOnStack.Get(method.ReturnType);

            UpdateState(OpCodes.Call, method, resultType, pop: expectedParams.Count);
        }
    }
}
