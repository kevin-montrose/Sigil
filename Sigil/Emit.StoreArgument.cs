using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

using Sigil.Impl;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public void StoreArgument(int index)
        {
            if (ParameterTypes.Length == 0)
            {
                throw new InvalidOperationException("Delegate of type " + typeof(DelegateType) + " takes no parameters");
            }

            if (index < 0 || index >= ParameterTypes.Length)
            {
                throw new ArgumentOutOfRangeException("index must be between 0 and " + (ParameterTypes.Length - 1) + ", inclusive");
            }

            var onStack = Stack.Top();

            if (onStack == null)
            {
                throw new SigilException("StoreArgument expects a value on the stack, but it was empty", Stack);
            }

            var paramType = ParameterTypes[index];

            if (!paramType.IsAssignableFrom(onStack[0]))
            {
                throw new SigilException("StoreArgument expects type on stack to be assignable to argument type, found " + onStack[0] + " and " + paramType, Stack);
            }

            if (index <= byte.MaxValue)
            {
                UpdateState(OpCodes.Starg_S, index, pop: 1);
                return;
            }

            UpdateState(OpCodes.Starg, index, pop: 1);
        }
    }
}
