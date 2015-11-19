using Sigil.Impl;
using System;
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
                if (((IOwned)label).Owner is DisassembledOperations<DelegateType>)
                {
                    return Leave(label.Name);
                }

                FailOwnership(label);
            }

            if (!TryBlocks.Any(t => t.Value.Item2 == -1) && !CatchBlocks.Any(c => c.Value.Item2 == -1))
            {
                throw new InvalidOperationException("Leave can only be used within an exception or catch block");
            }

            // Note that Leave *always* nuked the stack; nothing survies exiting an exception block
            UpdateOpCodeDelegate update;
            UpdateState(OpCodes.Leave, label, Wrap(new[] { new StackTransition(new [] { typeof(PopAllType) }, TypeHelpers.EmptyTypes) }, "Leave"), out update);

            Branches.Add(SigilTuple.Create(OpCodes.Leave, label, IL.Index));

            BranchPatches[IL.Index] = SigilTuple.Create(label, update, OpCodes.Leave);
            MustMark = true;

            var valid = CurrentVerifiers.UnconditionalBranch(label);
            if (!valid.Success)
            {
                throw new SigilVerificationException("Leave", valid, IL.Instructions(AllLocals));
            }

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
