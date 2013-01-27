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
        public void Leave(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (label.Owner != this)
            {
                throw new ArgumentException("label is not owned by this Emit, and thus cannot be used");
            }

            if (!TryBlocks.Any(t => t.Value.Item2 == -1) && !CatchBlocks.Any(c => c.Value.Item2 == -1))
            {
                throw new SigilException("Leave can only be used within an exception or catch block", Stack);
            }

            // Note that Leave *always* nuked the stack; nothing survies exiting an exception block
            Sigil.Impl.BufferedILGenerator.UpdateOpCodeDelegate update;
            UpdateState(OpCodes.Leave, label.LabelDel, out update, pop: Stack.Count());

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Leave);
        }
    }
}
