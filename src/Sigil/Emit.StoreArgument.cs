using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pops a value off the stack and stores it into the argument to the current method identified by index.
        /// </summary>
        public Emit<DelegateType> StoreArgument(ushort index)
        {
            if (ParameterTypes.Length == 0)
            {
                throw new InvalidOperationException("Delegate of type " + typeof(DelegateType) + " takes no parameters");
            }

            if (index >= ParameterTypes.Length)
            {
                throw new ArgumentException("index must be between 0 and " + (ParameterTypes.Length - 1) + ", inclusive");
            }

            if (index >= byte.MinValue && index <= byte.MaxValue)
            {
                byte asByte;
                unchecked
                {
                    asByte = (byte)index;
                }

                UpdateState(OpCodes.Starg_S, asByte, Wrap(StackTransition.Pop(ParameterTypes[index]), "StoreArgument"));
                return this;
            }

            short asShort;
            unchecked
            {
                asShort = (short)index;
            }

            UpdateState(OpCodes.Starg, asShort, Wrap(StackTransition.Pop(ParameterTypes[index]), "StoreArgument"));

            return this;
        }
    }
}
