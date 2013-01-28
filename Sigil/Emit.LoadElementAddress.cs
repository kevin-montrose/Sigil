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
        private void InjectReadOnly()
        {
            while(ReadonlyPatches.Count > 0)
            {
                var elem = ReadonlyPatches[0];
                ReadonlyPatches.RemoveAt(0);

                var at = elem.Item1;
                var value = elem.Item2;

                var asObjToLdfd = value.CountMarks(OpCodes.Ldfld, 0);
                var asObjToLdfda = value.CountMarks(OpCodes.Ldflda, 0);
                var asObjToStfd = value.CountMarks(OpCodes.Stfld, 0);

                // Need to detect that the 0th parameter is *actually* a `this` pointer here
                //var asObjToCall = value.CountMarks(OpCodes.Call, 0);

                var asPtrToLdobj = value.CountMarks(OpCodes.Ldobj, 0);
                var asPtrToLdind =
                    value.CountMarks(OpCodes.Ldind_I, 0) +
                    value.CountMarks(OpCodes.Ldind_I1, 0) +
                    value.CountMarks(OpCodes.Ldind_I2, 0) +
                    value.CountMarks(OpCodes.Ldind_I4, 0) +
                    value.CountMarks(OpCodes.Ldind_I8, 0) +
                    value.CountMarks(OpCodes.Ldind_R4, 0) +
                    value.CountMarks(OpCodes.Ldind_R8, 0) +
                    value.CountMarks(OpCodes.Ldind_Ref, 0) +
                    value.CountMarks(OpCodes.Ldind_U1, 0) +
                    value.CountMarks(OpCodes.Ldind_U2, 0) +
                    value.CountMarks(OpCodes.Ldind_U4, 0);

                var asSourceToCpobj = value.CountMarks(OpCodes.Cpobj, 1);

                var totalAllowedUses =
                    asObjToLdfd +
                    asObjToLdfda +
                    asObjToStfd +
                    asPtrToLdobj +
                    asPtrToLdind +
                    asSourceToCpobj;

                var totalActualUses = value.CountMarks();

                if (totalActualUses == totalAllowedUses)
                {
                    InsertInstruction(at, OpCodes.Readonly);
                }
            }
        }

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

            // needs to be markable so we can keep track of what makes use of this value
            var pushToStack = TypeOnStack.Get(arrElemType.MakeByRefType(), makeMarkable: true);

            // Shove this away, later on we'll figure out if we can insert a readonly here
            ReadonlyPatches.Add(Tuple.Create(IL.Index, pushToStack));

            UpdateState(OpCodes.Ldelema, elementType, pushToStack, pop: 2);
        }
    }
}
