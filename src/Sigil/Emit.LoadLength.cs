using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pops a reference to a rank 1 array off the stack, and pushes it's length onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadLength<ElementType>()
        {
            return LoadLength(typeof(ElementType));
        }

        /// <summary>
        /// Pops a reference to a rank 1 array off the stack, and pushes it's length onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadLength(Type elementType)
        {
            if (elementType == null)
            {
                throw new ArgumentNullException("elementType");
            }

            var transitions =
                new[] {
                    new StackTransition(new [] { elementType.MakeArrayType() }, new [] { typeof(int) })
                };

            UpdateState(OpCodes.Ldlen, Wrap(transitions, "LoadLength"));

            return this;
        }
    }
}
