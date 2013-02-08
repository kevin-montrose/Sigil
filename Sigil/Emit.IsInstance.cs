using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;
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

            var onStack = Stack.Top();

            if (onStack == null)
            {
                throw new SigilVerificationException("IsInstance expected a value to be on the stack, but it was empty", IL, Stack);
            }

            UpdateState(OpCodes.Isinst, type, TypeOnStack.Get(type), pop: 1);

            return this;
        }
    }
}
