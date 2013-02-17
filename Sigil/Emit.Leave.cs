using Sigil.Impl;
using System;
using System.Collections.Generic;
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

            var popAll = new List<Type>();
            for (var i = 0; i < Stack.Count(); i++)
            {
                popAll.Add(typeof(WildcardType));
            }

            TrackersAtBranches[label] = CurrentVerifier.Clone();

            // Note that Leave *always* nuked the stack; nothing survies exiting an exception block
            Sigil.Impl.BufferedILGenerator.UpdateOpCodeDelegate update;
            UpdateState(OpCodes.Leave, label, new[] { new StackTransition(popAll, Type.EmptyTypes) }.Wrap("Leave"), out update, pop: Stack.Count());

            Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Leave);

            if (TrackersAtLabels.ContainsKey(label))
            {
                var partial = TrackersAtLabels[label];

                var verifyRes = partial.Incoming(CurrentVerifier);

                if (!verifyRes.Success)
                {
                    // TODO: Gotta do better than this, needs "what the hell happened" messaging
                    throw new Exception("Branch violates stack");
                }
            }

            CurrentVerifier = new VerifiableTracker(baseless: true);

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
