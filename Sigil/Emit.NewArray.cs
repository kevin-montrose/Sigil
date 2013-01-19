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
        public void NewArray<ElementType>()
        {
            NewArray(typeof(ElementType));
        }

        public void NewArray(Type elementType)
        {
            if (elementType == null)
            {
                throw new ArgumentNullException("elementType");
            }

            var onStack = Stack.Top();

            if (onStack == null)
            {
                throw new SigilException("NewArray expects the size of the array to be on the stack, but it was empty", Stack);
            }

            var size = onStack[0];

            if (size != TypeOnStack.Get<int>() && size != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilException("NewArray expecte size to be an int or native int, found " + size, Stack);
            }

            UpdateState(OpCodes.Newarr, elementType, TypeOnStack.Get(elementType.MakeArrayType()), pop: 1);
        }
    }
}
