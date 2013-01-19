using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        private void VerifyAndBinaryBitwise(string name, OpCode op)
        {
            var onStack = Stack.Top(2);

            if (onStack == null)
            {
                throw new SigilException(name+" expects two values on the stack", Stack);
            }

            var val2 = onStack[0];
            var val1 = onStack[1];

            if (val1 != TypeOnStack.Get<int>() && val1 != TypeOnStack.Get<long>() && val1 != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilException(name+" expects integral types, but the first value type was " + val1, Stack);
            }

            if (val2 != TypeOnStack.Get<int>() && val2 != TypeOnStack.Get<long>() && val2 != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilException(name+" expects integral types, but the second value type was " + val2, Stack);
            }

            if (val1 != val2)
            {
                throw new SigilException(name+" expects the same types for values, found " + val1 + " and " + val2, Stack);
            }

            UpdateState(op, val1, pop: 2);
        }

        public void And()
        {
            VerifyAndBinaryBitwise("And", OpCodes.And);
        }

        public void Or()
        {
            VerifyAndBinaryBitwise("Or", OpCodes.Or);
        }

        public void Xor()
        {
            VerifyAndBinaryBitwise("Xor", OpCodes.Xor);
        }

        public void Not()
        {
            var onStack = Stack.Top();

            if (onStack == null)
            {
                throw new SigilException("Not expects a value to be on the stack, but it was empty", Stack);
            }

            var val = onStack[0];

            if (val != TypeOnStack.Get<int>() && val != TypeOnStack.Get<long>() && val != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilException("Not expects integral types, but found " + val, Stack);
            }

            UpdateState(OpCodes.Not, val, pop: 1);
        }
    }
}
