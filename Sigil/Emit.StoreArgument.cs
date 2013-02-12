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

            var onStack = Stack.Top();

            if (onStack == null)
            {
                FailStackUnderflow(1);
            }

            var paramType = ParameterTypes[index];

            if (!paramType.IsAssignableFrom(onStack[0]))
            {
                throw new SigilVerificationException("StoreArgument expects type on stack to be assignable to " + paramType + ", found " + onStack[0], IL.Instructions(Locals), Stack, 0);
            }

            if (index >= byte.MinValue && index <= byte.MaxValue)
            {
                byte asByte;
                unchecked
                {
                    asByte = (byte)index;
                }

                UpdateState(OpCodes.Starg_S, asByte, pop: 1);
                return this;
            }

            short asShort;
            unchecked
            {
                asShort = (short)index;
            }
            
            UpdateState(OpCodes.Starg, asShort, pop: 1);

            return this;
        }
    }
}
