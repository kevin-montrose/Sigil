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
        public void Jump(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            if (!Stack.IsRoot)
            {
                throw new SigilException("Jump expected the stack to be empty", Stack);
            }

            if (method.CallingConvention != DynMethod.CallingConvention)
            {
                throw new SigilException("Jump expected a calling convention of " + DynMethod.CallingConvention + ", found " + method.CallingConvention, Stack);
            }

            var paras = method.GetParameters();

            if (paras.Length != ParameterTypes.Length)
            {
                throw new SigilException("Jump expected a method with " + ParameterTypes.Length + " parameters, found " + paras.Length, Stack);
            }

            if (method.CallingConvention.HasFlag(CallingConventions.HasThis))
            {
                throw new SigilException("Jump cannot transfer to an instance method from a DynamicMethod", Stack);
            }

            for (var i = 0; i < paras.Length; i++)
            {
                var shouldBe = paras[i].ParameterType;
                var actuallyIs = ParameterTypes[i];

                if (!shouldBe.IsAssignableFrom(actuallyIs))
                {
                    throw new SigilException("Jump expected the #" + i + " parameter to be assignable from " + actuallyIs + ", but found " + shouldBe, Stack);
                }
            }

            if (TryBlocks.Any(t => t.Value.Item2 == -1))
            {
                throw new SigilException("Jump cannot transfer control from an exception block", Stack);
            }

            if (CatchBlocks.Any(t => t.Value.Item2 == -1))
            {
                throw new SigilException("Jump cannot transfer control from a catch block", Stack);
            }

            if (FinallyBlocks.Any(t => t.Value.Item2 == -1))
            {
                throw new SigilException("Jump cannot transfer control from a finally block", Stack);
            }

            UpdateState(OpCodes.Jmp, method);
        }
    }
}
