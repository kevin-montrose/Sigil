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
        /// <summary>
        /// Transfers control to another method.
        /// 
        /// The parameters and calling convention of method must match the current one's.
        /// 
        /// The stack must be empty to jump.
        /// 
        /// Like the branching instructions, Jump cannot leave exception blocks.
        /// </summary>
        public Emit<DelegateType> Jump(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            if (method.CallingConvention != DynMethod.CallingConvention)
            {
                throw new ArgumentException("Jump expected a calling convention of " + DynMethod.CallingConvention + ", found " + method.CallingConvention);
            }

            var paras = method.GetParameters();

            if (paras.Length != ParameterTypes.Length)
            {
                throw new ArgumentException("Jump expected a method with " + ParameterTypes.Length + " parameters, found " + paras.Length);
            }

            if (CatchBlocks.Any(t => t.Value.Item2 == -1))
            {
                throw new InvalidOperationException("Jump cannot transfer control from a catch block");
            }

            if (FinallyBlocks.Any(t => t.Value.Item2 == -1))
            {
                throw new InvalidOperationException("Jump cannot transfer control from a finally block");
            }

            if (TryBlocks.Any(t => t.Value.Item2 == -1))
            {
                throw new InvalidOperationException("Jump cannot transfer control from an exception block");
            }

            if (!Stack.IsRoot)
            {
                throw new SigilException("Jump expected the stack to be empty", Stack);
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

            UpdateState(OpCodes.Jmp, method);

            return this;
        }
    }
}
