using System;

namespace Sigil.Impl
{
    internal class RollingVerifier
    {
        private LinqList<VerifiableTracker> CurrentlyInScope;
        private LinqList<LinqStack<LinqList<TypeOnStack>>> CurrentlyInScopeStacks;

        private LinqDictionary<Label, LinqList<VerifiableTracker>> RestoreOnMark;
        private LinqDictionary<Label, LinqList<LinqStack<LinqList<TypeOnStack>>>> RestoreStacksOnMark;

        private LinqDictionary<Label, LinqList<VerifiableTracker>> VerifyFromLabel;

        private LinqDictionary<Label, SigilTuple<bool, LinqStack<LinqList<TypeOnStack>>>> StacksAtLabels;
        private LinqDictionary<Label, LinqList<SigilTuple<bool, LinqStack<LinqList<TypeOnStack>>>>> ExpectedStacksAtLabels;

        private bool MarkCreatesNewVerifier;

        /// From the spec [see section - III.1.7.5 Backward branch constraints]:
        ///   In particular, if that single-pass analysis arrives at an instruction, call it location X, that 
        ///   immediately follows an unconditional branch, and where X is not the target of an earlier branch 
        ///   instruction, then the state of the evaluation stack at X, clearly, cannot be derived from existing 
        ///   information. In this case, the CLI demands that the evaluation stack at X be empty.
        /// 
        /// In practice, DynamicMethods don't need to follow this rule *but* that doesn't mean stricter
        /// verification won't be needed elsewhere.
        /// 
        /// If this is set, then an "expectation of empty stack" transition is inserted before unconditional branches
        /// where needed.
        private bool UsesStrictBranchVerification;

        private LinqHashSet<Label> MustBeEmptyWhenBranchedTo;

        public RollingVerifier(Label beginAt, bool strictBranchVerification)
        {
            UsesStrictBranchVerification = strictBranchVerification;

            RestoreOnMark = new LinqDictionary<Label, LinqList<VerifiableTracker>>();
            RestoreStacksOnMark = new LinqDictionary<Label, LinqList<LinqStack<LinqList<TypeOnStack>>>>();
            VerifyFromLabel = new LinqDictionary<Label, LinqList<VerifiableTracker>>();

            StacksAtLabels = new LinqDictionary<Label, SigilTuple<bool, LinqStack<LinqList<TypeOnStack>>>>();
            ExpectedStacksAtLabels = new LinqDictionary<Label, LinqList<SigilTuple<bool, LinqStack<LinqList<TypeOnStack>>>>>();

            MustBeEmptyWhenBranchedTo = new LinqHashSet<Label>();

            EmptyCurrentScope();
            
            Add(new VerifiableTracker(beginAt), new LinqStack<LinqList<TypeOnStack>>());
        }

        private void EmptyCurrentScope()
        {
            CurrentlyInScope = new LinqList<VerifiableTracker>();
            CurrentlyInScopeStacks = new LinqList<LinqStack<LinqList<TypeOnStack>>>();
        }

        private void Add(VerifiableTracker tracker, LinqStack<LinqList<TypeOnStack>> stack)
        {
            CurrentlyInScope.Add(tracker);
            CurrentlyInScopeStacks.Add(stack);

            if (CurrentlyInScope.Count != CurrentlyInScopeStacks.Count)
            {
                throw new Exception();
            }
        }

        private void AddRange(LinqList<VerifiableTracker> trackers, LinqList<LinqStack<LinqList<TypeOnStack>>> stacks)
        {
            if (trackers.Count != stacks.Count)
            {
                throw new Exception();
            }

            for (var i = 0; i < trackers.Count; i++)
            {
                Add(trackers[i], stacks[i]);
            }
        }

        private void RemoveAt(int ix)
        {
            CurrentlyInScope.RemoveAt(ix);
            CurrentlyInScopeStacks.RemoveAt(ix);

            if (CurrentlyInScope.Count != CurrentlyInScopeStacks.Count)
            {
                throw new Exception();
            }
        }

