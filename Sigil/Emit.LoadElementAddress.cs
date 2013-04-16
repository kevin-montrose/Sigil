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

                var asObjToLdfd = value.CountMarks(OpCodes.Ldfld, 0);
                var asObjToLdfda = value.CountMarks(OpCodes.Ldflda, 0);
                var asObjToStfd = value.CountMarks(OpCodes.Stfld, 0);

                var asObjToCall = value.CountMarks(OpCodes.Call, 0);

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

                var asSourceToCpobj = value.CountMarks(OpCodes.Cpobj, 0);

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
        public Emit<DelegateType> LoadElementAddress<ElementType>()
        {
            return LoadElementAddress(typeof(ElementType));
        }

        /// <summary>
        /// Expects a reference to an array of the given element type and an index on the stack.
        /// 
        /// Pops both, and pushes the address of the element at the given index.
        /// </summary>
        public Emit<DelegateType> LoadElementAddress(Type elementType)
        {
            if (!AllowsUnverifiableCIL)
            {
                FailUnverifiable("LoadElementAddress");
            }

            var arrayType = elementType.MakeArrayType();

            // needs to be markable so we can keep track of what makes use of this value
            var pushToStack = TypeOnStack.Get(elementType.MakeByRefType());

            // Shove this away, later on we'll figure out if we can insert a readonly here
            ReadonlyPatches.Add(SigilTuple.Create(IL.Index, pushToStack));

            var transitions =
                new[] 
                {
                    new StackTransition(new [] { TypeOnStack.Get<NativeIntType>(), TypeOnStack.Get(arrayType) }, new [] { pushToStack }),
                    new StackTransition(new [] { TypeOnStack.Get<int>(), TypeOnStack.Get(arrayType) }, new [] { pushToStack })
                };

            UpdateState(OpCodes.Ldelema, elementType, Wrap(transitions, "LoadElementAddress"));

            return this;
        }
    }
}
