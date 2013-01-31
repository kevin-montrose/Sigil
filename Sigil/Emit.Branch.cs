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
        public void Branch(Label label)
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

            UpdateState(OpCodes.Br, label.LabelDel, out update);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Br);
        }

        /// <summary>
        /// Pops two arguments from the stack, if both are equal branches to the given label.
        /// </summary>
        public void BranchIfEqual(Label label)
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
                throw new SigilException("BranchIfEqual expects two values to be on the stack", Stack);
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Beq, label.LabelDel, out update, pop: 2);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Beq);
        }

        /// <summary>
        /// Pops two arguments from the stack, if they are not equal (when treated as unsigned values) branches to the given label.
        /// </summary>
        public void UnsignedBranchIfNotEqual(Label label)
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
                throw new SigilException("BranchIfNotEqual expects two values to be on the stack", Stack);
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Bne_Un, label.LabelDel, out update, pop: 2);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Bne_Un);
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is greater than or equal to the first value.
        /// </summary>
        public void BranchIfGreaterOrEqual(Label label)
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
                throw new SigilException("BranchIfGreaterOrEqual expects two values to be on the stack", Stack);
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Bge, label.LabelDel, out update, pop: 2);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Bge);
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is greater than or equal to the first value (when treated as unsigned values).
        /// </summary>
        public void UnsignedBranchIfGreaterOrEqual(Label label)
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
                throw new SigilException("UnsignedBranchIfGreaterOrEqual expects two values to be on the stack", Stack);
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Bge_Un, label.LabelDel, out update, pop: 2);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Bge_Un);
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is greater than the first value.
        /// </summary>
        public void BranchIfGreater(Label label)
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
                throw new SigilException("BranchIfGreater expects two values to be on the stack", Stack);
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Bgt, label.LabelDel, out update, pop: 2);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Bgt);
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is greater than the first value (when treated as unsigned values).
        /// </summary>
        public void UnsignedBranchIfGreater(Label label)
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
                throw new SigilException("UnsignedBranchIfGreater expects two values to be on the stack", Stack);
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Bgt_Un, label.LabelDel, out update, pop: 2);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Bgt_Un);
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is less than or equal to the first value.
        /// </summary>
        public void BranchIfLessOrEqual(Label label)
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
                throw new SigilException("BranchIfLessOrEqual expects two values to be on the stack", Stack);
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Ble, label.LabelDel, out update, pop: 2);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Ble);
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is less than or equal to the first value (when treated as unsigned values).
        /// </summary>
        public void UnsignedBranchIfLessOrEqual(Label label)
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
                throw new SigilException("UnsignedBranchIfLessOrEqual expects two values to be on the stack", Stack);
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Ble_Un, label.LabelDel, out update, pop: 2);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Ble_Un);
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is less than the first value.
        /// </summary>
        public void BranchIfLess(Label label)
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
                throw new SigilException("BranchIfLess expects two values to be on the stack", Stack);
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Blt, label.LabelDel, out update, pop: 2);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Blt);
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is less than the first value (when treated as unsigned values).
        /// </summary>
        public void UnsignedBranchIfLess(Label label)
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
                throw new SigilException("UnsignedBranchIfLess expects two values to be on the stack", Stack);
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Blt_Un, label.LabelDel, out update, pop: 2);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Blt_Un);
        }

        /// <summary>
        /// Pops one argument from the stack, branches to the given label if the value is false.
        /// 
        /// A value is false if it is zero or null.
        /// </summary>
        public void BranchIfFalse(Label label)
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
                throw new SigilException("BranchIfFalse expects one value to be on the stack", Stack);
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Brfalse, label.LabelDel, out update, pop: 1);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Brfalse);
        }

        /// <summary>
        /// Pops one argument from the stack, branches to the given label if the value is true.
        /// 
        /// A value is true if it is non-zero or non-null.
        /// </summary>
        public void BranchIfTrue(Label label)
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
                throw new SigilException("BranchIfTrue expects one value to be on the stack", Stack);
            }

            UnusedLabels.Remove(label);

            BufferedILGenerator.UpdateOpCodeDelegate update;

            UpdateState(OpCodes.Brtrue, label.LabelDel, out update, pop: 1);

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Brtrue);
        }
    }
}
