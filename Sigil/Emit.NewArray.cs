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

            var onStack = Stack.Top();

            if (onStack == null)
            {
                FailStackUnderflow(1);
            }

            var size = onStack[0];

            if (size != TypeOnStack.Get<int>() && size != TypeOnStack.Get<NativeIntType>())
            {
                throw new SigilVerificationException("NewArray expecte size to be an int or native int, found " + size, IL.Instructions(LocalsByIndex), Stack, 0);
            }

            UpdateState(OpCodes.Newarr, elementType, TypeOnStack.Get(elementType.MakeArrayType()), pop: 1);

            return this;
        }
    }
}
