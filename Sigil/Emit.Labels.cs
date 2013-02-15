using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

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

                if (!Marks.ContainsKey(label))
                {
                    throw new SigilVerificationException("Usage of unmarked label " + label, IL.Instructions(LocalsByIndex));
                }

                var stop = Marks[label].Item2;

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
            name = name ?? AutoNamer.Next(this, "_label");

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
        /// Label's can only be marked once, and *must* be marked before creating a delegate.
        /// 
        /// Logically after a Branch or Leave instruction, a stack assertion is required to continue emiting.  The stack
        /// is assumed to match that state in these cases.
        /// 
        /// In all other cases, a stack assertion is merely checked (and if failing, a verification exception is thrown).
        /// 
        /// In the assertion, the top of the stack is the first (0-indexed, left-most) parameter.
        /// </summary>
        public Emit<DelegateType> MarkLabel(Label label, IEnumerable<Type> stackAssertion = null)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (RequireTypeAssertion && stackAssertion == null)
            {
                throw new ArgumentNullException("stackAssertion must be set when marking a label after a Branch or Leave instruction");
            }

            if (((IOwned)label).Owner != this)
            {
                FailOwnership(label);
            }

            if (!UnmarkedLabels.Contains(label))
            {
                throw new InvalidOperationException("label [" + label.Name + "] has already been marked, and cannot be marked a second time");
            }

            if (RequireTypeAssertion)
            {
                var newStack = new StackState();
                foreach (var x in stackAssertion.Reverse())
                {
                    newStack = newStack.Push(TypeOnStack.Get(x.Alias()));
                }

                Stack = newStack;

                RequireTypeAssertion = false;
            }
            else
            {
                if (stackAssertion != null)
                {
                    var onStack = Stack.Top(stackAssertion.Count());

                    if (onStack == null)
                    {
                        FailStackUnderflow(onStack.Count());
                    }

                    for (var i = 0; i < onStack.Length; i++)
                    {
                        var shouldBe = TypeOnStack.Get(stackAssertion.ElementAt(i).Alias());
                        var actuallyIs = onStack[i];

                        // strict equality, assignment clouds things for debugging
                        if (shouldBe != actuallyIs)
                        {
                            throw new SigilVerificationException("Assertion failed expected " + shouldBe + ", but found " + actuallyIs, IL.Instructions(LocalsByIndex), Stack, i);
                        }
                    }
                }
            }

            UnmarkedLabels.Remove(label);

            IL.MarkLabel(label);

            Marks[label] = Tuple.Create(Stack, IL.Index);

            return this;
        }
    }
}
