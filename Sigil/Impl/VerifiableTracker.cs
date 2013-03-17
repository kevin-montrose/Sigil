using System;
using System.Collections.Generic;
using System.Text;

namespace Sigil.Impl
{
    internal class VerifiableTracker
    {
        public int Iteration { get { return Transitions.Count; } }

        public Label BeganAt { get; private set; }

        // When the stack is "unbased" or "baseless", underflowing it results in wildcards
        //   eventually they'll be fixed up to actual types
        public bool IsBaseless { get; private set; }
        private List<InstructionAndTransitions> Transitions = new List<InstructionAndTransitions>();

        private Dictionary<Label, int> MarkedLabelsAtTransitions = new Dictionary<Label, int>();
        private Dictionary<Label, int> BranchesAtTransitions = new Dictionary<Label, int>();

        private Stack<List<TypeOnStack>> StartingStack = new Stack<List<TypeOnStack>>();

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

        public bool ContainsUsageOf(Label label)
        {
            return MarkedLabelsAtTransitions.ContainsKey(label) || BranchesAtTransitions.ContainsKey(label);
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

        // Cuts out any sequence that is wholy contained within another
        private static List<VerifiableTrackerConcatPromise> RemoveOverlapping(List<VerifiableTrackerConcatPromise> all)
        {
            var ret = new List<VerifiableTrackerConcatPromise>();

            foreach (var a in all)
            {
                if (all.Any(x => x != a && x.Contains(a))) continue;

                ret.Add(a);
            }

            return ret;
        }

        public static VerificationResult Verify(Label modified, IEnumerable<VerifiableTracker> all, HashSet<TrackerDescriber> verificationCache)
        {
            var allStreams = new List<VerifiableTrackerConcatPromise>();

            var asPromise = all.Select(a => new VerifiableTrackerConcatPromise(a)).ToList();

            var involvingLabel = 
                modified != null ? 
                    asPromise.Where(p => p.ContainsUsageOf(modified) || !p.Inner.IsBaseless).ToList() : 
                    asPromise;

            for(var i = 0; i < involvingLabel.Count; i++)
            {
                var root = involvingLabel[i];

                var streams = BuildStreams(root, asPromise);

                var unique = RemoveOverlapping(streams);

                allStreams.AddRange(unique);
            }

            var culled = RemoveOverlapping(allStreams);

            for(var i = 0; i < culled.Count; i++)
            {
                var describer = culled[i].Description;

                if (verificationCache.Contains(describer))
                {
                    continue;
                }

                var s = culled[i].DePromise();

                var res = s.CollapseAndVerify();

                if (!res.Success)
                {
                    return res;
                }

                verificationCache.Add(describer);
            }

            return null;
        }

        private static List<VerifiableTrackerConcatPromise> BuildStreams(VerifiableTrackerConcatPromise root, IEnumerable<VerifiableTrackerConcatPromise> all)
        {
            var ret = new List<VerifiableTrackerConcatPromise>();
            ret.Add(root);

            foreach (var mark in root.Inner.BranchesAtTransitions)
            {
                var startingAt = all.SingleOrDefault(a => a.Inner.BeganAt == mark.Key);

                if (startingAt == null) continue;

                var exceptSelf = all.Where(a => a != startingAt).ToList();

                var sub = BuildStreams(startingAt, exceptSelf);

                for(var i = 0; i < sub.Count; i++)
                {
                    var s = sub[i];

                    ret.Add(root.Concat(s));
                }
            }

            return ret;
        }

        internal VerifiableTracker Concat(VerifiableTracker other)
        {
            var branchTo = BranchesAtTransitions.ContainsKey(other.BeganAt) ? BranchesAtTransitions[other.BeganAt] : Transitions.Count;
            var shouldTake = branchTo != Transitions.Count;

            var trans = new List<InstructionAndTransitions>(branchTo + other.Transitions.Count);
            //trans.AddRange(shouldTake ? Transitions.Take(branchTo) : Transitions);

            if (shouldTake)
            {
                for (var i = 0; i < branchTo; i++)
                {
                    trans.Add(Transitions[i]);
                }
            }
            else
            {
                trans.AddRange(Transitions);
            }

            trans.AddRange(other.Transitions);

            var canReuseCache = branchTo == Transitions.Count && IsBaseless && CachedVerifyStack != null;

            var ret =
                new VerifiableTracker(BeganAt, IsBaseless)
                {
                    StartingStack = IsBaseless ? new Stack<List<TypeOnStack>>(StartingStack) : new Stack<List<TypeOnStack>>(),
                    Transitions = trans,
                    CachedVerifyStack = canReuseCache ? new Stack<List<TypeOnStack>>(CachedVerifyStack) : null,
                    CachedVerifyIndex = canReuseCache ? CachedVerifyIndex : null
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

            return ret;
        }

        public int? GetInstructionIndex(int ix)
        {
            if (ix < 0 || ix >= Transitions.Count) throw new Exception("ix must be between 0 and " + (Transitions.Count - 1) + "; found " + ix);

            return Transitions[ix].InstructionIndex;
        }

        private static Stack<List<TypeOnStack>> GetStack(VerifiableTracker tracker)
        {
            var retStack = new Stack<List<TypeOnStack>>(tracker.StartingStack.Reverse());

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

        private static void UpdateStack(Stack<List<TypeOnStack>> stack, InstructionAndTransitions wrapped, bool isBaseless)
        {
            var legal = wrapped.Transitions;
            var instr = wrapped.Instruction;

            var legalSize = 0;

            legal.Each(
                t =>
                {
                    legalSize += t.PushedToStack.Count();

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

            var toPush = new List<TypeOnStack>(legalSize);
            var pushed = new HashSet<TypeOnStack>();
            for(var i = 0; i < legal.Count; i++)
            {
                foreach (var p in legal[i].PushedToStack)
                {
                    if (pushed.Contains(p)) continue;

                    toPush.Add(p);
                    pushed.Add(p);
                }
            }

            if (toPush.Count > 0)
            {
                stack.Push(toPush);
            }
        }

        private List<StackTransition> GetLegalTransitions(List<StackTransition> ops, Stack<List<TypeOnStack>> runningStack)
        {
            var ret = new List<StackTransition>(ops.Count);

            for (var i = 0; i < ops.Count; i++)
            {
                var w = ops[i];

                if (w.PoppedFromStack.All(u => u == TypeOnStack.Get<PopAllType>()))
                {
                    ret.Add(w);
                    continue;
                }

                var onStack = runningStack.Peek(IsBaseless, w.PoppedCount);

                if (onStack == null)
                {
                    continue;
                }

                bool outerContinue = false;

                for (var j = 0; j < w.PoppedCount; j++)
                {
                    var shouldBe = w.PoppedFromStack.ElementAt(j);
                    var actuallyIs = onStack[j];

                    if (!actuallyIs.Any(a => shouldBe.IsAssignableFrom(a)))
                    {
                        outerContinue = true;
                        break;
                    }
                }

                if (outerContinue) continue;

                ret.Add(w);
            }

            return ret;
        }

        private Stack<List<TypeOnStack>> CachedVerifyStack;
        private int? CachedVerifyIndex;
        private VerificationResult CollapseAndVerify()
        {
            var runningStack = CachedVerifyStack ?? new Stack<List<TypeOnStack>>(StartingStack.Reverse());

            int i = CachedVerifyIndex ?? 0;

            for (; i < Transitions.Count; i++)
            {
                var wrapped = Transitions[i];
                var ops = wrapped.Transitions;

                if(ops.Any(o => o.StackSizeMustBe.HasValue))
                {
                    if (ops.Count > 1)
                    {
                        throw new Exception("Shouldn't have multiple 'must be size' transitions at the same point");
                    }

                    var doIt = ops[0];

                    if(doIt.StackSizeMustBe != runningStack.Count)
                    {
                        return VerificationResult.FailureStackSize(this, i, doIt.StackSizeMustBe.Value);
                    }
                }

                var legal = GetLegalTransitions(ops, runningStack);

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

                    var toPush = runningStack.Count > 0 ? runningStack.Peek() : new List<TypeOnStack>(new[] { TypeOnStack.Get<WildcardType>() });

                    UpdateStack(runningStack, new InstructionAndTransitions(wrapped.Instruction, wrapped.InstructionIndex, new List<StackTransition>(new [] { new StackTransition(new TypeOnStack[0], toPush) })), IsBaseless);
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

        private int FindStackFailureIndex(Stack<List<TypeOnStack>> types, IEnumerable<StackTransition> ops, out IEnumerable<TypeOnStack> expected)
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
