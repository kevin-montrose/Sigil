using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public void Switch(params Label[] labels)
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
                if (label.Owner != this)
                {
                    throw new ArgumentException(label + " is not owned by this Emit, and thus cannot be used");
                }
            }

            var onStack = Stack.Top();

            if (onStack == null)
            {
                throw new SigilException("Switch expected a value on the stack, but it was empty", Stack);
            }

            var val = onStack[0];

            if (val != TypeOnStack.Get<int>())
            {
                throw new SigilException("Switch expected an int on the stack, found " + val, Stack);
            }

            BufferedILGenerator.UpdateOpCodeDelegate update;
            UpdateState(OpCodes.Switch, labels.Select(l => l.LabelDel).ToArray(), out update, pop: 1);

            foreach (var label in labels)
            {
                Branches[Stack.Unique()] = Tuple.Create(label, IL.Index);
                BranchPatches[IL.Index] = Tuple.Create(label, update, OpCodes.Switch);
            }
        }
    }
}
