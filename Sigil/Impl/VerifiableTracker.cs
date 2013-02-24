using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sigil.Impl
{
    internal class VerifiableTracker
    {
        public Label BeganAt { get; private set; }

        // When the stack is "unbased" or "baseless", underflowing it results in wildcards
        //   eventually they'll be fixed up to actual types
        public bool IsBaseless { get; private set; }
        private List<InstructionAndTransitions> Transitions = new List<InstructionAndTransitions>();
        private Dictionary<InstructionAndTransitions, int> TransitionLookup = new Dictionary<InstructionAndTransitions, int>();

        private Dictionary<Label, int> MarkedLabelsAtTransitions = new Dictionary<Label, int>();
        private Dictionary<Label, int> BranchesAtTransitions = new Dictionary<Label, int>();

        private Stack<IEnumerable<TypeOnStack>> StartingStack = new Stack<IEnumerable<TypeOnStack>>();

        public VerifiableTracker(Label beganAt, bool baseless = false, VerifiableTracker createdFrom = null) 
        {
            IsBaseless = baseless;
            BeganAt = beganAt;

            MarkedLabelsAtTransitions[beganAt] = 0;

            if (createdFrom != null)
            {
                StartingStack = GetStack(createdFrom);
            }
        }

        public void Mark(Label label)
        {
            MarkedLabelsAtTransitions[label] = Transitions.Count;
        }

        public void Branch(Label label)
        {
            BranchesAtTransitions[label] = Transitions.Count;
        }

        private static bool CompareOnStack(IEnumerable<TypeOnStack> a, IEnumerable<TypeOnStack> b, bool mustBeExact)
        {
            if (mustBeExact)
            {
                if (a.Count() != b.Count()) return false;

                return !a.Any(x => !b.Any(y => x == y));
            }

            return a.Any(x => b.Any(y => x == y)) || b.Any(y => a.Any(x => x == y));
        }

        public VerificationResult AreCompatible(VerifiableTracker other)
        {
            var thisStack = GetStack(this);
            var otherStack = GetStack(other);

            if (!IsBaseless && !other.IsBaseless)
            {
                if (thisStack.Count != otherStack.Count)
                {
                    return VerificationResult.FailureStackMismatch(this, thisStack, otherStack);
                }
            }

            var literalCheck = Math.Min(thisStack.Count, otherStack.Count);

            for (var i = 0; i < literalCheck; i++)
            {
                var onThis = thisStack.ElementAt(i);
                var onOther = otherStack.ElementAt(i);

                if (!CompareOnStack(onThis, onOther, !(IsBaseless || other.IsBaseless)))
                {
                    return VerificationResult.FailureStackMismatch(this, thisStack, otherStack);
                }
            }

            return null;
        }

        private bool Contains(VerifiableTracker other)
        {
            if (other.Transitions.Count == 0) return true;

            var start = other.Transitions.First();
            int inThisIx;
            if (!TransitionLookup.TryGetValue(start, out inThisIx)) return false;

            for (var i = 0; i < other.Transitions.Count; i++)
            {
                if (inThisIx + i >= this.Transitions.Count) return false;

                var otherTran = other.Transitions[i];
                var thisTran = this.Transitions[i + inThisIx];

                if (otherTran != thisTran) return false;
            }

            return true;
        }

        // Cuts out any sequence that is wholy contained within another
        private static List<VerifiableTracker> RemoveOverlapping(List<VerifiableTracker> all)
        {
            var ret = new List<VerifiableTracker>();

            foreach (var a in all)
            {
                if (all.Any(x => x != a && x.Contains(a))) continue;

                ret.Add(a);
            }

            return ret;
        }

        public static VerificationResult Verify(IEnumerable<VerifiableTracker> all)
        {
            var allStreams = new List<VerifiableTracker>();

            foreach (var root in all)
            {
                var streams = BuildStreams(root, all);

                var longest = RemoveOverlapping(streams);

                allStreams.AddRange(longest);
            }

            var culled = RemoveOverlapping(allStreams);

            foreach (var s in culled)
            {
                var res = s.CollapseAndVerify();

                if (!res.Success)
                {
                    return res;
                }
            }

            return null;
        }

        private static List<VerifiableTracker> BuildStreams(VerifiableTracker root, IEnumerable<VerifiableTracker> all)
        {
            var ret = new List<VerifiableTracker>();
            ret.Add(root);

            foreach (var mark in root.BranchesAtTransitions)
            {
                var startingAt = all.SingleOrDefault(a => a.BeganAt == mark.Key);

                if (startingAt == null) continue;

                var exceptSelf = all.Where(a => a != startingAt);

                var sub = BuildStreams(startingAt, exceptSelf);

                foreach (var s in sub)
                {
                    ret.Add(root.Concat(s));
                }
            }

            return ret;
        }

        private VerifiableTracker Concat(VerifiableTracker other)
        {
            var trans = new List<InstructionAndTransitions>();
            trans.AddRange(Transitions);
            trans.AddRange(other.Transitions);

            var lookup = new Dictionary<InstructionAndTransitions, int>(TransitionLookup);
            int offset = lookup.Count;
            foreach (var kv in other.TransitionLookup)
            {
                lookup[kv.Key] = kv.Value + offset;
            }

            var ret =
                new VerifiableTracker(BeganAt, IsBaseless)
                {
                    StartingStack = IsBaseless ? new Stack<IEnumerable<TypeOnStack>>(StartingStack) : new Stack<IEnumerable<TypeOnStack>>(),
                    Transitions = trans,
                    CachedVerifyStack = IsBaseless && CachedVerifyStack != null ? new Stack<IEnumerable<TypeOnStack>>(CachedVerifyStack) : null,
                    CachedVerifyIndex = IsBaseless ? CachedVerifyIndex : null,
                    TransitionLookup = lookup
                };

            return ret;
        }

        public VerificationResult Transition(InstructionAndTransitions legalTransitions)
        {
            Transitions.Add(legalTransitions);
            var ret = CollapseAndVerify();

            // revert!
            if (!ret.Success)
            {
                Transitions.RemoveAt(Transitions.Count - 1);
            }

            TransitionLookup[legalTransitions] = Transitions.Count - 1;

            return ret;
        }

        public int? GetInstructionIndex(int ix)
        {
            if (ix < 0 || ix >= Transitions.Count) throw new Exception("ix must be between 0 and " + (Transitions.Count - 1) + "; found " + ix);

            return Transitions[ix].InstructionIndex;
        }

        private static Stack<IEnumerable<TypeOnStack>> GetStack(VerifiableTracker tracker)
        {
            var retStack = new Stack<IEnumerable<TypeOnStack>>(tracker.StartingStack);

            foreach (var t in tracker.Transitions)
            {
                UpdateStack(retStack, t, tracker.IsBaseless);
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

        private static void UpdateStack(Stack<IEnumerable<TypeOnStack>> stack, InstructionAndTransitions wrapped, bool isBaseless)
        {
            var legal = wrapped.Transitions;
            var instr = wrapped.Instruction;

            legal.Each(
                t =>
                {
                    if (t.Before != null) t.Before(stack, isBaseless);
                }
            );

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

            legal.Each(
                t =>
                {
                    if (t.After != null) t.After(stack, isBaseless);
                }
            );
        }

        private Stack<IEnumerable<TypeOnStack>> CachedVerifyStack;
        private int? CachedVerifyIndex;
        private VerificationResult CollapseAndVerify()
        {
            var runningStack = CachedVerifyStack ?? new Stack<IEnumerable<TypeOnStack>>(StartingStack);

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
                        return VerificationResult.FailureStackSize(this, i, doIt.StackSizeMustBe.Value);
                    }
                }

                var legal =
                    ops.Where(
                        w =>
                        {
                            if (w.PoppedFromStack.All(u => u == TypeOnStack.Get<PopAllType>())) return true;

                            var onStack = runningStack.Peek(IsBaseless, w.PoppedCount);

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
                        return VerificationResult.FailureUnderflow(this, i, wouldPop, runningStack);
                    }

                    IEnumerable<TypeOnStack> expected;
                    var stackI = FindStackFailureIndex(runningStack, ops, out expected);

                    return VerificationResult.FailureTypeMismatch(this, i, stackI, expected, runningStack);
                }

                if (legal.GroupBy(g => new { a = g.PoppedCount, b = g.PushedToStack.Count() }).Count() > 1)
                {
                    throw new Exception("Shouldn't be possible; legal transitions should have same push/pop #s");
                }

                // No reason to do all this work again
                Transitions[i] = new InstructionAndTransitions(wrapped.Instruction, wrapped.InstructionIndex, legal);
                TransitionLookup.Remove(wrapped);
                TransitionLookup[Transitions[i]] = i;

                bool popAll = legal.Any(l => l.PoppedFromStack.Contains(TypeOnStack.Get<PopAllType>()));
                if (popAll && legal.Count() != 1)
                {
                    throw new Exception("PopAll cannot coexist with any other transitions");
                }

                if(!popAll)
                {
                    var toPop = legal.First().PoppedCount;

                    if (toPop > runningStack.Count && !IsBaseless)
                    {
                        return VerificationResult.FailureUnderflow(this, i, toPop, runningStack);
                    }
                }

                bool isDuplicate = legal.Any(l => l.IsDuplicate);
                if (isDuplicate && legal.Count() > 1)
                {
                    throw new Exception("Duplicate must be only transition");
                }

                if (isDuplicate)
                {
                    if (!IsBaseless && runningStack.Count == 0) return VerificationResult.FailureUnderflow(this, i, 1, runningStack);

                    IEnumerable<TypeOnStack> toPush = runningStack.Count > 0 ? runningStack.Peek() : new[] { TypeOnStack.Get<WildcardType>() };

                    UpdateStack(runningStack, new InstructionAndTransitions(wrapped.Instruction, wrapped.InstructionIndex, new StackTransition[] { new StackTransition(new TypeOnStack[0], toPush) }), IsBaseless);
                }
                else
                {
                    UpdateStack(runningStack, new InstructionAndTransitions(wrapped.Instruction, wrapped.InstructionIndex, legal), IsBaseless);
                }
            }

            CachedVerifyIndex = i;
            CachedVerifyStack = runningStack;

            return VerificationResult.Successful(this, runningStack);
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
                new VerifiableTracker(BeganAt)
                {
                    IsBaseless = IsBaseless,
                    MarkedLabelsAtTransitions = new Dictionary<Label,int>(MarkedLabelsAtTransitions),
                    BranchesAtTransitions = new Dictionary<Label,int>(BranchesAtTransitions),
                    Transitions = Transitions.ToList(),
                    TransitionLookup = new Dictionary<InstructionAndTransitions,int>(TransitionLookup)
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

        public override string ToString()
        {
            var ret = new StringBuilder();

            if (StartingStack.Count > 0)
            {
                ret.AppendLine(
                    "starts with: " +
                        string.Join(", ",
                            StartingStack.Select(s => "[" + string.Join(", or", s.Select(x => x.ToString()).ToArray()) + "]").ToArray()
                        )
                );
            }

            for(var i = 0; i < Transitions.Count; i++)
            {
                var label = MarkedLabelsAtTransitions.Where(kv => kv.Value == i).Select(kv => kv.Key).SingleOrDefault();

                if (label != null)
                {
                    ret.AppendLine(label.Name + ":");
                }

                var tran = Transitions[i];

                ret.AppendLine(tran.ToString());
            }

            return ret.ToString();
        }
    }
}
