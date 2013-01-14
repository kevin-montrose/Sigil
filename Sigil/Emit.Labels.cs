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

                var stop = Marks[label].Item2;

                var distance = IL.ByteDistance(start, stop);

                if (distance >= sbyte.MinValue && distance <= sbyte.MaxValue)
                {
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

                    throw new Exception("Unexpected OpCode: " + originalOp);
                }
            }

            // We can't use this data again, so nuke it
            BranchPatches = null;
        }

        public EmitLabel CreateLabel()
        {
            return CreateLabel("_" + Guid.NewGuid().ToString().Replace("-", ""));
        }

        public EmitLabel CreateLabel(string name)
        {
            var label = IL.DefineLabel();

            var ret = new EmitLabel(this, label, name);

            UnusedLabels.Add(ret);
            UnmarkedLabels.Add(ret);

            return ret;
        }

        public void MarkLabel(EmitLabel label)
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
                throw new SigilException("label [" + label.Name + "] has already been marked, and cannot be marked a second time", Stack);
            }

            UnmarkedLabels.Remove(label);

            IL.MarkLabel(label.Label);

            Marks[label] = Tuple.Create(Stack, IL.Index);
        }
    }
}
