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
        /// <summary>
        /// Calls the given method virtually.  Pops its arguments in reverse order (left-most deepest in the stack), and pushes the return value if it is non-void.
        /// 
        /// The `this` reference should appear before any arguments (deepest in the stack).
        /// 
        /// The method invoked at runtime is determined by the type of the `this` reference.
        /// 
        /// If the method invoked shouldn't vary (or if the method is static), use Call instead.
        /// </summary>
        public Emit<DelegateType> CallVirtual(MethodInfo method, Type constrained = null)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            if (method.IsStatic)
            {
                throw new ArgumentException("Only non-static methods can be called using CallVirtual, found " + method);
            }

            var expectedParams = method.GetParameters().Select(s => TypeOnStack.Get(s.ParameterType)).ToList();

            // "this" parameter
            expectedParams.Insert(0, TypeOnStack.Get(method.DeclaringType));

            var resultType = method.ReturnType == typeof(void) ? null : TypeOnStack.Get(method.ReturnType);

            // Shove the constrained prefix in if it's supplied
            if (constrained != null)
            {
                UpdateState(OpCodes.Constrained, constrained, StackTransition.None().Wrap("CallVirtual"));
            }

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

            UpdateState(OpCodes.Callvirt, method, transitions.Wrap("CallVirtual"), resultType, pop: expectedParams.Count);

            return this;
        }
    }
}
