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
        private void InjectTailCall()
        {
            if (InstructionStream.Count < 2) return;

            var last = InstructionStream[InstructionStream.Count - 1];
            var nextToLast = InstructionStream[InstructionStream.Count - 2];

            if (last.Item1 != OpCodes.Ret) return;

            if (!new[] { OpCodes.Call, OpCodes.Calli, OpCodes.Callvirt }.Contains(nextToLast.Item1)) return;

            InsertInstruction(IL.Index - 2, OpCodes.Tailcall);
        }

        public void Call(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            var expectedParams = method.GetParameters().Select(s => TypeOnStack.Get(s.ParameterType)).ToList();

            // Instance methods expect this to preceed parameters
            if (method.CallingConvention.HasFlag(CallingConventions.HasThis))
            {
                expectedParams.Insert(0, TypeOnStack.Get(method.DeclaringType));
            }

            var onStack = Stack.Top(expectedParams.Count);

            if (onStack == null)
            {
                throw new SigilException("Call to " + method + " expected parameters [" + string.Join(", ", expectedParams) + "] to be on the stack", Stack);
            }

            // Things come off the stack in "Reverse" order
            var onStackR = onStack.Reverse().ToList();

            for (var i = 0; i < expectedParams.Count; i++)
            {
                var shouldBe = expectedParams[i];
                var actuallyIs = onStackR[i];

                if (!shouldBe.IsAssignableFrom(actuallyIs))
                {
                    // OK, apparently for the `this` pointer, you can get away with using an explicit reference (type &) rather than
                    //   an "object reference" (type O)
                    if (i == 0 && actuallyIs.IsReference && method.CallingConvention.HasFlag(CallingConventions.HasThis))
                    {
                        var actuallyIsDeref = TypeOnStack.Get(actuallyIs.Type);
                        if (shouldBe.IsAssignableFrom(actuallyIsDeref))
                        {
                            continue;
                        }
                    }

                    throw new SigilException("Parameter #" + i + " to " + method + " should be " + shouldBe + ", but found " + actuallyIs, Stack);
                }
            }

            var resultType = method.ReturnType == typeof(void) ? null : TypeOnStack.Get(method.ReturnType);

            var firstParamIsThis =
                method.CallingConvention.HasFlag(CallingConventions.HasThis) ||
                method.CallingConvention.HasFlag(CallingConventions.ExplicitThis);
            
            UpdateState(OpCodes.Call, method, resultType, pop: expectedParams.Count, firstParamIsThis: firstParamIsThis);
        }
    }
}
