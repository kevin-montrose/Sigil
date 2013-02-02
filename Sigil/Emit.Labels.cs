using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        // Go through and slap *_S everywhere it's needed for branches
        private void PatchBranches()
        {
            foreach (var start in BranchPatches.Keys.OrderBy(o => o))
            {
                var item = BranchPatches[start];

                var label = item.Item1;
                var patcher = item.Item2;
                var originalOp = item.Item3;

                if (!Marks.ContainsKey(label))
                {
                    throw new SigilException("Usage of unmarked label " + label);
                }

                var stop = Marks[label].Item2;

                var distance = IL.ByteDistance(start, stop);

                if (distance >= sbyte.MinValue && distance <= sbyte.MaxValue)
                {
                    if (originalOp == OpCodes.Switch)
                    {
                        // No short form to be had
                        continue;
                    }

                    if (originalOp == OpCodes.Br)
                    {
                        patcher(OpCodes.Br_S);
                        continue;
                    }

                    if (originalOp == OpCodes.Beq)
                    {
                        patcher(OpCodes.Beq_S);
                        continue;
                    }

                    if (originalOp == OpCodes.Bne_Un)
                    {
                        patcher(OpCodes.Bne_Un_S);
                        continue;
                    }

                    if (originalOp == OpCodes.Bge)
                    {
                        patcher(OpCodes.Bge_S);
                        continue;
                    }

                    if (originalOp == OpCodes.Bge_Un)
                    {
                        patcher(OpCodes.Bge_Un_S);
                        continue;
                    }

                    if (originalOp == OpCodes.Bgt)
                    {
                        patcher(OpCodes.Bgt_S);
                        continue;
                    }

                    if (originalOp == OpCodes.Bgt_Un)
                    {
                        patcher(OpCodes.Bgt_Un_S);
                        continue;
                    }

                    if (originalOp == OpCodes.Ble)
                    {
                        patcher(OpCodes.Ble_S);
                        continue;
                    }

                    if (originalOp == OpCodes.Ble_Un)
                    {
                        patcher(OpCodes.Ble_Un_S);
                        continue;
                    }

                    if (originalOp == OpCodes.Blt)
                    {
                        patcher(OpCodes.Blt_S);
                        continue;
                    }

                    if (originalOp == OpCodes.Blt_Un)
                    {
                        patcher(OpCodes.Blt_Un_S);
                        continue;
                    }

                    if (originalOp == OpCodes.Brfalse)
                    {
                        patcher(OpCodes.Brfalse_S);
                        continue;
                    }

                    if (originalOp == OpCodes.Brtrue)
                    {
                        patcher(OpCodes.Brtrue_S);
                        continue;
                    }

                    if (originalOp == OpCodes.Leave)
                    {
                        patcher(OpCodes.Leave_S);
                        continue;
                    }

                    throw new Exception("Unexpected OpCode: " + originalOp);
                }
            }
        }

        /// <summary>
        /// Defines a new label.
        /// 
        /// This label can be used for branching, leave, and switch instructions.
        /// 
        /// A label must be marked exactly once after being defined, using the MarkLabel() method.        
        /// </summary>
        public Label DefineLabel(string name = null)
        {
            name = name ?? AutoNamer.Next(this, "_label");

            var label = IL.DefineLabel();

            var ret = new Label(this, label, name);

            UnusedLabels.Add(ret);
            UnmarkedLabels.Add(ret);

            return ret;
        }


        /// <summary>
        /// Marks a label in the instruction stream.
        /// 
        /// When branching, leaving, or switching with a label control will be transfered to where it was *marked* not defined.
        /// 
        /// Label's can only be marked once, and *must* be marked before creating a delegate.
        /// </summary>
        public void MarkLabel(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (label.Owner != this)
            {
                throw new ArgumentException("label is not owner by this Emit, and thus cannot be used");
            }

            if (!UnmarkedLabels.Contains(label))
            {
                throw new InvalidOperationException("label [" + label.Name + "] has already been marked, and cannot be marked a second time");
            }

            UnmarkedLabels.Remove(label);

            IL.MarkLabel(label.LabelDel);

            Marks[label] = Tuple.Create(Stack, IL.Index);
        }
    }
}
