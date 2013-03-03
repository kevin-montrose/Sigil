using System;
using System.Collections.Generic;

namespace Sigil.Impl
{
    internal class VerifiableTrackerConcatPromise
    {
        public VerifiableTracker Inner { get; private set; }
        private VerifiableTrackerConcatPromise Next;
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
