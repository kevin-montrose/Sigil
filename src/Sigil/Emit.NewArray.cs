using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pops a size from the stack, allocates a rank-1 array of the given type, and pushes a reference to the new array onto the stack.
        /// </summary>
        public Emit<DelegateType> NewArray<ElementType>()
        {
            return NewArray(typeof(ElementType));
        }

        /// <summary>
        /// Pops a size from the stack, allocates a rank-1 array of the given type, and pushes a reference to the new array onto the stack.
        /// </summary>
        public Emit<DelegateType> NewArray(Type elementType)
        {
            if (elementType == null)
            {
                throw new ArgumentNullException("elementType");
            }

            var transitions =
                new[]
                {
                    new StackTransition(new [] { typeof(NativeIntType) }, new[] { elementType.MakeArrayType() }),
                    new StackTransition(new [] { typeof(int) }, new[] { elementType.MakeArrayType() })
                };

            UpdateState(OpCodes.Newarr, elementType, Wrap(transitions, "NewArray"));

            return this;
        }
    }
}
