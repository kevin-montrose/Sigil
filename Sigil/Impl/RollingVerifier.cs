using System;

namespace Sigil.Impl
{
    internal class RollingVerifier
    {
        // Copy (never changing) of the validators that were in scope when a label was marked
        private LinqDictionary<Label, LinqList<VerifiableTracker>> StateAtMark;
        private LinqDictionary<Label, LinqList<VerifiableTracker>> BecomesInScopeOnMark;

        // The validators that begin at a label, that carry state forward from validators that existed when the label was marked
        private LinqDictionary<Label, LinqList<VerifiableTracker>> StateFromMark;

        private LinqList<VerifiableTracker> CurrentlyInScope;

        public RollingVerifier(Label beginAt)
        {
            StateAtMark = new LinqDictionary<Label, LinqList<VerifiableTracker>>();
            StateFromMark = new LinqDictionary<Label, LinqList<VerifiableTracker>>();
            BecomesInScopeOnMark = new LinqDictionary<Label, LinqList<VerifiableTracker>>();

            CurrentlyInScope = new LinqList<VerifiableTracker>();
            CurrentlyInScope.Add(new VerifiableTracker(beginAt));
        }

        private VerificationResult VerifyCurrent()
        {
            VerificationResult last = null;
            foreach (var x in CurrentlyInScope.AsEnumerable())
            {
                var inner = x.CollapseAndVerify();
                if (!inner.Success)
                {
                    return inner;
                }

                last = inner;
            }

            foreach(var x in CurrentlyInScope.AsEnumerable())
            {
                foreach (var y in CurrentlyInScope.AsEnumerable())
                {
                    if (x == y) continue;

                    var compat = x.AreCompatible(y);
                    if (compat != null)
                    {
                        return compat;
                    }
                }
            }

            return last;
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

        public VerificationResult UnconditionalBranch(Label to)
        {
            var copy = CurrentlyInScope.Select(e => e.Clone()).ToList();

            if (!BecomesInScopeOnMark.ContainsKey(to))
            {
                BecomesInScopeOnMark[to] = new LinqList<VerifiableTracker>();
            }

            BecomesInScopeOnMark[to].AddRange(copy);

            var ret =
                CheckMarkStateCompatible(to) ??
                VerifyCurrent() ??
                VerificationResult.Successful(null, null);

            CurrentlyInScope = new LinqList<VerifiableTracker>();

            return ret;
        }

        public VerificationResult ConditionalBranch(params Label[] toLabels)
        {
            var copy = CurrentlyInScope.Select(e => e.Clone()).ToList();

            for (var i = 0; i < toLabels.Length; i++)
            {
                var to = toLabels[i];

                if (!BecomesInScopeOnMark.ContainsKey(to))
                {
                    BecomesInScopeOnMark[to] = new LinqList<VerifiableTracker>();
                }

                BecomesInScopeOnMark[to].AddRange(copy);

                var valid = CheckMarkStateCompatible(to);
                if (valid != null) return valid;
            }

            return VerifyCurrent();
        }

        private VerificationResult CheckMarkStateCompatible(Label branchingTo)
        {
            if (StateAtMark.ContainsKey(branchingTo))
            {
                var atMark = StateAtMark[branchingTo];

                foreach (var x in atMark.AsEnumerable())
                {
                    foreach (var y in CurrentlyInScope.AsEnumerable())
                    {
                        var compat = x.AreCompatible(y);

                        if (compat != null)
                        {
                            return compat;
                        }
                    }
                }
            }

            if (StateFromMark.ContainsKey(branchingTo))
            {
                var fromMark = StateFromMark[branchingTo];

                foreach (var x in CurrentlyInScope.AsEnumerable())
                {
                    foreach (var y in fromMark.AsEnumerable())
                    {
                        var combined = x.Concat(y);
                        var result = combined.CollapseAndVerify();

                        if (!result.Success)
                        {
                            return result;
                        }
                    }
                }
            }

            return null;
        }

        public VerificationResult Mark(Label label)
        {
            if (CurrentlyInScope.Count == 0)
            {
                CurrentlyInScope.Add(new VerifiableTracker(label, baseless: true));
            }

            if (BecomesInScopeOnMark.ContainsKey(label))
            {
                var nowInScope = BecomesInScopeOnMark[label];
                CurrentlyInScope.AddRange(nowInScope);
            }

            StateAtMark[label] = CurrentlyInScope.Select(c => c.Clone()).ToList();

            var fromMark = CurrentlyInScope.Select(c => new VerifiableTracker(label, c.IsBaseless, c)).ToList();
            StateFromMark[label] = fromMark;
            
            CurrentlyInScope.AddRange(fromMark);

            return VerifyCurrent();
        }

        public LinqStack<TypeOnStack> InferStack(int ofDepth)
        {
            return CurrentlyInScope.First().InferStack(ofDepth);
        }
    }
}
