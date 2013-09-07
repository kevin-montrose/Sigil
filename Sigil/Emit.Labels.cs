using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        // Go through and slap *_S everywhere it's needed for branches
        private void PatchBranches()
        {
            foreach (var start in BranchPatches.Keys.OrderBy(o => o).AsEnumerable())
            {
                var item = BranchPatches[start];

                var label = item.Item1;
                var patcher = item.Item2;
                var originalOp = item.Item3;

                if (!Marks.ContainsKey(label))
                {
                    throw new SigilVerificationException("Usage of unmarked label " + label, IL.Instructions(AllLocals));
                }

                var stop = Marks[label];

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
            name = name ?? AutoNamer.Next(this, "_label", Locals.Names, Labels.Names);

            if (CurrentLabels.ContainsKey(name))
            {
                throw new InvalidOperationException("Label with name '" + name + "' already exists");
            }

            var label = IL.DefineLabel();

            var ret = new Label(this, label, name);

            UnusedLabels.Add(ret);
            UnmarkedLabels.Add(ret);

            CurrentLabels[name] = ret;

            return ret;
        }

        /// <summary>
        /// Defines a new label.
        /// 
        /// This label can be used for branching, leave, and switch instructions.
        /// 
        /// A label must be marked exactly once after being defined, using the MarkLabel() method.        
        /// </summary>
        public Emit<DelegateType> DefineLabel(out Label label, string name = null)
        {
            label = DefineLabel(name);

            return this;
        }


        /// <summary>
        /// Marks a label in the instruction stream.
        /// 
        /// When branching, leaving, or switching with a label control will be transfered to where it was *marked* not defined.
        /// 
        /// Labels can only be marked once, and *must* be marked before creating a delegate.
        /// </summary>
        public Emit<DelegateType> MarkLabel(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (((IOwned)label).Owner != this)
            {
                if (((IOwned)label).Owner is DisassembledOperations<DelegateType>)
                {
                    return MarkLabel(label.Name);
                }

                FailOwnership(label);
            }

            if (!UnmarkedLabels.Contains(label))
            {
                throw new InvalidOperationException("label [" + label.Name + "] has already been marked, and cannot be marked a second time");
            }

            if (MustMark)
            {
                MustMark = false;
            }

            var valid = CurrentVerifiers.Mark(label);
            if (!valid.Success)
            {
                throw new SigilVerificationException("MarkLabel", valid, IL.Instructions(AllLocals));
            }

            UnmarkedLabels.Remove(label);

            IL.MarkLabel(label);

            Marks[label] = IL.Index;

            return this;
        }

        /// <summary>
        /// Marks a label with the given name in the instruction stream.
        /// 
        /// When branching, leaving, or switching with a label control will be transfered to where it was *marked* not defined.
        /// 
        /// Labels can only be marked once, and *must* be marked before creating a delegate.
        /// </summary>
        public Emit<DelegateType> MarkLabel(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            return MarkLabel(Labels[name]);
        }
    }
}
