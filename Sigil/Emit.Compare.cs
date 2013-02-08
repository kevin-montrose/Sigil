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
        private void ValidateComparable(string method, TypeOnStack t1, TypeOnStack t2)
        {
            if (t1 != t2)
            {
                throw new SigilVerificationException(method + " expected two comparable values of the same type, instead found " + t1 + " and " + t2, IL, Stack);
            }

            if (!t1.Type.IsPrimitive)
            {
                throw new SigilVerificationException(method + " expected primitive types, instead found " + t1, IL, Stack);
            }
        }

        /// <summary>
        /// Pops two values from the stack, and pushes a 1 if they are equal and 0 if they are not.
        /// 
        /// New value on the stack is an Int32.
        /// </summary>
        public Emit<DelegateType> CompareEqual()
        {
            var top = Stack.Top(2);

            if (top == null)
            {
                throw new SigilVerificationException("CompareEqual expects two values on the stack", IL, Stack);
            }

            UpdateState(OpCodes.Ceq, TypeOnStack.Get<int>(), pop: 2);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, pushes a 1 if the second value is greater than the first value and a 0 otherwise.
        /// 
        /// New value on the stack is an Int32.
        /// </summary>
        public Emit<DelegateType> CompareGreaterThan()
        {
            var top = Stack.Top(2);

            if (top == null)
            {
                throw new SigilVerificationException("CompareGreaterThan expects two values on the stack", IL, Stack);
            }

            ValidateComparable("CompareGreaterThan", top.First(), top.Last());

            UpdateState(OpCodes.Cgt, TypeOnStack.Get<int>(), pop: 2);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, pushes a 1 if the second value is greater than the first value (as unsigned values) and a 0 otherwise.
        /// 
        /// New value on the stack is an Int32.
        /// </summary>
        public Emit<DelegateType> UnsignedCompareGreaterThan()
        {
            var top = Stack.Top(2);

            if (top == null)
            {
                throw new SigilVerificationException("UnsignedCompareGreaterThan expects two values on the stack", IL, Stack);
            }

            ValidateComparable("UnsignedCompareGreaterThan", top.First(), top.Last());

            UpdateState(OpCodes.Cgt_Un, TypeOnStack.Get<int>(), pop: 2);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, pushes a 1 if the second value is less than the first value and a 0 otherwise.
        /// 
        /// New value on the stack is an Int32.
        /// </summary>
        public Emit<DelegateType> CompareLessThan()
        {
            var top = Stack.Top(2);

            if (top == null)
            {
                throw new SigilVerificationException("CompareLessThan expects two values on the stack", IL, Stack);
            }

            ValidateComparable("CompareLessThan", top.First(), top.Last());

            UpdateState(OpCodes.Clt, TypeOnStack.Get<int>(), pop: 2);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, pushes a 1 if the second value is less than the first value (as unsigned values) and a 0 otherwise.
        /// 
        /// New value on the stack is an Int32.
        /// </summary>
        public Emit<DelegateType> UnsignedCompareLessThan()
        {
            var top = Stack.Top(2);

            if (top == null)
            {
                throw new SigilVerificationException("UnsignedCompareLessThan expects two values on the stack", IL, Stack);
            }

            ValidateComparable("UnsignedCompareLessThan", top.First(), top.Last());

            UpdateState(OpCodes.Clt_Un, TypeOnStack.Get<int>(), pop: 2);

            return this;
        }
    }
}
