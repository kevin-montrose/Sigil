using Sigil.Impl;
using System;
using System.Linq;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Leave an exception or catch block, branching to the given label.
        /// 
        /// This instruction empties the stack.
        /// </summary>
        public Emit<DelegateType> Leave(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (((IOwned)label).Owner != this)
            {
                FailOwnership(label);
            }

            if (!TryBlocks.Any(t => t.Value.Item2 == -1) && !CatchBlocks.Any(c => c.Value.Item2 == -1))
            {
                throw new InvalidOperationException("Leave can only be used within an exception or catch block");
            }

            CurrentVerifier.Branch(label);

            // Note that Leave *always* nuked the stack; nothing survies exiting an exception block
            UpdateOpCodeDelegate update;
            UpdateState(OpCodes.Leave, label, new[] { new StackTransition(new [] { typeof(PopAllType) }, Type.EmptyTypes) }.Wrap("Leave"), out update);

            CheckBranchesAndLabels("Leave", label);

            Branches.Add(Tuple.Create(label, IL.Index));

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Leave);

            CurrentVerifier = null;
            MustMark = true;

            return this;
        }

        /// <summary>
        /// Leave an exception or catch block, branching to the label with the given name.
        /// 
        /// This instruction empties the stack.
        /// </summary>
        public Emit<DelegateType> Leave(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            return Leave(Labels[name]);
        }
    }
}
