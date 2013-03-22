using Sigil.Impl;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        private TransitionWrapper ValidateComparable(string method)
        {
            return
                Wrap(
                    new[]
                    {
                        new StackTransition(new [] { typeof(int), typeof(int) }, new [] { typeof(int) }),
                        new StackTransition(new [] { typeof(NativeIntType), typeof(NativeIntType) }, new [] { typeof(int) }),
                        new StackTransition(new [] { typeof(long), typeof(long) }, new [] { typeof(int) }),
                        new StackTransition(new [] { typeof(float), typeof(float) }, new [] { typeof(int) }),
                        new StackTransition(new [] { typeof(double), typeof(double) }, new [] { typeof(int) }),
                    },
                    method
                );
        }

        /// <summary>
        /// Pops two values from the stack, and pushes a 1 if they are equal and 0 if they are not.
        /// 
        /// New value on the stack is an Int32.
        /// </summary>
        public Emit<DelegateType> CompareEqual()
        {
            var transitions =
                new[]
                {
                    new StackTransition(new [] { typeof(WildcardType), typeof(WildcardType) }, new [] { typeof(int) })
                };

            UpdateState(OpCodes.Ceq, Wrap(transitions, "CompareEqual"));

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, pushes a 1 if the second value is greater than the first value and a 0 otherwise.
        /// 
        /// New value on the stack is an Int32.
        /// </summary>
        public Emit<DelegateType> CompareGreaterThan()
        {
            var transitions = ValidateComparable("CompareGreaterThan");

            UpdateState(OpCodes.Cgt, transitions);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, pushes a 1 if the second value is greater than the first value (as unsigned values) and a 0 otherwise.
        /// 
        /// New value on the stack is an Int32.
        /// </summary>
        public Emit<DelegateType> UnsignedCompareGreaterThan()
        {
            var transitions = ValidateComparable("UnsignedCompareGreaterThan");

            UpdateState(OpCodes.Cgt_Un, transitions);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, pushes a 1 if the second value is less than the first value and a 0 otherwise.
        /// 
        /// New value on the stack is an Int32.
        /// </summary>
        public Emit<DelegateType> CompareLessThan()
        {
            var transitions = ValidateComparable("CompareLessThan");

            UpdateState(OpCodes.Clt, transitions);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, pushes a 1 if the second value is less than the first value (as unsigned values) and a 0 otherwise.
        /// 
        /// New value on the stack is an Int32.
        /// </summary>
        public Emit<DelegateType> UnsignedCompareLessThan()
        {
            var transitions = ValidateComparable("UnsignedCompareLessThan");

            UpdateState(OpCodes.Clt_Un, transitions);

            return this;
        }
    }
}