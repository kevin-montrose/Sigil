using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Sigil.Impl
{
    internal class ReturnTracerResult
    {
        public bool IsSuccess { get; private set; }
        public IEnumerable<IEnumerable<Label>> FailingPaths { get; private set; }

        private ReturnTracerResult() { }

        public static ReturnTracerResult Success()
        {
            return
                new ReturnTracerResult
                {
                    IsSuccess = true
                };
        }

        public static ReturnTracerResult Failure(IEnumerable<Label> path)
        {
            return
                new ReturnTracerResult
                {
                    IsSuccess = false,

                    FailingPaths = new [] { path.ToList() }
                };
        }

        public static ReturnTracerResult Combo(params ReturnTracerResult[] other)
        {
            if (other.All(r => r.IsSuccess)) return Success();

            var badPaths = other.Where(r => !r.IsSuccess).SelectMany(r => r.FailingPaths);

            return
                new ReturnTracerResult
                {
                    IsSuccess = false,

                    FailingPaths = badPaths
                };
        }
    }

    internal class ReturnTracer
    {
        private List<Tuple<OpCode, Label, int>> Branches;
        private Dictionary<Label, int> Marks;
        private List<int> Returns;

        public ReturnTracer(List<Tuple<OpCode, Label, int>> branches, Dictionary<Label, int> marks, List<int> returns) 
        {
            Branches = branches;
            Marks = marks;
            Returns = returns;
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

        private ReturnTracerResult TraceFrom(int startAt, List<Label> path, HashSet<Label> pathLookup)
        {
            ReturnTracerResult cached;
            if (Cache.TryGetValue(startAt, out cached))
            {
                return cached;
            }

            var nextBranch = Branches.Where(b => b.Item3 >= startAt).OrderBy(x => x.Item3).FirstOrDefault();
            var orReturn = Returns.Where(ix => ix >= startAt && (nextBranch != null ? ix < nextBranch.Item3 : true)).Count();

            if (orReturn != 0)
            {
                Cache[startAt] = cached = ReturnTracerResult.Success();
                return cached;
            }

            if (nextBranch == null)
            {
                Cache[startAt] = cached = ReturnTracerResult.Failure(path);
                return cached;
            }

            if (pathLookup.Contains(nextBranch.Item2)) return ReturnTracerResult.Success();

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
                return cached;
            }

            var fromFallingThrough = TraceFrom(startAt + 1, path, pathLookup);

            Cache[startAt] = cached = ReturnTracerResult.Combo(fromFallingThrough, fromFollowingBranch);

            return cached;
        }

        public ReturnTracerResult Verify()
        {
            var firstLabel = Marks.OrderBy(o => o.Value).First().Key;
            var firstIx = Marks[firstLabel];

            var path = new List<Label>();
            path.Add(firstLabel);
            var pathLookup = new HashSet<Label>(path);

            return TraceFrom(firstIx, path, pathLookup);
        }
    }
}
