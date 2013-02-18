using Sigil.Impl;
using System;
using System.Linq;
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

            var onStack = Stack.Top();

            if (onStack == null)
            {
                FailStackUnderflow(1);
            }

            /*var val = onStack[0];

            if (val != TypeOnStack.Get<int>())
            {
                throw new SigilVerificationException("Switch expected an int on the stack, found " + val, IL.Instructions(LocalsByIndex), Stack, 0);
            }*/

            var transitions =
                new[]
                {
                    new StackTransition(new [] { typeof(int) }, Type.EmptyTypes),
                    new StackTransition(new [] { typeof(NativeIntType) }, Type.EmptyTypes),
                };

            BufferedILGenerator.UpdateOpCodeDelegate update;
            UpdateState(OpCodes.Switch, labels.Select(l => l).ToArray(), transitions.Wrap("Switch"), out update, pop: 1);

            foreach (var label in labels)
            {
                Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);
                BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Switch);
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

            if (names.Any(n => n == null)) throw new ArgumentException("no label can be null");

            return Switch(names.Select(n => Labels[n]).ToArray());
        }
    }
}