        public virtual VerificationResult Mark(Label label)
        {
            // This is, effectively, "follows an unconditional branch & hasn't been seen before"
            if (MarkCreatesNewVerifier && UsesStrictBranchVerification && !ExpectedStacksAtLabels.ContainsKey(label))
            {
                MustBeEmptyWhenBranchedTo.Add(label);
            }

            if (CurrentlyInScope.Count > 0)
            {
                StacksAtLabels[label] = GetCurrentStack();

                var verify = CheckStackMatches(label);
                if (verify != null)
                {
                    return verify;
                }
            }

            if (MarkCreatesNewVerifier)
            {
                var newVerifier = new VerifiableTracker(label, baseless: true);
                Add(newVerifier, new LinqStack<LinqList<TypeOnStack>>());
                MarkCreatesNewVerifier = false;
            }

            LinqList<VerifiableTracker> restore;
            if (RestoreOnMark.TryGetValue(label, out restore))
            {
                // don't copy, we want the *exact* same verifiers restore here
                AddRange(restore, RestoreStacksOnMark[label]);
                RestoreOnMark.Remove(label);
                RestoreStacksOnMark.Remove(label);
            }

            var based = CurrentlyInScope.FirstOrDefault(f => !f.IsBaseless);
            based = based ?? CurrentlyInScope.First();

            var fromLabel = new VerifiableTracker(label, based.IsBaseless, based);
            var fromStack = CurrentlyInScopeStacks[CurrentlyInScope.IndexOf(based)];
            Add(fromLabel, CopyStack(fromStack));

            if (!VerifyFromLabel.ContainsKey(label))
            {
                VerifyFromLabel[label] = new LinqList<VerifiableTracker>();
            }

            VerifyFromLabel[label].Add(fromLabel);

            RemoveUnnecessaryVerifiers();

            return VerificationResult.Successful();
        }

        // Looks at CurrentlyInScope and removes any verifiers that are not necessary going forward
        private void RemoveUnnecessaryVerifiers()
        {
            // if anything's rooted, we only need one of them (since the IL stream being currently valid means they must in the future)
            var rooted = CurrentlyInScope.Where(c => !c.IsBaseless && c.CanBePruned).ToList();
            if (rooted.Count >= 2)
            {
                for (var i = 1; i < rooted.Count; i++)
                {
                    var toRemove = rooted[i];
                    var ix = CurrentlyInScope.IndexOf(toRemove);

                    RemoveAt(ix);
                }
            }

            // remove any verifiers that have duplicate terminal stack states; we know that another verifier will do just as well, no need to verify the whole instruction stream again
            for (var i = CurrentlyInScope.Count - 1; i >= 0; i--)
            {
                var curStack = CurrentlyInScopeStacks[i];

                var otherMatch =
                    CurrentlyInScopeStacks
                        .Select(
                            (cx, ix) =>
                            {
                                if (ix == i) return -1;

                                if (curStack.Count != cx.Count) return -1;

                                if (!CurrentlyInScope[ix].CanBePruned) return -1;

                                for (var j = 0; j < curStack.Count; j++)
                                {
                                    var curFrame = curStack.ElementAt(j);
                                    var cxFrame = cx.ElementAt(j);

                                    if (curFrame.Count != cxFrame.Count) return -1;

                                    curFrame = curFrame.OrderBy(_ => _).ToList();
                                    cxFrame = cxFrame.OrderBy(_ => _).ToList();

                                    for (var k = 0; k < curFrame.Count; k++)
                                    {
                                        var curT = curFrame[k];
                                        var cxT = cxFrame[k];

                                        if (curT != cxT) return -1;
                                    }
                                }

                                return ix;
                            }
                        ).Where(x => x != -1).OrderByDescending(_ => _).ToList();

                foreach (var o in otherMatch.AsEnumerable())
                {
                    RemoveAt(o);

                    if (o < i)
                    {
                        i--;
                    }
                }
            }
        }

        public virtual VerificationResult ReThrow()
        {
            return Throw();
        }

        public virtual VerificationResult Throw()
        {
            EmptyCurrentScope();
            MarkCreatesNewVerifier = true;

            return VerificationResult.Successful();
        }

        public virtual VerificationResult Return()
        {
            EmptyCurrentScope();
            MarkCreatesNewVerifier = true;

            return VerificationResult.Successful();
        }

