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

            // Note that Leave *always* nuked the stack; nothing survies exiting an exception block
            Sigil.Impl.BufferedILGenerator.UpdateOpCodeDelegate update;
            UpdateState(OpCodes.Leave, label, out update, pop: Stack.Count());

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Leave);

            RequireTypeAssertion = true;

            return this;
        }

        /// <summary>
        /// Leave an exception or catch block, branching to the label with the given name.
        /// 
        /// This instruction empties the stack.
        /// </summary>
        public Emit<DelegateType> Leave(string name)
        {
            return Leave(Labels[name]);
        }
    }
}
