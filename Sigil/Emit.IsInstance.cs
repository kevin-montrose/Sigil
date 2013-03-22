using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pops a value from the stack and casts to the given type if possible pushing the result, otherwise pushes a null.
        /// 
        /// This is analogous to C#'s `as` operator.
        /// </summary>
        public Emit<DelegateType> IsInstance<Type>()
        {
            return IsInstance(typeof(Type));
        }

        /// <summary>
        /// Pops a value from the stack and casts to the given type if possible pushing the result, otherwise pushes a null.
        /// 
        /// This is analogous to C#'s `as` operator.
        /// </summary>
        public Emit<DelegateType> IsInstance(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            var curIndex = IL.Index;
            bool elided = false;

            VerificationCallback before =
                (stack, baseless) =>
                {
                    // Can't reason about stack unless it's completely known
                    if (baseless || elided) return;

                    var onStack = stack.First();

                    if (onStack.All(a => ExtensionMethods.IsAssignableFrom(type, a)))
                    {
                        ElidableCasts.Add(curIndex);
                        elided = true;
                    }
                };

            var transitions =
                new[] 
                {
                    new StackTransition(new[] { typeof(WildcardType) }, new [] { type }, before)
                };

            UpdateState(OpCodes.Isinst, type, Wrap(transitions, "IsInstance"));

            return this;
        }
    }
}
