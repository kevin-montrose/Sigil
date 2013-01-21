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

            Branches[Stack] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Br);
        }

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

            Branches[Stack] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Beq);
        }

        public void BranchIfNotEqual(Label label)
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

            Branches[Stack] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Bne_Un);
        }

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

            Branches[Stack] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Bge);
        }

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

            Branches[Stack] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Bge_Un);
        }

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

            Branches[Stack] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Bgt);
        }

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

            Branches[Stack] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Bgt_Un);
        }

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

            Branches[Stack] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Ble);
        }

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

            Branches[Stack] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Ble_Un);
        }

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

            Branches[Stack] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Blt);
        }

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

            Branches[Stack] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Blt_Un);
        }

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

            Branches[Stack] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Brfalse);
        }

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

            Branches[Stack] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Brtrue);
        }
    }
}
