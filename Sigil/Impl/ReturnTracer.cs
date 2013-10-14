using System.Collections.Generic;
using System.Reflection.Emit;

namespace Sigil.Impl
{
    internal class ReturnTracerResult
    {
        private class LabelEnumerableComparer : IEqualityComparer<LinqRoot<Label>>
        {
            public static readonly LabelEnumerableComparer Singleton = new LabelEnumerableComparer();

            private LabelEnumerableComparer() { }

            public bool Equals(LinqRoot<Label> x, LinqRoot<Label> y)
            {
                if (x == y) return true;
                if (x == null || y == null) return false;

                if (x.Count() != y.Count()) return false;

                using(var eX = x.AsEnumerable().GetEnumerator())
                using (var eY = y.AsEnumerable().GetEnumerator())
                {
                    while (eX.MoveNext() && eY.MoveNext())
                    {
                        if (eX.Current != eY.Current) return false;
                    }
                }

                return true;
            }

            public int GetHashCode(LinqRoot<Label> obj)
            {
                if (obj == null) return 0;

                var ret = 0;

                foreach (var label in obj.AsEnumerable())
                {
                    ret ^= label.GetHashCode();
                }

                return ret;
            }
        }

        public bool IsSuccess { get; private set; }
        public LinqRoot<LinqRoot<Label>> FailingPaths { get; private set; }

        private ReturnTracerResult() { }

        public static ReturnTracerResult Success()
        {
            return
                new ReturnTracerResult
                {
                    IsSuccess = true
                };
        }

        public static ReturnTracerResult Failure(LinqList<Label> path)
        {
            return
                new ReturnTracerResult
                {
                    IsSuccess = false,

                    FailingPaths = (LinqArray<LinqRoot<Label>>)new [] { path.ToList() }
                };
        }

        public static ReturnTracerResult Combo(params ReturnTracerResult[] other)
        {
            var asArr = (LinqArray<ReturnTracerResult>)other;

            if (asArr.All(r => r.IsSuccess)) return Success();

            var allPaths = asArr.Where(r => !r.IsSuccess).SelectMany(r => r.FailingPaths).ToList();

            var uniqePaths = allPaths.Distinct(LabelEnumerableComparer.Singleton).ToList();

            return
                new ReturnTracerResult
                {
                    IsSuccess = false,

                    FailingPaths = uniqePaths
                };
        }
    }

    internal class ReturnTracer
    {
        private LinqList<SigilTuple<OpCode, Label, int>> Branches;
        private LinqDictionary<Label, int> Marks;
        private LinqList<int> Returns;
        private LinqList<int> Throws;

        public ReturnTracer(LinqList<SigilTuple<OpCode, Label, int>> branches, LinqDictionary<Label, int> marks, LinqList<int> returns, LinqList<int> throws) 
        {
            Branches = branches;
            Marks = marks;
            Returns = returns;
            Throws = throws;
        }

        private static bool IsUnconditionalBranch(OpCode op)
        {
            return 
                op == OpCodes.Br ||
                op == OpCodes.Br_S ||
                op == OpCodes.Leave ||
                op == OpCodes.Leave_S;
        }

        private Dictionary<int, ReturnTracerResult> Cache = new Dictionary<int, ReturnTracerResult>();

        private ReturnTracerResult TraceFrom(int startAt, LinqList<Label> path, LinqHashSet<Label> pathLookup)
        {
            ReturnTracerResult cached;
            if (Cache.TryGetValue(startAt, out cached))
            {
                return cached;
            }

            var nextBranches = Branches.Where(b => b.Item3 >= startAt).GroupBy(g => g.Item3).OrderBy(x => x.Key).FirstOrDefault();
            var orReturn = Returns.Where(ix => ix >= startAt && (nextBranches != null ? ix < nextBranches.Key : true)).Count();
            var orThrow = Throws.Where(ix => ix >= startAt && (nextBranches != null ? ix < nextBranches.Key : true)).Count();

            if (orReturn != 0)
            {
                Cache[startAt] = cached = ReturnTracerResult.Success();
                return cached;
            }

            if (orThrow != 0)
            {
                Cache[startAt] = cached = ReturnTracerResult.Success();
                return cached;
            }

            if (nextBranches == null)
            {
                Cache[startAt] = cached = ReturnTracerResult.Failure(path);
                return cached;
            }

            var ret = new LinqList<ReturnTracerResult>();

            foreach (var nextBranch in nextBranches)
            {

                if (pathLookup.Contains(nextBranch.Item2))
                {
                    Cache[startAt] = cached = ReturnTracerResult.Success();
                    ret.Add(cached);
                    continue;
                }

                var branchOp = nextBranch.Item1;

                var branchTo = Marks[nextBranch.Item2];

                var removeFromPathAt = path.Count;
                path.Add(nextBranch.Item2);
                pathLookup.Add(nextBranch.Item2);

                var fromFollowingBranch = TraceFrom(branchTo, path, pathLookup);

                path.RemoveAt(removeFromPathAt);
                pathLookup.Remove(nextBranch.Item2);

                if (IsUnconditionalBranch(branchOp))
                {
                    Cache[startAt] = cached = fromFollowingBranch;
                    //return cached;
                    ret.Add(cached);
                    continue;
                }

                var fromFallingThrough = TraceFrom(startAt + 1, path, pathLookup);

                Cache[startAt] = cached = ReturnTracerResult.Combo(fromFallingThrough, fromFollowingBranch);

                ret.Add(cached);
            }

            Cache[startAt] = cached = ReturnTracerResult.Combo(ret.ToArray());
            return cached;
        }

        public ReturnTracerResult Verify()
        {
            var firstLabel = Marks.OrderBy(o => o.Value).First().Key;
            var firstIx = Marks[firstLabel];

            var path = new LinqList<Label>();
            path.Add(firstLabel);
            var pathLookup = new LinqHashSet<Label>(path.AsEnumerable());

            return TraceFrom(firstIx, path, pathLookup);
        }
    }
}
