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

            switch (index)
            {
                case 0: UpdateState(OpCodes.Ldarg_0, TypeOnStack.Get(ParameterTypes[index])); return this;
                case 1: UpdateState(OpCodes.Ldarg_1, TypeOnStack.Get(ParameterTypes[index])); return this;
                case 2: UpdateState(OpCodes.Ldarg_2, TypeOnStack.Get(ParameterTypes[index])); return this;
                case 3: UpdateState(OpCodes.Ldarg_3, TypeOnStack.Get(ParameterTypes[index])); return this;
            }

            if (index >= byte.MinValue && index <= byte.MaxValue)
            {
                UpdateState(OpCodes.Ldarg_S, (byte)index, TypeOnStack.Get(ParameterTypes[index]));
                return this;
            }

            short asShort;
            unchecked
            {
                asShort = (short)index;
            }

            UpdateState(OpCodes.Ldarg, asShort, TypeOnStack.Get(ParameterTypes[index]));

            return this;
        }
    }
}
