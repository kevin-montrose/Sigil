using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public void LoadElementAddress<ElementType>()
        {
            LoadElementAddress(typeof(ElementType));
        }

        public void LoadElementAddress(Type elementType)
        {
            if (elementType == null)
            {
                throw new ArgumentNullException("elementType");
            }

            var top = Stack.Top(2);

            if (top == null)
            {
                throw new SigilException("LoadElementAddress expects two values on the stack", Stack);
            }

            var index = top[0];
            var array = top[1];

            if (index != TypeOnStack.Get<int>() && index != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilException("LoadElementAddress expects an int or native int on the top of the stack, found " + index, Stack);
            }

            if (array.IsReference || array.IsPointer || !array.Type.IsArray)
            {
                throw new SigilException("LoadElementAddress expects an array as the second element on the stack, found " + array, Stack);
            }

            if (array.Type.GetArrayRank() != 1)
            {
                throw new SigilException("LoadElementAddress expects a 1-dimensional array, found " + array, Stack);
            }

            var arrElemType = array.Type.GetElementType();

            if (arrElemType != elementType)
            {
                throw new SigilException("LoadElementAddress found array of type " + array + ", but expected elements of type " + arrElemType, Stack);
            }

            UpdateState(OpCodes.Ldelema, elementType, TypeOnStack.Get(arrElemType.MakeByRefType()), pop: 2);
        }
    }
}
