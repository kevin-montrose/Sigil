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
        public void CompareEqual()
        {
            var top = Stack.Top(2);

            if (top == null)
            {
                throw new SigilException("CompareEqual expects two values on the stack", Stack);
            }

            UpdateState(OpCodes.Ceq, TypeOnStack.Get<int>(), pop: 2);
        }

        public void CompareGreaterThan()
        {
            var top = Stack.Top(2);

            if (top == null)
            {
                throw new SigilException("CompareGreaterThan expects two values on the stack", Stack);
            }

            UpdateState(OpCodes.Cgt, TypeOnStack.Get<int>(), pop: 2);
        }

        public void UnsignedCompareGreaterThan()
        {
            var top = Stack.Top(2);

            if (top == null)
            {
                throw new SigilException("UnsignedCompareGreaterThan expects two values on the stack", Stack);
            }

            UpdateState(OpCodes.Cgt_Un, TypeOnStack.Get<int>(), pop: 2);
        }

        public void CompareInfinite()
        {
            var top = Stack.Top(2);

            if (top == null)
            {
                throw new SigilException("CompareInfinite expects two values on the stack", Stack);
            }

            var val1 = top[1];
            var val2 = top[0];

            if (val1 != TypeOnStack.Get<float>() && val1 != TypeOnStack.Get<double>())
            {
                throw new SigilException("CompareInfinite expects floating point values, first value was " + val1, Stack);
            }

            if (val2 != TypeOnStack.Get<float>() && val2 != TypeOnStack.Get<double>())
            {
                throw new SigilException("CompareInfinite expects floating point values, second value was " + val2, Stack);
            }

            UpdateState(OpCodes.Ckfinite, TypeOnStack.Get<int>(), pop: 2);
        }

        public void CompareLessThan()
        {
            var top = Stack.Top(2);

            if (top == null)
            {
                throw new SigilException("CompareLessThan expects two values on the stack", Stack);
            }

            UpdateState(OpCodes.Clt, TypeOnStack.Get<int>(), pop: 2);
        }

        public void UnsignedCompareLessThan()
        {
            var top = Stack.Top(2);

            if (top == null)
            {
                throw new SigilException("UnsignedCompareLessThan expects two values on the stack", Stack);
            }

            UpdateState(OpCodes.Clt_Un, TypeOnStack.Get<int>(), pop: 2);
        }
    }
}