        public virtual VerificationResult UnconditionalBranch(Label to)
        {
            // If we've recorded elsewhere that the label we're branching to *must* receive
            // an empty stack, then inject a transition that expects that
            if (MustBeEmptyWhenBranchedTo.Contains(to))
            {
                var trans = new LinqList<StackTransition>();
                trans.Add(new StackTransition(sizeMustBe: 0));

                var stackIsEmpty = Transition(new InstructionAndTransitions(null, null, trans));

                if (stackIsEmpty != null) return stackIsEmpty;
            }

            var intoVerified = VerifyBranchInto(to);
            if (intoVerified != null)
            {
                return intoVerified;
            }

            UpdateRestores(to);

            if (!RestoreOnMark.ContainsKey(to))
            {
                RestoreOnMark[to] = new LinqList<VerifiableTracker>();
                RestoreStacksOnMark[to] = new LinqList<LinqStack<LinqList<TypeOnStack>>>();
            }

            if (!ExpectedStacksAtLabels.ContainsKey(to))
            {
                ExpectedStacksAtLabels[to] = new LinqList<SigilTuple<bool, LinqStack<LinqList<TypeOnStack>>>>();
            }
            ExpectedStacksAtLabels[to].Add(GetCurrentStack());

            var verify = CheckStackMatches(to);
            if (verify != null)
            {
                return verify;
            }

            RestoreOnMark[to].AddRange(CurrentlyInScope);
            RestoreStacksOnMark[to].AddRange(CurrentlyInScopeStacks);

            EmptyCurrentScope();
            MarkCreatesNewVerifier = true;

            return VerificationResult.Successful();
        }

        public virtual VerificationResult ConditionalBranch(params Label[] toLabels)
        {
            foreach(var to in toLabels)
            {
                var intoVerified = VerifyBranchInto(to);
                if (intoVerified != null)
                {
                    return intoVerified;
                }

                UpdateRestores(to);

                if (!RestoreOnMark.ContainsKey(to))
                {
                    if (!RestoreOnMark.ContainsKey(to))
                    {
                        RestoreOnMark[to] = new LinqList<VerifiableTracker>();
                        RestoreStacksOnMark[to] = new LinqList<LinqStack<LinqList<TypeOnStack>>>();
                    }

                    RestoreOnMark[to].AddRange(CurrentlyInScope.Select(t => t.Clone()));
                    RestoreStacksOnMark[to].AddRange(CurrentlyInScopeStacks.Select(s => CopyStack(s)));
                }

                if (!ExpectedStacksAtLabels.ContainsKey(to))
                {
                    ExpectedStacksAtLabels[to] = new LinqList<SigilTuple<bool, LinqStack<LinqList<TypeOnStack>>>>();
                }
                ExpectedStacksAtLabels[to].Add(GetCurrentStack());

                var verify = CheckStackMatches(to);
                if (verify != null)
                {
                    return verify;
                }
            }

            return VerificationResult.Successful();
        }

        private SigilTuple<bool, LinqStack<LinqList<TypeOnStack>>> GetCurrentStack()
        {
            SigilTuple<bool, LinqStack<LinqList<TypeOnStack>>> ret = null;
            for(var i = 0; i < CurrentlyInScope.Count; i++)
            {
                var c = CurrentlyInScope[i];
                var stack = CurrentlyInScopeStacks[i];

                var stackCopy = CopyStack(stack);

                var innerRet = SigilTuple.Create(c.IsBaseless, stackCopy);

                if (ret == null || (innerRet.Item1 && !ret.Item1) || innerRet.Item2.Count > ret.Item2.Count)
                {
                    ret = innerRet;
                }
            }

            return ret;
        }

        private VerificationResult CheckStackMatches(Label atLabel)
        {
            if (!StacksAtLabels.ContainsKey(atLabel) || !ExpectedStacksAtLabels.ContainsKey(atLabel)) return null;

            var actual = StacksAtLabels[atLabel];
            var expectations = ExpectedStacksAtLabels[atLabel];

            foreach (var exp in expectations.AsEnumerable())
            {
                var mismatch = CompareStacks(atLabel, actual, exp);
                if (mismatch != null)
                {
                    return mismatch;
                }
            }

            return null;
        }

