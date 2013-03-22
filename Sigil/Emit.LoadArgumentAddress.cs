using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Loads a pointer to the argument at index (starting at zero) onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadArgumentAddress(ushort index)
        {
            if (ParameterTypes.Length == 0)
            {
                throw new ArgumentException("Delegate of type " + typeof(DelegateType) + " takes no parameters");
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

                UpdateState(OpCodes.Ldarga_S, asByte, Wrap(StackTransition.Push(ParameterTypes[index].MakePointerType()), "LoadArgumentAddress"));

                return this;
            }

            short asShort;
            unchecked
            {
                asShort = (short)index;
            }

            UpdateState(OpCodes.Ldarga, asShort, Wrap(StackTransition.Push(ParameterTypes[index].MakePointerType()), "LoadArgumentAddress"));

            return this;
        }
    }
}
