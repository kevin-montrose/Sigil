using System;
using System.Collections.Generic;

namespace Sigil.Impl
{
#if NET35
        internal static class Tuple {
            public static Tuple<T1,T2> Create<T1,T2>(T1 item1, T2 item2) { return new Tuple<T1,T2>(item1, item2); }
            public static Tuple<T1,T2,T3> Create<T1,T2,T3>(T1 item1, T2 item2, T3 item3) { return new Tuple<T1,T2,T3>(item1, item2, item3); }
            public static Tuple<T1,T2,T3,T4> Create<T1,T2,T3,T4>(T1 item1, T2 item2, T3 item3, T4 item4) { return new Tuple<T1,T2,T3,T4>(item1, item2, item3, item4); }
        }

        internal class Tuple<T1,T2> : IEquatable<Tuple<T1,T2>> {
            private readonly T1 item1;
            private readonly T2 item2;

            public T1 Item1 { get { return item1; } }

            public T2 Item2 { get { return item2; } }

            public Tuple(T1 item1, T2 item2) {
                this.item1 = item1;
                this.item2 = item2;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as Tuple<T1,T2>);
            }

            public bool Equals(Tuple<T1,T2> obj)
            {
                if (obj == null) return false;
                if (obj == this) return true;
                return EqualityComparer<T1>.Default.Equals(obj.item1, this.item1)
                    && EqualityComparer<T2>.Default.Equals(obj.item2, this.item2);
            }

            public override int GetHashCode()
            {
                var hash = 13;
                hash = (hash * -17) + EqualityComparer<T1>.Default.GetHashCode(this.item1);
                hash = (hash * -17) + EqualityComparer<T2>.Default.GetHashCode(this.item2);
                return hash;
            }

            public override string ToString()
            {
                return "(" + this.item1 + "," + this.item2 + ")";
            }
        }

        internal class Tuple<T1,T2, T3>  : IEquatable<Tuple<T1,T2,T3>> {
            private readonly T1 item1;
            private readonly T2 item2;
            private readonly T3 item3;

            public T1 Item1 { get { return item1; } }

            public T2 Item2 { get { return item2; } }

            public T3 Item3 { get { return item3; } }

            public Tuple(T1 item1, T2 item2, T3 item3) {
                this.item1 = item1;
                this.item2 = item2;
                this.item3 = item3;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as Tuple<T1, T2, T3>);
            }

            public bool Equals(Tuple<T1, T2, T3> obj)
            {
                if (obj == null) return false;
                if (obj == this) return true;
                return EqualityComparer<T1>.Default.Equals(obj.item1, this.item1)
                    && EqualityComparer<T2>.Default.Equals(obj.item2, this.item2)
                    && EqualityComparer<T3>.Default.Equals(obj.item3, this.item3);
            }

            public override int GetHashCode()
            {
                var hash = 13;
                hash = (hash * -17) + EqualityComparer<T1>.Default.GetHashCode(this.item1);
                hash = (hash * -17) + EqualityComparer<T2>.Default.GetHashCode(this.item2);
                hash = (hash * -17) + EqualityComparer<T3>.Default.GetHashCode(this.item3);
                return hash;
            }

            public override string ToString()
            {
                return "(" + this.item1 + "," + this.item2 + "," + this.item3 + ")";
            }
        }

        internal class Tuple<T1,T2, T3, T4>  : IEquatable<Tuple<T1,T2,T3,T4>>{
            private readonly T1 item1;
            private readonly T2 item2;
            private readonly T3 item3;
            private readonly T4 item4;

            public T1 Item1 { get { return item1; } }

            public T2 Item2 { get { return item2; } }

            public T3 Item3 { get { return item3; } }

            public T4 Item4 { get { return item4; } }

            public Tuple(T1 item1, T2 item2, T3 item3, T4 item4) {
                this.item1 = item1;
                this.item2 = item2;
                this.item3 = item3;
                this.item4 = item4;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as Tuple<T1, T2, T3, T4>);
            }

            public bool Equals(Tuple<T1, T2, T3, T4> obj)
            {
                if (obj == null) return false;
                if (obj == this) return true;
                return EqualityComparer<T1>.Default.Equals(obj.item1, this.item1)
                    && EqualityComparer<T2>.Default.Equals(obj.item2, this.item2)
                    && EqualityComparer<T3>.Default.Equals(obj.item3, this.item3)
                    && EqualityComparer<T4>.Default.Equals(obj.item4, this.item4);
            }

            public override int GetHashCode()
            {
                var hash = 13;
                hash = (hash * -17) + EqualityComparer<T1>.Default.GetHashCode(this.item1);
                hash = (hash * -17) + EqualityComparer<T2>.Default.GetHashCode(this.item2);
                hash = (hash * -17) + EqualityComparer<T3>.Default.GetHashCode(this.item3);
                hash = (hash * -17) + EqualityComparer<T4>.Default.GetHashCode(this.item4);
                return hash;
            }

            public override string ToString()
            {
                return "(" + this.item1 + "," + this.item2 + "," + this.item3 + "," + this.item4 + ")";
            }
        }
#endif
}
