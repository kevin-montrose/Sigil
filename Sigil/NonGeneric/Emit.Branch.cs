
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Unconditionally branches to the given label.
        /// </summary>
        public Emit Branch(Label label)
        {
            InnerEmit.Branch(label);
            return this;
        }

        /// <summary>
        /// Unconditionally branches to the label with the given name.
        /// </summary>
        public Emit Branch(string name)
        {
            InnerEmit.Branch(name);
            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, if both are equal branches to the given label.
        /// </summary>
        public Emit BranchIfEqual(Label label)
        {
            InnerEmit.BranchIfEqual(label);
            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, if both are equal branches to the label with the given name.
        /// </summary>
        public Emit BranchIfEqual(string name)
        {
            InnerEmit.BranchIfEqual(name);
            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, if they are not equal (when treated as unsigned values) branches to the given label.
        /// </summary>
        public Emit UnsignedBranchIfNotEqual(Label label)
        {
            InnerEmit.UnsignedBranchIfNotEqual(label);
            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, if they are not equal (when treated as unsigned values) branches to the label with the given name.
        /// </summary>
        public Emit UnsignedBranchIfNotEqual(string name)
        {
            InnerEmit.UnsignedBranchIfNotEqual(name);
            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is greater than or equal to the first value.
        /// </summary>
        public Emit BranchIfGreaterOrEqual(Label label)
        {
            InnerEmit.BranchIfGreaterOrEqual(label);
            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the label with the given name if the second value is greater than or equal to the first value.
        /// </summary>
        public Emit BranchIfGreaterOrEqual(string name)
        {
            InnerEmit.BranchIfGreaterOrEqual(name);
            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is greater than or equal to the first value (when treated as unsigned values).
        /// </summary>
        public Emit UnsignedBranchIfGreaterOrEqual(Label label)
        {
            InnerEmit.UnsignedBranchIfGreaterOrEqual(label);
            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the label with the given name if the second value is greater than or equal to the first value (when treated as unsigned values).
        /// </summary>
        public Emit UnsignedBranchIfGreaterOrEqual(string name)
        {
            InnerEmit.UnsignedBranchIfGreaterOrEqual(name);
            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is greater than the first value.
        /// </summary>
        public Emit BranchIfGreater(Label label)
        {
            InnerEmit.BranchIfGreater(label);
            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the label with the given name if the second value is greater than the first value.
        /// </summary>
        public Emit BranchIfGreater(string name)
        {
            InnerEmit.BranchIfGreater(name);
            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is greater than the first value (when treated as unsigned values).
        /// </summary>
        public Emit UnsignedBranchIfGreater(Label label)
        {
            InnerEmit.UnsignedBranchIfGreater(label);
            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the label with the given name if the second value is greater than the first value (when treated as unsigned values).
        /// </summary>
        public Emit UnsignedBranchIfGreater(string name)
        {
            InnerEmit.UnsignedBranchIfGreater(name);
            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is less than or equal to the first value.
        /// </summary>
        public Emit BranchIfLessOrEqual(Label label)
        {
            InnerEmit.BranchIfLessOrEqual(label);
            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the label with the given name if the second value is less than or equal to the first value.
        /// </summary>
        public Emit BranchIfLessOrEqual(string name)
        {
            InnerEmit.BranchIfLessOrEqual(name);
            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is less than or equal to the first value (when treated as unsigned values).
        /// </summary>
        public Emit UnsignedBranchIfLessOrEqual(Label label)
        {
            InnerEmit.UnsignedBranchIfLessOrEqual(label);
            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the label with the given name if the second value is less than or equal to the first value (when treated as unsigned values).
        /// </summary>
        public Emit UnsignedBranchIfLessOrEqual(string name)
        {
            InnerEmit.UnsignedBranchIfLessOrEqual(name);
            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is less than the first value.
        /// </summary>
        public Emit BranchIfLess(Label label)
        {
            InnerEmit.BranchIfLess(label);
            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the label with the given name if the second value is less than the first value.
        /// </summary>
        public Emit BranchIfLess(string name)
        {
            InnerEmit.BranchIfLess(name);
            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is less than the first value (when treated as unsigned values).
        /// </summary>
        public Emit UnsignedBranchIfLess(Label label)
        {
            InnerEmit.UnsignedBranchIfLess(label);
            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the label with the given name if the second value is less than the first value (when treated as unsigned values).
        /// </summary>
        public Emit UnsignedBranchIfLess(string name)
        {
            InnerEmit.UnsignedBranchIfLess(name);
            return this;
        }

        /// <summary>
        /// Pops one argument from the stack, branches to the given label if the value is false.
        /// 
        /// A value is false if it is zero or null.
        /// </summary>
        public Emit BranchIfFalse(Label label)
        {
            InnerEmit.BranchIfFalse(label);
            return this;
        }

        /// <summary>
        /// Pops one argument from the stack, branches to the label with the given name if the value is false.
        /// 
        /// A value is false if it is zero or null.
        /// </summary>
        public Emit BranchIfFalse(string name)
        {
            InnerEmit.BranchIfFalse(name);
            return this;
        }

        /// <summary>
        /// Pops one argument from the stack, branches to the given label if the value is true.
        /// 
        /// A value is true if it is non-zero or non-null.
        /// </summary>
        public Emit BranchIfTrue(Label label)
        {
            InnerEmit.BranchIfTrue(label);
            return this;
        }

        /// <summary>
        /// Pops one argument from the stack, branches to the label with the given name if the value is true.
        /// 
        /// A value is true if it is non-zero or non-null.
        /// </summary>
        public Emit BranchIfTrue(string name)
        {
            InnerEmit.BranchIfTrue(name);
            return this;
        }
    }
}