        private VerificationResult CompareStacks(Label label, SigilTuple<bool, LinqStack<LinqList<TypeOnStack>>> actual, SigilTuple<bool, LinqStack<LinqList<TypeOnStack>>> expected)
        {
            if (!actual.Item1 && !expected.Item1)
            {
                if (expected.Item2.Count != actual.Item2.Count)
                {
                    // Both stacks are based, so the wrong size is a serious error as well
                    return VerificationResult.FailureUnderflow(label, expected.Item2.Count);
                }
            }

            for (var i = 0; i < expected.Item2.Count; i++)
            {
                if (i >= actual.Item2.Count && !actual.Item1)
                {
                    // actual is based and expected wanted a value, this is an UNDERFLOW
                    return VerificationResult.FailureUnderflow(label, expected.Item2.Count);
                }

                var expectedTypes = expected.Item2.ElementAt(i);
                LinqList<TypeOnStack> actualTypes;
                if (i < actual.Item2.Count)
                {
                    actualTypes = actual.Item2.ElementAt(i);
                }
                else
                {
                    // Underflowed, but our actual stack is baseless; so we assume we're good until proven otherwise
                    break;
                }

                bool typesMatch = false;
                foreach (var a in actualTypes.AsEnumerable())
                {
                    foreach (var e in expectedTypes.AsEnumerable())
                    {
                        typesMatch |= e.IsAssignableFrom(a);
                    }
                }

                if (!typesMatch)
                {
                    // Just went through what's on the stack, and we the types are known to not be compatible
                    return VerificationResult.FailureTypeMismatch(label, actualTypes, expectedTypes);
                }
            }

            return null;
        }

        private void UpdateRestores(Label l)
        {
            // current verify is branching to this label, go and update all the "will be restored" bits

            var curRoot = CurrentlyInScope.FirstOrDefault(f => !f.IsBaseless);
            curRoot = curRoot ?? CurrentlyInScope.OrderByDescending(c => c.Iteration).First();

            foreach (var kv in RestoreOnMark.AsEnumerable())
            {
                foreach (var v in kv.Value.ToList().AsEnumerable())
                {
                    if (v.BeganAt == l)
                    {
                        var replacement = curRoot.Concat(v);

                        kv.Value.Remove(v);
                        kv.Value.Add(replacement);
                    }
                }
            }
        }

        private VerificationResult VerifyBranchInto(Label to)
        {
            LinqList<VerifiableTracker> onInto;
            if (!VerifyFromLabel.TryGetValue(to, out onInto)) return null;

            VerifyFromLabel.Remove(to);

            foreach (var c in CurrentlyInScope.AsEnumerable())
            {
                foreach (var into in onInto.AsEnumerable())
                {
                    var completedCircuit = c.Concat(into);

                    var verified = completedCircuit.CollapseAndVerify();
                    if (!verified.Success)
                    {
                        return verified;
                    }

                    if (completedCircuit.IsBaseless)
                    {
                        if (!VerifyFromLabel.ContainsKey(completedCircuit.BeganAt))
                        {
                            VerifyFromLabel[completedCircuit.BeganAt] = new LinqList<VerifiableTracker>();
                        }

                        VerifyFromLabel[completedCircuit.BeganAt].Add(completedCircuit);
                    }
                }
            }

            return null;
        }

        private LinqStack<LinqList<TypeOnStack>> CopyStack(LinqStack<LinqList<TypeOnStack>> toCopy)
        {
            var ret = new LinqStack<LinqList<TypeOnStack>>(toCopy.Count);

            for (var i = toCopy.Count - 1; i >= 0; i--)
            {
                ret.Push(toCopy.ElementAt(i));
            }

            return ret;
        }

        public virtual VerificationResult Transition(InstructionAndTransitions legalTransitions)
        {
            var stacks = new LinqList<LinqStack<LinqList<TypeOnStack>>>();

            VerificationResult last = null;
            foreach (var x in CurrentlyInScope.AsEnumerable())
            {
                var inner = x.Transition(legalTransitions);

                if (!inner.Success) return inner;

                last = inner;
                stacks.Add(CopyStack(inner.Stack));
            }

            CurrentlyInScopeStacks = stacks;

            return last;
        }

        public virtual LinqStack<TypeOnStack> InferStack(int ofDepth)
        {
            return CurrentlyInScope.First().InferStack(ofDepth);
        }
    }
}
