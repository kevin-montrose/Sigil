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
        public void Return()
        {
            if (ReturnType == typeof(void))
            {
                if (!Stack.IsRoot)
                {
                    throw new SigilException("Returning from a void method must leave the stack empty", Stack);
                }

                UpdateState(OpCodes.Ret);

                return;
            }

            var retType = Stack.Top();

            if(retType == null)
            {
                throw new SigilException("Return expects a value on the stack, but the stack is empty", Stack);
            }

            if (!ReturnType.IsAssignableFrom(retType[0]))
            {
                throw new SigilException("Return expects a value assignable to " + ReturnType.FullName + " to be on the stack; found " + retType[0], Stack);
            }

            UpdateState(OpCodes.Ret, pop: 1);

            if (!Stack.IsRoot)
            {
                throw new SigilException("Return should leave the stack empty", Stack);
            }
        }
    }
}
