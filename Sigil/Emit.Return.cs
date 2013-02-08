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
        /// Ends the execution of the current method.
        /// 
        /// If the current method does not return void, pops a value from the stack and returns it to the calling method.
        /// 
        /// Return should leave the stack empty.
        /// </summary>
        public Emit<DelegateType> Return()
        {
            if (ReturnType == TypeOnStack.Get(typeof(void)))
            {
                if (!Stack.IsRoot)
                {
                    throw new SigilVerificationException("Returning from a void method must leave the stack empty", IL, Stack);
                }

                UpdateState(OpCodes.Ret);

                return this;
            }

            var retType = Stack.Top();

            if(retType == null)
            {
                throw new SigilVerificationException("Return expects a value on the stack, but the stack is empty", IL, Stack);
            }

            if (!ReturnType.IsAssignableFrom(retType[0]))
            {
                throw new SigilVerificationException("Return expects a value assignable to " + ReturnType + " to be on the stack; found " + retType[0], IL, Stack);
            }

            UpdateState(OpCodes.Ret, pop: 1);

            if (!Stack.IsRoot)
            {
                throw new SigilVerificationException("Return should leave the stack empty", IL, Stack);
            }

            return this;
        }
    }
}
