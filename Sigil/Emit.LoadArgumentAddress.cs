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
        public void LoadArgumentAddress(int index)
        {
            if (ParameterTypes.Length == 0)
            {
                throw new InvalidOperationException("Delegate of type " + typeof(DelegateType) + " takes no parameters");
            }

            if (index < 0 || index >= ParameterTypes.Length)
            {
                throw new ArgumentOutOfRangeException("index must be between 0 and " + (ParameterTypes.Length - 1) + ", inclusive");
            }

            if (index >= byte.MinValue && index <= byte.MaxValue)
            {
                UpdateState(OpCodes.Ldarga_S, index, TypeOnStack.Get(ParameterTypes[index].MakePointerType()));

                return;
            }

            UpdateState(OpCodes.Ldarga, index, TypeOnStack.Get(ParameterTypes[index].MakePointerType()));
        }
    }
}
