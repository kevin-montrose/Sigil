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
        private LinqList<InstructionAndTransitions> Transitions = new LinqList<InstructionAndTransitions>();

        private LinqDictionary<Label, int> MarkedLabelsAtTransitions = new LinqDictionary<Label, int>();
        private LinqDictionary<Label, int> BranchesAtTransitions = new LinqDictionary<Label, int>();

        private LinqStack<LinqList<TypeOnStack>> StartingStack = new LinqStack<LinqList<TypeOnStack>>();

        public bool CanBePruned { get; private set; }

        public VerifiableTracker(Label beganAt, bool baseless = false, VerifiableTracker createdFrom = null, bool canBePruned = true) 
        {
            IsBaseless = baseless;
            BeganAt = beganAt;
            CanBePruned = canBePruned;

            MarkedLabelsAtTransitions[beganAt] = 0;

            if (createdFrom != null)
            {
                StartingStack = GetStack(createdFrom);
            }
        }

        internal VerifiableTracker Concat(VerifiableTracker other)
        {
            var branchTo = BranchesAtTransitions.ContainsKey(other.BeganAt) ? BranchesAtTransitions[other.BeganAt] : Transitions.Count;
            var shouldTake = branchTo != Transitions.Count;

            var trans = new LinqList<InstructionAndTransitions>(branchTo + other.Transitions.Count);

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
                    StartingStack = new LinqStack<LinqList<TypeOnStack>>(StartingStack.Reverse().AsEnumerable()),
                    Transitions = trans,
                    CachedVerifyStack = canReuseCache ? new LinqStack<LinqList<TypeOnStack>>(CachedVerifyStack.Reverse().AsEnumerable()) : null,
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

        private static LinqStack<LinqList<TypeOnStack>> GetStack(VerifiableTracker tracker)
        {
            var retStack = new LinqStack<LinqList<TypeOnStack>>(tracker.StartingStack.Reverse());

            foreach (var t in tracker.Transitions.AsEnumerable())
            {
                UpdateStack(retStack, t, tracker.IsBaseless);
            }

            return retStack;
        }

        private static void UpdateStack(LinqStack<LinqList<TypeOnStack>> stack, InstructionAndTransitions wrapped, bool isBaseless)
        {
            var legal = wrapped.Transitions;
            var instr = wrapped.Instruction;

            var legalSize = 0;

            legal.Each(
                t =>
                {
                    legalSize += t.PushedToStack.Length;

                    if (t.Before != null) t.Before(stack, isBaseless);
                }
            );

            if (legal.Any(l => LinqAlternative.Any(l.PoppedFromStack, u => u == TypeOnStack.Get<PopAllType>())))
            {
                if (instr.HasValue)
                {
                    for (var i = 0; i < stack.Count; i++)
                    {
                        var ix = stack.Count - i - 1;
                        stack.ElementAt(i).Each(y => y.Mark(wrapped, ix));
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
                        popped.Each(y => y.Mark(wrapped, ix));
                    }
                }
            }

            var toPush = new LinqList<TypeOnStack>(legalSize);
            var pushed = new LinqHashSet<TypeOnStack>();
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

        private LinqList<StackTransition> GetLegalTransitions(LinqList<StackTransition> ops, LinqStack<LinqList<TypeOnStack>> runningStack)
        {
            var ret = new LinqList<StackTransition>(ops.Count);

            for (var i = 0; i < ops.Count; i++)
            {
                var w = ops[i];

                if (LinqAlternative.All(w.PoppedFromStack, u => u == TypeOnStack.Get<PopAllType>()))
                {
                    ret.Add(w);
                    continue;
                }

                var onStack = runningStack.Peek(IsBaseless, w.PoppedCount);

                if (onStack == null)
                {
                    continue;
                }

                if (LinqAlternative.Any(w.PushedToStack, p => p == TypeOnStack.Get<SamePointerType>()))
                {
                    if (w.PushedToStack.Length > 1)
                    {
                        throw new Exception("SamePointerType can be only product of a transition which contains it");
                    }

                    var shouldBePointer = LinqAlternative.SelectMany(onStack, p => p.Where(x => x.IsPointer || x == TypeOnStack.Get<WildcardType>()).AsEnumerable()).Distinct().ToList();

                    if (shouldBePointer.Count == 0) continue;
                    w = new StackTransition(w.PoppedFromStack, new [] { shouldBePointer.Single() });
                }

                if (LinqAlternative.Any(w.PushedToStack, p => p == TypeOnStack.Get<SameByRefType>()))
                {
                    if (w.PushedToStack.Length > 1)
                    {
                        throw new Exception("SameByRefType can be only product of a transition which contains it");
                    }

                    var shouldBeByRef = LinqAlternative.SelectMany(onStack, p => p.Where(x => x.IsReference || x == TypeOnStack.Get<WildcardType>()).AsEnumerable()).Distinct().ToList();

                    if (shouldBeByRef.Count == 0) continue;
                    w = new StackTransition(w.PoppedFromStack, new[] { shouldBeByRef.Single() });
                }

                bool outerContinue = false;

                for (var j = 0; j < w.PoppedCount; j++)
                {
                    var shouldBe = w.PoppedFromStack[j];
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

        private LinqStack<LinqList<TypeOnStack>> CachedVerifyStack;
        private int? CachedVerifyIndex;
        public VerificationResult CollapseAndVerify()
        {
            var runningStack = CachedVerifyStack ?? new LinqStack<LinqList<TypeOnStack>>(StartingStack.Reverse());

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
                    var wouldPop = ops.GroupBy(g => g.PoppedFromStack.Length).Single().Key;

                    if (runningStack.Count < wouldPop)
                    {
                        return VerificationResult.FailureUnderflow(this, i, wouldPop, runningStack);
                    }

                    IEnumerable<TypeOnStack> expected;
                    var stackI = FindStackFailureIndex(runningStack, ops.AsEnumerable(), out expected);

                    return VerificationResult.FailureTypeMismatch(this, i, stackI, expected, runningStack);
                }

                if (legal.GroupBy(g => new { a = g.PoppedCount, b = g.PushedToStack.Length }).Count() > 1)
                {
                    throw new Exception("Shouldn't be possible; legal transitions should have same push/pop #s");
                }

                // No reason to do all this work again
                Transitions[i] = new InstructionAndTransitions(wrapped.Instruction, wrapped.InstructionIndex, legal);

                bool popAll = legal.Any(l => ((LinqArray<TypeOnStack>)l.PoppedFromStack).Contains(TypeOnStack.Get<PopAllType>()));
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

                    var toPush = runningStack.Count > 0 ? runningStack.Peek() : new LinqList<TypeOnStack>(new[] { TypeOnStack.Get<WildcardType>() });

                    UpdateStack(runningStack, new InstructionAndTransitions(wrapped.Instruction, wrapped.InstructionIndex, new LinqList<StackTransition>(new[] { new StackTransition(new TypeOnStack[0], toPush.AsEnumerable()) })), IsBaseless);
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

        private int FindStackFailureIndex(LinqStack<LinqList<TypeOnStack>> types, IEnumerable<StackTransition> ops, out IEnumerable<TypeOnStack> expected)
        {
            var stillLegal = new LinqList<StackTransition>(ops);

            for (var i = 0; i < types.Count; i++)
            {
                var actuallyIs = types.ElementAt(i);

                var legal = stillLegal.Where(l => actuallyIs.Any(a => l.PoppedFromStack[i].IsAssignableFrom(a))).ToList();

                if (legal.Count == 0)
                {
                    expected = stillLegal.Select(l => l.PoppedFromStack[i]).Distinct().ToList().AsEnumerable();
                    return i;
                }

                stillLegal = new LinqList<StackTransition>(legal);
            }

            throw new Exception("Shouldn't be possible");
        }

        public VerifiableTracker Clone()
        {
            return
                new VerifiableTracker(BeganAt)
                {
                    IsBaseless = IsBaseless,
                    MarkedLabelsAtTransitions = new LinqDictionary<Label,int>(MarkedLabelsAtTransitions),
                    BranchesAtTransitions = new LinqDictionary<Label,int>(BranchesAtTransitions),
                    Transitions = new LinqList<InstructionAndTransitions>(Transitions.AsEnumerable()),
                    StartingStack = new LinqStack<LinqList<TypeOnStack>>(StartingStack.Reverse())
                };
        }

        // Returns the current stack *if* it can be inferred down to single types *and* is either based or verifiable to the given depth
        public LinqStack<TypeOnStack> InferStack(int ofDepth)
        {
            var res = CollapseAndVerify();

            if(res.Stack.Count < ofDepth) return null;

            var ret = new LinqStack<TypeOnStack>();
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
