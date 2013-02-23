using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Sigil.Impl
{
    internal class VerifiableTracker
    {
        // When the stack is "unbased" or "baseless", underflowing it results in wildcards
        //   eventually they'll be fixed up to actual types
        private bool Baseless;
        private List<InstructionAndTransitions> Transitions = new List<InstructionAndTransitions>();

        public VerifiableTracker(bool baseless = false) { Baseless = baseless; }

        public VerificationResult Transition(InstructionAndTransitions legalTransitions)
        {
            Transitions.Add(legalTransitions);
            var ret = CollapseAndVerify();

            // revert!
            if (!ret.Success)
            {
                Transitions.RemoveAt(Transitions.Count - 1);
            }

            return ret;
        }

        private static Stack<IEnumerable<TypeOnStack>> GetStack(VerifiableTracker tracker)
        {
            var retStack = new Stack<IEnumerable<TypeOnStack>>();
            foreach (var t in tracker.Transitions)
            {
                UpdateStack(retStack, t);
            }

            return retStack;
        }

        private bool IsEquivalent(VerifiableTracker other)
        {
            var ourStack = GetStack(this);
            var otherStack = GetStack(other);

            if (ourStack.Count != otherStack.Count) return false;

            for (var i = 0; i < ourStack.Count; i++)
            {
                var ours = ourStack.ElementAt(i);
                var theirs = otherStack.ElementAt(i);

                if (ours.Count() != theirs.Count()) return false;

                if (ours.Any(o => !theirs.Any(t => o == t))) return false;
            }

            return true;
        }

        public VerificationResult Incoming(VerifiableTracker other)
        {
            // We need to reverify the whole thing if we're seeing a branch in
            CachedVerifyIndex = null;
            CachedVerifyStack = null;

            // If we're not baseless, we can't modify ourselves; but we have to make sure the other one is equivalent
            if (!Baseless)
            {
                var isEquivalent = IsEquivalent(other);

                if (isEquivalent)
                {
                    return VerificationResult.Successful(GetStack(this));
                }

                return VerificationResult.FailureStackMismatch(GetStack(this), GetStack(other));
            }

            var old = Transitions;

            Transitions = new List<InstructionAndTransitions>();
            Transitions.AddRange(other.Transitions);
            Transitions.AddRange(old);

            var ret = CollapseAndVerify();
            
            if (!ret.Success)
            {
                // revert!
                Transitions = old;
            }
            else
            {
                // we're no longer baseless if the other guy isn't
                this.Baseless = other.Baseless;
            }

            return ret;
        }

        private static void UpdateStack(Stack<IEnumerable<TypeOnStack>> stack, InstructionAndTransitions wrapped)
        {
            var legal = wrapped.Transitions;
            var instr = wrapped.Instruction;

            if (legal.Any(l => l.PoppedFromStack.Any(u => u == TypeOnStack.Get<PopAllType>())))
            {
                if (instr.HasValue)
                {
                    for (var i = 0; i < stack.Count; i++)
                    {
                        var ix = stack.Count - i - 1;
                        stack.ElementAt(i).Each(y => y.Mark(instr.Value, ix));
                    }
                }

                stack.Clear();
            }
            else
            {
                var toPop = legal.First().PoppedCount;

                for (var j = 0; j < toPop && stack.Count > 0; j++)
                {
                    var popped = stack.Pop();

                    if (instr.HasValue)
                    {
                        var ix = toPop - j - 1;
                        popped.Each(y => y.Mark(instr.Value, ix));
                    }
                }
            }

            var toPush = new List<TypeOnStack>();
            foreach (var op in legal)
            {
                toPush.AddRange(op.PushedToStack);
            }

            if (toPush.Count > 0)
            {
                stack.Push(toPush.Distinct().ToList());
            }
        }

        private Stack<IEnumerable<TypeOnStack>> CachedVerifyStack;
        private int? CachedVerifyIndex;
        private VerificationResult CollapseAndVerify()
        {
            var runningStack = CachedVerifyStack ?? new Stack<IEnumerable<TypeOnStack>>();

            int i = CachedVerifyIndex ?? 0;

            for (; i < Transitions.Count; i++)
            {
                var wrapped = Transitions[i];
                var ops = wrapped.Transitions;

                if(ops.Any(o => o.StackSizeMustBe.HasValue))
                {
                    if(ops.Count() > 1) throw new Exception("Shouldn't have multiple 'must be size' transitions at the same point");
                    var doIt = ops.Single();

                    if(doIt.StackSizeMustBe != runningStack.Count)
                    {
                        return VerificationResult.FailureStackSize(doIt.StackSizeMustBe.Value);
                    }
                }

                var legal =
                    ops.Where(
                        w =>
                        {
                            if (w.PoppedFromStack.All(u => u == TypeOnStack.Get<PopAllType>())) return true;

                            var onStack = runningStack.Peek(Baseless, w.PoppedCount);

                            if (onStack == null)
                            {
                                return false;
                            }

                            for (var j = 0; j < w.PoppedCount; j++)
                            {
                                var shouldBe = w.PoppedFromStack.ElementAt(j);
                                var actuallyIs = onStack[j];

                                if (!actuallyIs.Any(a => shouldBe.IsAssignableFrom(a)))
                                {
                                    return false;
                                }
                            }

                            return true;
                        }
                    ).ToList();

                if (legal.Count == 0)
                {
                    var wouldPop = ops.GroupBy(g => g.PoppedFromStack.Count()).Single().Key;

                    if (runningStack.Count < wouldPop)
                    {
                        return VerificationResult.FailureUnderflow(wouldPop);
                    }

                    IEnumerable<TypeOnStack> expected;
                    var stackI = FindStackFailureIndex(runningStack, ops, out expected);

                    return VerificationResult.FailureTypeMismatch(i, stackI, expected, runningStack);
                }

                if (legal.GroupBy(g => new { a = g.PoppedCount, b = g.PushedToStack.Count() }).Count() > 1)
                {
                    throw new Exception("Shouldn't be possible; legal transitions should have same push/pop #s");
                }

                // No reason to do all this work again
                Transitions[i] = new InstructionAndTransitions(wrapped.Instruction, legal);

                bool popAll = legal.Any(l => l.PoppedFromStack.Contains(TypeOnStack.Get<PopAllType>()));
                if (popAll && legal.Count() != 1)
                {
                    throw new Exception("PopAll cannot coexist with any other transitions");
                }

                if(!popAll)
                {
                    var toPop = legal.First().PoppedCount;

                    if (toPop > runningStack.Count && !Baseless)
                    {
                        return VerificationResult.FailureUnderflow(toPop);
                    }
                }

                bool isDuplicate = legal.Any(l => l.IsDuplicate);
                if (isDuplicate && legal.Count() > 1)
                {
                    throw new Exception("Duplicate must be only transition");
                }

                if (isDuplicate)
                {
                    if (!Baseless && runningStack.Count == 0) return VerificationResult.FailureUnderflow(1);

                    IEnumerable<TypeOnStack> toPush = runningStack.Count > 0 ? runningStack.Peek() : new[] { TypeOnStack.Get<WildcardType>() };

                    UpdateStack(runningStack, new InstructionAndTransitions(wrapped.Instruction, new StackTransition[] { new StackTransition(new TypeOnStack[0], toPush) }));
                }
                else
                {
                    UpdateStack(runningStack, new InstructionAndTransitions(wrapped.Instruction, legal));
                }
            }

            CachedVerifyIndex = i;
            CachedVerifyStack = runningStack;

            return VerificationResult.Successful(runningStack);
        }

        private int FindStackFailureIndex(Stack<IEnumerable<TypeOnStack>> types, IEnumerable<StackTransition> ops, out IEnumerable<TypeOnStack> expected)
        {
            var stillLegal = new List<StackTransition>(ops);

            for (var i = 0; i < types.Count; i++)
            {
                var actuallyIs = types.ElementAt(i);

                var legal = stillLegal.Where(l => actuallyIs.Any(a => l.PoppedFromStack.ElementAt(i).IsAssignableFrom(a))).ToList();

                if (legal.Count == 0)
                {
                    expected = stillLegal.Select(l => l.PoppedFromStack.ElementAt(i)).Distinct().ToList();
                    return i;
                }

                stillLegal = legal;
            }

            throw new Exception("Shouldn't be possible");
        }

        public VerifiableTracker Clone()
        {
            return
                new VerifiableTracker
                {
                    Baseless = Baseless,
                    Transitions = Transitions.ToList()
                };
        }

        // Returns the current stack *if* it can be inferred down to single types *and* is either based or verifiable to the given depth
        public Stack<TypeOnStack> InferStack(int ofDepth)
        {
            var res = CollapseAndVerify();

            if(res.Stack.Count < ofDepth) return null;

            var ret = new Stack<TypeOnStack>();
            for (var i = ofDepth - 1; i >= 0; i--)
            {
                var couldBe = res.Stack.ElementAt(i);

                if (couldBe.Count() > 1) return null;

                ret.Push(couldBe.Single());
            }

            return ret;
        }
    }
}
