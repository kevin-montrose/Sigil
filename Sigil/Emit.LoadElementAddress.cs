using Sigil.Impl;
using System;
using System.Reflection.Emit;

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

                var asObjToLdfd = value.CountMarks(OpCodes.Ldfld, 0, false);
                var asObjToLdfda = value.CountMarks(OpCodes.Ldflda, 0, false);
                var asObjToStfd = value.CountMarks(OpCodes.Stfld, 0, false);

                var asObjToCall = value.CountMarks(OpCodes.Call, 0, true);

                var asPtrToLdobj = value.CountMarks(OpCodes.Ldobj, 0, false);
                var asPtrToLdind =
                    value.CountMarks(OpCodes.Ldind_I, 0, false) +
                    value.CountMarks(OpCodes.Ldind_I1, 0, false) +
                    value.CountMarks(OpCodes.Ldind_I2, 0, false) +
                    value.CountMarks(OpCodes.Ldind_I4, 0, false) +
                    value.CountMarks(OpCodes.Ldind_I8, 0, false) +
                    value.CountMarks(OpCodes.Ldind_R4, 0, false) +
                    value.CountMarks(OpCodes.Ldind_R8, 0, false) +
                    value.CountMarks(OpCodes.Ldind_Ref, 0, false) +
                    value.CountMarks(OpCodes.Ldind_U1, 0, false) +
                    value.CountMarks(OpCodes.Ldind_U2, 0, false) +
                    value.CountMarks(OpCodes.Ldind_U4, 0, false);

                var asSourceToCpobj = value.CountMarks(OpCodes.Cpobj, 1, false);

                var totalAllowedUses =
                    asObjToLdfd +
                    asObjToLdfda +
                    asObjToStfd +
                    asPtrToLdobj +
                    asPtrToLdind +
                    asSourceToCpobj +
                    asObjToCall;

                var totalActualUses = value.CountMarks();

                if (totalActualUses == totalAllowedUses)
                {
                    InsertInstruction(at, OpCodes.Readonly);
                }
            }
        }

        /// <summary>
        /// Expects a reference to an array of the given element type and an index on the stack.
        /// 
        /// Pops both, and pushes the address of the element at the given index.
        /// </summary>
        public Emit<DelegateType> LoadElementAddress()
        {
            if (!AllowsUnverifiableCIL)
            {
                FailUnverifiable();
            }

            var top = Stack.Top(2);

            if (top == null)
            {
                FailStackUnderflow(2);
            }

            var index = top[0];
            var array = top[1];

            if (index != TypeOnStack.Get<int>() && index != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilVerificationException("LoadElementAddress expects an int or native int on the top of the stack, found " + index, IL.Instructions(Locals), Stack, 0);
            }

            if (array.IsReference || array.IsPointer || !array.Type.IsArray)
            {
                throw new SigilVerificationException("LoadElementAddress expects an array as the second element on the stack, found " + array, IL.Instructions(Locals), Stack, 1);
            }

            if (array.Type.GetArrayRank() != 1)
            {
                throw new SigilVerificationException("LoadElementAddress expects a 1-dimensional array, found " + array, IL.Instructions(Locals), Stack, 1);
            }

            var arrElemType = array.Type.GetElementType();
            
            // needs to be markable so we can keep track of what makes use of this value
            var pushToStack = TypeOnStack.Get(arrElemType.MakeByRefType(), makeMarkable: true);

            // Shove this away, later on we'll figure out if we can insert a readonly here
            ReadonlyPatches.Add(Tuple.Create(IL.Index, pushToStack));

            UpdateState(OpCodes.Ldelema, arrElemType, pushToStack, pop: 2);

            return this;
        }
    }
}
