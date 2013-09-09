
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Pops two values from the stack, and pushes a 1 if they are equal and 0 if they are not.
        /// 
        /// New value on the stack is an Int32.
        /// </summary>
        public Emit CompareEqual()
        {
            InnerEmit.CompareEqual();
            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, pushes a 1 if the second value is greater than the first value and a 0 otherwise.
        /// 
        /// New value on the stack is an Int32.
        /// </summary>
        public Emit CompareGreaterThan()
        {
            InnerEmit.CompareGreaterThan();
            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, pushes a 1 if the second value is greater than the first value (as unsigned values) and a 0 otherwise.
        /// 
        /// New value on the stack is an Int32.
        /// </summary>
        public Emit UnsignedCompareGreaterThan()
        {
            InnerEmit.UnsignedCompareGreaterThan();
            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, pushes a 1 if the second value is less than the first value and a 0 otherwise.
        /// 
        /// New value on the stack is an Int32.
        /// </summary>
        public Emit CompareLessThan()
        {
            InnerEmit.CompareLessThan();
            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, pushes a 1 if the second value is less than the first value (as unsigned values) and a 0 otherwise.
        /// 
        /// New value on the stack is an Int32.
        /// </summary>
        public Emit UnsignedCompareLessThan()
        {
            InnerEmit.UnsignedCompareLessThan();
            return this;
        }
    }
}