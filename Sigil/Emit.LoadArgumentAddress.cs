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
                UpdateState(OpCodes.Ldarga_S, (byte)index, TypeOnStack.Get(ParameterTypes[index].MakePointerType()));

                return this;
            }

            short asShort;
            unchecked
            {
                asShort = (short)index;
            }

            UpdateState(OpCodes.Ldarga, asShort, TypeOnStack.Get(ParameterTypes[index].MakePointerType()));

            return this;
        }
    }
}
