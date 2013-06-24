using System;

namespace Sigil.Impl
{
    internal sealed class PrefixTracker
    {
        public bool HasUnaligned { get; private set; }
        public int? Unaligned { get; private set; }

        public bool HasVolatile { get; private set; }

        public bool HasReadOnly { get; private set; }

        public bool HasTailCall { get; private set; }

        public bool HasConstrained { get; private set; }
        public Type Constrained { get; private set; }

        public void SetUnaligned(int a)
        {
            HasUnaligned = true;
            Unaligned = a;
        }

        public void SetVolatile()
        {
            HasVolatile = true;
        }

        public void SetReadOnly()
        {
            HasReadOnly = true;
        }

        public void SetTailCall()
        {
            HasTailCall = true;
        }

        public void SetConstrained(Type t)
        {
            HasConstrained = true;
            Constrained = t;
        }

        public void Clear()
        {
            HasUnaligned = false;
            Unaligned = -1;
            HasVolatile = false;
            HasReadOnly = false;
            HasConstrained = false;
            Constrained = null;
        }

        public PrefixTracker Clone()
        {
            return
                new PrefixTracker
                {
                    Constrained = Constrained,
                    HasConstrained = HasConstrained,
                    HasReadOnly = HasReadOnly,
                    HasTailCall = HasTailCall,
                    HasUnaligned = HasUnaligned,
                    HasVolatile = HasVolatile,
                    Unaligned = Unaligned
                };
        }
    }
}
