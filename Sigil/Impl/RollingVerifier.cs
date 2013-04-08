using System;

namespace Sigil.Impl
{
    internal class RollingVerifier
    {
        private LinqList<VerifiableTracker> CurrentlyInScope;

        private LinqDictionary<Label, LinqList<VerifiableTracker>> RestoreOnMark;

        private LinqDictionary<Label, LinqList<VerifiableTracker>> VerifyFromLabel;

        private LinqDictionary<Label, Tuple<bool, LinqStack<LinqList<TypeOnStack>>>> StacksAtLabels;
        private LinqDictionary<Label, LinqList<Tuple<bool, LinqStack<LinqList<TypeOnStack>>>>> ExpectedStacksAtLabels;

        private bool MarkCreatesNewVerifier;

        public RollingVerifier(Label beginAt)
        {
            RestoreOnMark = new LinqDictionary<Label, LinqList<VerifiableTracker>>();
            VerifyFromLabel = new LinqDictionary<Label, LinqList<VerifiableTracker>>();

            StacksAtLabels = new LinqDictionary<Label, Tuple<bool, LinqStack<LinqList<TypeOnStack>>>>();
            ExpectedStacksAtLabels = new LinqDictionary<Label, LinqList<Tuple<bool, LinqStack<LinqList<TypeOnStack>>>>>();

            CurrentlyInScope = new LinqList<VerifiableTracker>();
            CurrentlyInScope.Add(new VerifiableTracker(beginAt));
        }

        public VerificationResult Mark(Label label)
        {
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
                CurrentlyInScope.Add(newVerifier);
                MarkCreatesNewVerifier = false;
            }

            LinqList<VerifiableTracker> restore;
            if(RestoreOnMark.TryGetValue(label, out restore))
            {
                // don't copy, we want the *exact* same verifiers restore here
                CurrentlyInScope.AddRange(restore);
                RestoreOnMark.Remove(label);
            }

            var based = CurrentlyInScope.FirstOrDefault(f => !f.IsBaseless);
            based = based ?? CurrentlyInScope.First();

            var fromLabel = new VerifiableTracker(label, based.IsBaseless, based);
            CurrentlyInScope.Add(fromLabel);

            if (!VerifyFromLabel.ContainsKey(label))
            {
                VerifyFromLabel[label] = new LinqList<VerifiableTracker>();
            }

            VerifyFromLabel[label].Add(fromLabel);

            return VerificationResult.Successful(null, null);
        }

        public VerificationResult Return()
        {
            CurrentlyInScope = new LinqList<VerifiableTracker>();
            MarkCreatesNewVerifier = true;

            return VerificationResult.Successful(null, null);
        }

        public VerificationResult UnconditionalBranch(Label to)
        {
            var intoVerified = VerifyBranchInto(to);
            if (intoVerified != null)
            {
                return intoVerified;
            }

            UpdateRestores(to);

            if (!RestoreOnMark.ContainsKey(to))
            {
                RestoreOnMark[to] = new LinqList<VerifiableTracker>();
            }

            if (!ExpectedStacksAtLabels.ContainsKey(to))
            {
                ExpectedStacksAtLabels[to] = new LinqList<Tuple<bool, LinqStack<LinqList<TypeOnStack>>>>();
            }
            ExpectedStacksAtLabels[to].Add(GetCurrentStack());

            var verify = CheckStackMatches(to);
            if (verify != null)
            {
                return verify;
            }

            RestoreOnMark[to].AddRange(CurrentlyInScope);
            CurrentlyInScope = new LinqList<VerifiableTracker>();
            MarkCreatesNewVerifier = true;

            return VerificationResult.Successful(null, null);
        }

        public VerificationResult ConditionalBranch(params Label[] toLabels)
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
                    }

                    RestoreOnMark[to].AddRange(CurrentlyInScope.Select(t => t.Clone()));
                }

                if (!ExpectedStacksAtLabels.ContainsKey(to))
                {
                    ExpectedStacksAtLabels[to] = new LinqList<Tuple<bool, LinqStack<LinqList<TypeOnStack>>>>();
                }
                ExpectedStacksAtLabels[to].Add(GetCurrentStack());

                var verify = CheckStackMatches(to);
                if (verify != null)
                {
                    return verify;
                }
            }

            return VerificationResult.Successful(null, null);
        }

        private Tuple<bool, LinqStack<LinqList<TypeOnStack>>> GetCurrentStack()
        {
            Tuple<bool, LinqStack<LinqList<TypeOnStack>>> ret = null;
            foreach (var c in CurrentlyInScope.AsEnumerable())
            {
                var res = c.CollapseAndVerify();

                var stack = res.Stack;

                var stackCopy = new LinqStack<LinqList<TypeOnStack>>(stack.Count);
                for (var i = stack.Count - 1; i >= 0; i--)
                {
                    var toCopy = stack.ElementAt(i);
                    stackCopy.Push(toCopy.Select(x => x).ToList());
                }

                var innerRet = Tuple.Create(c.IsBaseless, stackCopy);

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
                var mismatch = CompareStacks(actual, exp);
                if (mismatch != null)
                {
                    return mismatch;
                }
            }

            return null;
        }

        private VerificationResult CompareStacks(Tuple<bool, LinqStack<LinqList<TypeOnStack>>> actual, Tuple<bool, LinqStack<LinqList<TypeOnStack>>> expected)
        {
            if (!actual.Item1 && !expected.Item1)
            {
                if (expected.Item2.Count != actual.Item2.Count)
                {
                    // Both stacks are based, so the wrong size is a serious error as well
                    return VerificationResult.FailureUnderflow(null, 0, 0, null);
                }
            }

            for (var i = 0; i < expected.Item2.Count; i++)
            {
                if (i >= actual.Item2.Count && !actual.Item1)
                {
                    // actual is based and expected wanted a value, this is an UNDERFLOW
                    return VerificationResult.FailureUnderflow(null, 0, 0, null);
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
                    return VerificationResult.FailureTypeMismatch(null, 0, 0, null, null);
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

        public VerificationResult Transition(InstructionAndTransitions legalTransitions)
        {
            VerificationResult last = null;
            foreach (var x in CurrentlyInScope.AsEnumerable())
            {
                var inner = x.Transition(legalTransitions);

                if (!inner.Success) return inner;

                last = inner;
            }

            return last;
        }

        public LinqStack<TypeOnStack> InferStack(int ofDepth)
        {
            return CurrentlyInScope.First().InferStack(ofDepth);
        }
    }
}
