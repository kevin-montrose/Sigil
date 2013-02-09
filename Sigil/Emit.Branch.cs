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
        /// <summary>
        /// Unconditionally branches to the given label.
        /// </summary>
        public Emit<DelegateType> Branch(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (label.Owner != this)
            {
                throw new ArgumentException("label is not owned by this Emit, and thus cannot be used");
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Br, label, out update);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Br);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, if both are equal branches to the given label.
        /// </summary>
        public Emit<DelegateType> BranchIfEqual(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (label.Owner != this)
            {
                throw new ArgumentException("label is not owned by this Emit, and thus cannot be used");
            }

            var top = Stack.Top(2);

            if (top == null)
            {
                FailStackUnderflow(2);
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Beq, label, out update, pop: 2);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Beq);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, if they are not equal (when treated as unsigned values) branches to the given label.
        /// </summary>
        public Emit<DelegateType> UnsignedBranchIfNotEqual(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (label.Owner != this)
            {
                throw new ArgumentException("label is not owned by this Emit, and thus cannot be used");
            }

            var top = Stack.Top(2);

            if (top == null)
            {
                FailStackUnderflow(2);
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Bne_Un, label, out update, pop: 2);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Bne_Un);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is greater than or equal to the first value.
        /// </summary>
        public Emit<DelegateType> BranchIfGreaterOrEqual(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (label.Owner != this)
            {
                throw new ArgumentException("label is not owned by this Emit, and thus cannot be used");
            }

            var top = Stack.Top(2);

            if (top == null)
            {
                FailStackUnderflow(2);
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Bge, label, out update, pop: 2);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Bge);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is greater than or equal to the first value (when treated as unsigned values).
        /// </summary>
        public Emit<DelegateType> UnsignedBranchIfGreaterOrEqual(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (label.Owner != this)
            {
                throw new ArgumentException("label is not owned by this Emit, and thus cannot be used");
            }

            var top = Stack.Top(2);

            if (top == null)
            {
                FailStackUnderflow(2);
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Bge_Un, label, out update, pop: 2);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Bge_Un);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is greater than the first value.
        /// </summary>
        public Emit<DelegateType> BranchIfGreater(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (label.Owner != this)
            {
                throw new ArgumentException("label is not owned by this Emit, and thus cannot be used");
            }

            var top = Stack.Top(2);

            if (top == null)
            {
                FailStackUnderflow(2);
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Bgt, label, out update, pop: 2);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Bgt);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is greater than the first value (when treated as unsigned values).
        /// </summary>
        public Emit<DelegateType> UnsignedBranchIfGreater(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (label.Owner != this)
            {
                throw new ArgumentException("label is not owned by this Emit, and thus cannot be used");
            }

            var top = Stack.Top(2);

            if (top == null)
            {
                FailStackUnderflow(2);
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Bgt_Un, label, out update, pop: 2);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Bgt_Un);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is less than or equal to the first value.
        /// </summary>
        public Emit<DelegateType> BranchIfLessOrEqual(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (label.Owner != this)
            {
                throw new ArgumentException("label is not owned by this Emit, and thus cannot be used");
            }

            var top = Stack.Top(2);

            if (top == null)
            {
                FailStackUnderflow(2);
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Ble, label, out update, pop: 2);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Ble);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is less than or equal to the first value (when treated as unsigned values).
        /// </summary>
        public Emit<DelegateType> UnsignedBranchIfLessOrEqual(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (label.Owner != this)
            {
                throw new ArgumentException("label is not owned by this Emit, and thus cannot be used");
            }

            var top = Stack.Top(2);

            if (top == null)
            {
                FailStackUnderflow(2);
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Ble_Un, label, out update, pop: 2);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Ble_Un);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is less than the first value.
        /// </summary>
        public Emit<DelegateType> BranchIfLess(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (label.Owner != this)
            {
                throw new ArgumentException("label is not owned by this Emit, and thus cannot be used");
            }

            var top = Stack.Top(2);

            if (top == null)
            {
                FailStackUnderflow(2);
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Blt, label, out update, pop: 2);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Blt);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is less than the first value (when treated as unsigned values).
        /// </summary>
        public Emit<DelegateType> UnsignedBranchIfLess(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (label.Owner != this)
            {
                throw new ArgumentException("label is not owned by this Emit, and thus cannot be used");
            }

            var top = Stack.Top(2);

            if (top == null)
            {
                FailStackUnderflow(2);
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Blt_Un, label, out update, pop: 2);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Blt_Un);

            return this;
        }

        /// <summary>
        /// Pops one argument from the stack, branches to the given label if the value is false.
        /// 
        /// A value is false if it is zero or null.
        /// </summary>
        public Emit<DelegateType> BranchIfFalse(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (label.Owner != this)
            {
                throw new ArgumentException("label is not owned by this Emit, and thus cannot be used");
            }

            var top = Stack.Top(1);

            if (top == null)
            {
                FailStackUnderflow(1);
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Brfalse, label, out update, pop: 1);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Brfalse);

            return this;
        }

        /// <summary>
        /// Pops one argument from the stack, branches to the given label if the value is true.
        /// 
        /// A value is true if it is non-zero or non-null.
        /// </summary>
        public Emit<DelegateType> BranchIfTrue(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (label.Owner != this)
            {
                throw new ArgumentException("label is not owned by this Emit, and thus cannot be used");
            }

            var top = Stack.Top(1);

            if (top == null)
            {
                FailStackUnderflow(1);
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Brtrue, label, out update, pop: 1);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Brtrue);

            return this;
        }
    }
}
