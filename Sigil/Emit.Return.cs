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
                    var stackSize = Stack.Count();
                    var mark = new List<int>();
                    for (var i = 0; i < stackSize; i++)
                    {
                        mark.Add(i);
                    }

                    throw new SigilVerificationException("Returning from a void method must leave the stack empty", IL, Stack, mark.ToArray());
                }

                UpdateState(OpCodes.Ret);

                return this;
            }

            var retType = Stack.Top();

            if(retType == null)
            {
                FailStackUnderflow(1);
            }

            if (!ReturnType.IsAssignableFrom(retType[0]))
            {
                throw new SigilVerificationException("Return expects a value assignable to " + ReturnType + " to be on the stack; found " + retType[0], IL, Stack, 0);
            }

            UpdateState(OpCodes.Ret, pop: 1);

            if (!Stack.IsRoot)
            {
                var stackSize = Stack.Count();
                var mark = new List<int>();
                for (var i = 0; i < stackSize; i++)
                {
                    mark.Add(i);
                }

                throw new SigilVerificationException("Return should leave the stack empty", IL, Stack, mark.ToArray());
            }

            return this;
        }
    }
}
