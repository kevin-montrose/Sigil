using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        private void InjectTailCall()
        {
            for (var i = 0; i < IL.Index; i++)
            {
                var instr = IL[i];

                if (instr.IsInstruction == OpCodes.Ret)
                {
                    int callIx = -1;

                    for (var j = i - 1; j >= 0; j--)
                    {
                        var atJ = IL[j];

                        if (atJ.MarksLabel != null)
                        {
                            break;
                        }

                        if (atJ.IsInstruction.HasValue)
                        {
                            if (atJ.IsInstruction.Value.IsCall())
                            {
                                callIx = j;
                            }

                            break;
                        }
                    }

                    if (callIx == -1) continue;

                    InsertInstruction(callIx, OpCodes.Tailcall);
                    i++;
                }
            }
        }

        /// <summary>
        /// Calls the given method.  Pops its arguments in reverse order (left-most deepest in the stack), and pushes the return value if it is non-void.
        /// 
        /// If the given method is an instance method, the `this` reference should appear before any parameters.
        /// 
        /// Call does not respect overrides, the implementation defined by the given MethodInfo is what will be called at runtime.
        /// 
        /// To call overrides of instance methods, use CallVirtual.
        /// </summary>
        public Emit<DelegateType> Call(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            var expectedParams = method.GetParameters().Select(s => TypeOnStack.Get(s.ParameterType)).ToList();

            // Instance methods expect this to preceed parameters
            if (HasFlag(method.CallingConvention, CallingConventions.HasThis))
            {
                expectedParams.Insert(0, TypeOnStack.Get(method.DeclaringType));
            }

            var resultType = method.ReturnType == typeof(void) ? null : TypeOnStack.Get(method.ReturnType);

            var firstParamIsThis =
                HasFlag(method.CallingConvention, CallingConventions.HasThis) ||
                HasFlag(method.CallingConvention, CallingConventions.ExplicitThis);

            IEnumerable<StackTransition> transitions;
            if (resultType != null)
            {
                transitions =
                    new[]
                    {
                        new StackTransition(expectedParams.AsEnumerable().Reverse(), new [] { resultType })
                    };
            }
            else
            {
                transitions =
                    new[]
                    {
                        new StackTransition(expectedParams.AsEnumerable().Reverse(), new TypeOnStack[0])
                    };
            }

            UpdateState(OpCodes.Call, method, transitions.Wrap("Call"), firstParamIsThis: firstParamIsThis);

            return this;
        }
    }
}
