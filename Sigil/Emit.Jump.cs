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

            if (method.CallingConvention != CallingConventions)
            {
                throw new ArgumentException("Jump expected a calling convention of " + CallingConventions + ", found " + method.CallingConvention);
            }

            var paras = method.GetParameters();

            if (paras.Length != ParameterTypes.Length)
            {
                throw new ArgumentException("Jump expected a method with " + ParameterTypes.Length + " parameters, found " + paras.Length);
            }

            if (!AllowsUnverifiableCIL)
            {
                FailUnverifiable();
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
                throw new SigilVerificationException("Jump expected the stack to be empty", IL.Instructions(LocalsByIndex), Stack);
            }

            for (var i = 0; i < paras.Length; i++)
            {
                var shouldBe = paras[i].ParameterType;
                var actuallyIs = ParameterTypes[i];

                if (!shouldBe.IsAssignableFrom(actuallyIs))
                {
                    throw new SigilVerificationException("Jump expected the #" + i + " parameter to be assignable from " + actuallyIs + ", but found " + shouldBe, IL.Instructions(LocalsByIndex), Stack);
                }
            }

            UpdateState(OpCodes.Jmp, method, StackTransition.None().Wrap("Jump"));

            return this;
        }
    }
}
