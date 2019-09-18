using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Loads the argument at the given index (starting at 0) for the current method onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadArgument(ushort index)
        {
            if (ParameterTypes.Length == 0)
            {
                throw new ArgumentException("Delegate of type " + typeof(DelegateType) + " takes no parameters");
            }

            if (index >= ParameterTypes.Length)
            {
                throw new ArgumentException("index must be between 0 and " + (ParameterTypes.Length - 1) + ", inclusive");
            }

            var transitions = Wrap(StackTransition.Push(ParameterTypes[index]), "LoadArgument");

            switch (index)
            {
                case 0: UpdateState(OpCodes.Ldarg_0, transitions); return this;
                case 1: UpdateState(OpCodes.Ldarg_1, transitions); return this;
                case 2: UpdateState(OpCodes.Ldarg_2, transitions); return this;
                case 3: UpdateState(OpCodes.Ldarg_3, transitions); return this;
            }

            if (index >= byte.MinValue && index <= byte.MaxValue)
            {
                UpdateState(OpCodes.Ldarg_S, (byte)index, transitions);
                return this;
            }

            short asShort;
            unchecked
            {
                asShort = (short)index;
            }

            UpdateState(OpCodes.Ldarg, asShort, transitions);

            return this;
        }
    }
}
