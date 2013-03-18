using System;
using System.Collections.Generic;

namespace Sigil.Impl
{
    internal class TrackerDescriber
    {
        private LinqList<SigilTuple<VerifiableTracker, int>> Tokens;

        private TrackerDescriber(IEnumerable<VerifiableTracker> trackers)
        {
            Tokens = new LinqList<SigilTuple<VerifiableTracker, int>>();

            foreach (var tracker in trackers)
            {
                Tokens.Add(SigilTuple.Create(tracker, tracker.Iteration));
            }
        }

        public override int GetHashCode()
        {
            var ret = 0;

            for (var i = 0; i < Tokens.Count; i++)
            {
                ret ^= Tokens[i].Item1.GetHashCode() + Tokens[i].Item2.GetHashCode();
            }

            return ret;
        }

        public override bool Equals(object obj)
        {
            var other = obj as TrackerDescriber;
            if (other == null) return false;

            if (other.Tokens.Count != Tokens.Count) return false;

            for (var i = 0; i < other.Tokens.Count; i++)
            {
                if (other.Tokens[i].Item1 != Tokens[i].Item1 || other.Tokens[i].Item2 != Tokens[i].Item2) return false;
            }

            return true;
        }

        private static IEnumerable<VerifiableTracker> Enumerate(VerifiableTrackerConcatPromise promise)
        {
            while (promise != null)
            {
                yield return promise.Inner;

                promise = promise.Next;
            }
        }

        public static TrackerDescriber Get(VerifiableTrackerConcatPromise promise)
        {
            return new TrackerDescriber(Enumerate(promise));
        }
    }

    internal class VerifiableTrackerConcatPromise
    {
        public TrackerDescriber Description { get { return TrackerDescriber.Get(this); } }

        internal VerifiableTracker Inner;
        internal VerifiableTrackerConcatPromise Next;
        private HashSet<VerifiableTracker> ContainsLookup = new HashSet<VerifiableTracker>();

        public VerifiableTrackerConcatPromise(VerifiableTracker a)
        {
            Inner = a;
            ContainsLookup.Add(Inner);
        }

        private VerifiableTrackerConcatPromise(VerifiableTracker a, VerifiableTrackerConcatPromise next)
            : this(a)
        {
            Next = next;

            foreach(var x in Next.ContainsLookup)
            {
                ContainsLookup.Add(x);
            }
        }

        public VerifiableTrackerConcatPromise Concat(VerifiableTrackerConcatPromise next)
        {
            return new VerifiableTrackerConcatPromise(Inner, next);
        }

        public bool Contains(VerifiableTrackerConcatPromise v)
        {
            if (!ContainsLookup.Contains(Inner)) return false;

            var thisHead = this;
            var otherHead = v;

            // scan forward until we find an overlap
            while (otherHead != null)
            {
                if (thisHead.Inner == otherHead.Inner) break;

                otherHead = otherHead.Next;
            }

            // didn't find any overlap
            if (otherHead == null) return false;

            // scan forward until one of them ends ...
            while (thisHead != null && otherHead != null)
            {
                // ... or there's a mismatch
                if (thisHead.Inner != otherHead.Inner) return false;

                thisHead = thisHead.Next;
                otherHead = otherHead.Next;
            }

            // we run through all of the other without finding a mismatch
            return otherHead == null && thisHead != null;
        }

        public VerifiableTracker DePromise()
        {
            if (Next == null) return Inner;

            var tail = Next.DePromise();

            return Inner.Concat(tail);
        }

        public override string ToString()
        {
            return
                Inner.ToString() +
                (Next != null ? "\r\n" + Next.ToString() : "");
        }

        public bool ContainsUsageOf(Label label)
        {
            return
                Inner.ContainsUsageOf(label) ||
                (Next != null ? Next.ContainsUsageOf(label) : false);
        }
    }
}
