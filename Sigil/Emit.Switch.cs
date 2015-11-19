using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pops a value off the stack and branches to the label at the index of that value in the given labels.
        /// 
        /// If the value is out of range, execution falls through to the next instruction.
        /// </summary>
        public Emit<DelegateType> Switch(params Label[] labels)
        {
            if (labels == null)
            {
                throw new ArgumentNullException("labels");
            }

            if (labels.Length == 0)
            {
                throw new ArgumentException("labels must have at least one element");
            }

            if (LinqAlternative.Any(labels, l => ((IOwned)l).Owner is DisassembledOperations<DelegateType>))
            {
                return
                    Switch(LinqAlternative.Select(labels, l => l.Name).ToArray());
            }

            foreach (var label in labels)
            {
                if (((IOwned)label).Owner != this)
                {
                    FailOwnership(label);
                }
            }

            foreach (var label in labels)
            {
                UnusedLabels.Remove(label);
            }

            var transitions =
                new[]
                {
                    new StackTransition(new [] { typeof(int) }, TypeHelpers.EmptyTypes),
                    new StackTransition(new [] { typeof(NativeIntType) }, TypeHelpers.EmptyTypes),
                };

            var labelsCopy = ((LinqArray<Label>)labels).Select(l => l).ToArray();

            UpdateOpCodeDelegate update;
            UpdateState(OpCodes.Switch, labelsCopy, Wrap(transitions, "Switch"), out update);

            var valid = CurrentVerifiers.ConditionalBranch(labels);
            if (!valid.Success)
            {
                throw new SigilVerificationException("Switch", valid, IL.Instructions(AllLocals));
            }

            foreach (var label in labels)
            {
                Branches.Add(SigilTuple.Create(OpCodes.Switch, label, IL.Index));
                BranchPatches[IL.Index] = SigilTuple.Create(label, update, OpCodes.Switch);
            }

            return this;
        }

        /// <summary>
        /// Pops a value off the stack and branches to the label at the index of that value in the given label names.
        /// 
        /// If the value is out of range, execution falls through to the next instruction.
        /// </summary>
        public Emit<DelegateType> Switch(params string[] names)
        {
            if (names == null) throw new ArgumentNullException("names");

            var lNames = (LinqArray<string>)names;

            if (lNames.Any(n => n == null)) throw new ArgumentException("no label can be null");

            return Switch(lNames.Select(n => Labels[n]).ToArray());
        }
    }
}
