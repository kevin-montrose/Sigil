using System;
using System.Collections;
using System.Collections.Generic;

namespace Sigil.Impl
{
    internal interface IGrouping<K, V> : IEnumerable<V>
    {
        K Key { get; }
    }

    // Linq doesn't exist in .NET 3.0 and below; here implementing the operators we need
    internal static class LinqAlternative
    {
        private sealed class AscendingOrderComparer<A, B> : IComparer<Tuple<A, B>>
        {
            public static readonly IComparer<Tuple<A, B>> Singleton = new AscendingOrderComparer<A, B>();

            private AscendingOrderComparer() { }

            public int Compare(Tuple<A, B> x, Tuple<A, B> y)
            {
                return Comparer<B>.Default.Compare(x.Item2, y.Item2);
            }
        }

        private sealed class DescendingOrderComparer<A, B> : IComparer<Tuple<A, B>>
        {
            public static readonly IComparer<Tuple<A, B>> Singleton = new DescendingOrderComparer<A, B>();

            private DescendingOrderComparer() { }

            public int Compare(Tuple<A, B> x, Tuple<A, B> y)
            {
                return Comparer<B>.Default.Compare(y.Item2, x.Item2);
            }
        }

        private sealed class Grouping<K, V> : IGrouping<K, V>
        {
            public K Key { get; private set; }
            private List<V> Values;

            internal Grouping(K key)
            {
                Key = key;
                Values = new List<V>();
            }

            internal void Add(V i)
            {
                Values.Add(i);
            }

            public IEnumerator<V> GetEnumerator()
            {
                return Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)Values).GetEnumerator();
            }
        }

        private static IEnumerable<T> _Where<T>(IEnumerable<T> e, Func<T, bool> pred)
        {
            using (var i = e.GetEnumerator())
            {
                while (i.MoveNext())
                {
                    if (pred(i.Current))
                    {
                        yield return i.Current;
                    }
                }
            }
        }

        public static IEnumerable<T> Where<T>(this IEnumerable<T> e, Func<T, bool> p)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (p == null) throw new ArgumentNullException("p");

            return _Where(e, p);
        }

        public static int Count<T>(this IEnumerable<T> e)
        {
            if (e == null) throw new ArgumentNullException("e");

            int ret = 0;

            using (var i = e.GetEnumerator())
            {
                while (i.MoveNext())
                {
                    ret++;
                }
            }

            return ret;
        }

        public static int Count<T>(this IEnumerable<T> e, Func<T, bool> p)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (p == null) throw new ArgumentNullException("p");

            int ret = 0;

            using (var i = e.GetEnumerator())
            {
                while (i.MoveNext())
                {
                    if (p(i.Current))
                    {
                        ret++;
                    }
                }
            }

            return ret;
        }

        public static bool Any<T>(this IEnumerable<T> e, Func<T, bool> p)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (p == null) throw new ArgumentNullException("p");

            using (var i = e.GetEnumerator())
            {
                while (i.MoveNext())
                {
                    if (p(i.Current))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static IEnumerable<V> _Select<T, V>(this IEnumerable<T> e, Func<T, V> p)
        {
            using (var i = e.GetEnumerator())
            {
                while (i.MoveNext())
                {
                    yield return p(i.Current);
                }
            }
        }

        public static IEnumerable<V> Select<T, V>(this IEnumerable<T> e, Func<T, V> p)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (p == null) throw new ArgumentNullException("p");

            return _Select(e, p);
        }

        public static IEnumerable<V> _Select<T, V>(this IEnumerable<T> e, Func<T, int, V> p)
        {
            using (var i = e.GetEnumerator())
            {
                var x = 0;
                while (i.MoveNext())
                {
                    yield return p(i.Current, x);
                    x++;
                }
            }
        }

        public static IEnumerable<V> Select<T, V>(this IEnumerable<T> e, Func<T, int, V> p)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (p == null) throw new ArgumentNullException("p");

            return _Select(e, p);
        }

        public static List<T> ToList<T>(this IEnumerable<T> e)
        {
            if (e == null) throw new ArgumentNullException();

            List<T> ret;

            var col = e as ICollection<T>;
            if (col != null)
            {
                ret = new List<T>(col.Count);
            }
            else
            {
                ret = new List<T>();
            }

            using (var i = e.GetEnumerator())
            {
                while (i.MoveNext())
                {
                    ret.Add(i.Current);
                }
            }

            return ret;
        }

        public static T[] ToArray<T>(this IEnumerable<T> e)
        {
            var l = ToList(e);

            var ret = new T[l.Count];
            l.CopyTo(ret);

            return ret;
        }

        public static Dictionary<Key, Value> ToDictionary<T, Key, Value>(this IEnumerable<T> e, Func<T, Key> k, Func<T, Value> v)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (k == null) throw new ArgumentNullException("k");
            if (v == null) throw new ArgumentNullException("v");

            var ret = new Dictionary<Key, Value>();

            using (var i = e.GetEnumerator())
            {
                while (i.MoveNext())
                {
                    ret.Add(k(i.Current), v(i.Current));
                }
            }

            return ret;
        }

        public static IEnumerable<T> AsEnumerable<T>(this IEnumerable<T> e)
        {
            if (e == null) throw new ArgumentNullException("e");

            return e;
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> e)
        {
            if (e == null) throw new ArgumentNullException("e");

            using (var i = e.GetEnumerator())
            {
                if (!i.MoveNext())
                {
                    return default(T);
                }

                return i.Current;
            }
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> e, Func<T, bool> p)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (p == null) throw new ArgumentNullException("p");

            using (var i = e.GetEnumerator())
            {
                while (i.MoveNext())
                {
                    if (p(i.Current))
                    {
                        return i.Current;
                    }
                }
            }

            return default(T);
        }

        public static T First<T>(this IEnumerable<T> e)
        {
            if (e == null) throw new ArgumentNullException("e");

            using (var i = e.GetEnumerator())
            {
                if (!i.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }

                return i.Current;
            }
        }

        public static T ElementAt<T>(this IEnumerable<T> e, int n)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (n < 0) throw new ArgumentOutOfRangeException();

            using (var i = e.GetEnumerator())
            {
                for (var x = 0; x <= n; x++)
                {
                    if (!i.MoveNext()) throw new ArgumentOutOfRangeException();
                }

                return i.Current;
            }
        }

        public static T Last<T>(this IEnumerable<T> e)
        {
            if (e == null) throw new ArgumentNullException("e");

            var l = e as IList<T>;
            if (l != null)
            {
                if (l.Count == 0) throw new InvalidOperationException();

                return l[l.Count - 1];
            }

            using (var i = e.GetEnumerator())
            {
                bool set = false;
                T last = default(T);

                while (i.MoveNext())
                {
                    set = true;
                    last = i.Current;
                }

                if (!set) throw new InvalidOperationException();

                return last;
            }
        }

        private static IEnumerable<T> _Reverse<T>(IEnumerable<T> e)
        {
            var reversed = new Stack<T>(e);
            using (var i = reversed.GetEnumerator())
            {
                foreach (var x in reversed)
                {
                    yield return x;
                } 
            }
        }

        public static IEnumerable<T> Reverse<T>(this IEnumerable<T> e)
        {
            if (e == null) throw new ArgumentNullException("e");

            return _Reverse(e);
        }

        public static bool All<T>(this IEnumerable<T> e, Func<T, bool> p)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (p == null) throw new ArgumentNullException("p");

            using (var i = e.GetEnumerator())
            {
                while (i.MoveNext())
                {
                    if (!p(i.Current)) return false;
                }
            }

            return true;
        }

        private static IEnumerable<T> _Skip<T>(IEnumerable<T> e, int n)
        {
            using (var i = e.GetEnumerator())
            {
                for (int x = 0; x < n; x++)
                {
                    if (!i.MoveNext())
                    {
                        yield break;
                    }
                }

                while (i.MoveNext())
                {
                    yield return i.Current;
                }
            }
        }

        public static IEnumerable<T> Skip<T>(this IEnumerable<T> e, int n)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (n < 0) throw new ArgumentException("n");

            return _Skip(e, n);
        }

        public static T Single<T>(this IEnumerable<T> e)
        {
            if (e == null) throw new ArgumentNullException("e");

            using (var i = e.GetEnumerator())
            {
                if (!i.MoveNext()) throw new InvalidOperationException();

                var ret = i.Current;

                if (i.MoveNext()) throw new InvalidOperationException();

                return ret;
            }
        }

        public static T SingleOrDefault<T>(this IEnumerable<T> e)
        {
            if (e == null) throw new ArgumentNullException("e");

            using (var i = e.GetEnumerator())
            {
                if (!i.MoveNext()) return default(T);

                var ret = i.Current;

                if (i.MoveNext())
                {
                    throw new InvalidOperationException();
                }

                return ret;
            }
        }

        public static T SingleOrDefault<T>(this IEnumerable<T> e, Func<T, bool> p)
        {
            if (e == null) throw new ArgumentNullException("e");

            using (var i = e.GetEnumerator())
            {
                while (i.MoveNext())
                {
                    if (p(i.Current))
                    {
                        var ret = i.Current;

                        while (i.MoveNext())
                        {
                            if (p(i.Current))
                            {
                                throw new InvalidOperationException();
                            }
                        }

                        return ret;
                    }
                }

                return default(T);
            }
        }

        private static IEnumerable<T> _Cast<T>(IEnumerable e)
        {
            var i = e.GetEnumerator();
            
            while (i.MoveNext())
            {
                yield return (T)i.Current;
            }
        }

        public static IEnumerable<T> Cast<T>(this IEnumerable e)
        {
            if (e == null) throw new ArgumentNullException("e");

            return _Cast<T>(e);
        }

        public static bool Contains<T>(this IEnumerable<T> e, T a)
        {
            if (e == null) throw new ArgumentNullException("e");

            var comparer = EqualityComparer<T>.Default;

            using (var i = e.GetEnumerator())
            {
                while (i.MoveNext())
                {
                    if (comparer.Equals(a, i.Current))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static V Aggregate<T, V>(this IEnumerable<T> e, V seed, Func<V, T, V> a)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (a == null) throw new ArgumentNullException("a");

            using (var i = e.GetEnumerator())
            {
                while (i.MoveNext())
                {
                    seed = a(seed, i.Current);
                }
            }

            return seed;
        }

        private static IEnumerable<V> _SelectMany<T, V>(IEnumerable<T> e, Func<T, IEnumerable<V>> p)
        {
            using (var i = e.GetEnumerator())
            {
                while (i.MoveNext())
                {
                    var x = p(i.Current);

                    using (var j = x.GetEnumerator())
                    {
                        while (j.MoveNext())
                        {
                            yield return j.Current;
                        }
                    }
                }
            }
        }

        public static IEnumerable<V> SelectMany<T, V>(this IEnumerable<T> e, Func<T, IEnumerable<V>> p)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (p == null) throw new ArgumentNullException("p");

            return _SelectMany(e, p);
        }

        private static IEnumerable<T> _Distinct<T>(IEnumerable<T> e, IEqualityComparer<T> c)
        {
            var h = new HashSet<T>(c);
            using (var i = e.GetEnumerator())
            {
                while (i.MoveNext())
                {
                    if (h.Add(i.Current))
                    {
                        yield return i.Current;
                    }
                }
            }
        }

        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> e)
        {
            if (e == null) throw new ArgumentNullException("e");

            return _Distinct(e, EqualityComparer<T>.Default);
        }

        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> e, IEqualityComparer<T> c)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (c == null) throw new ArgumentNullException("c");

            return _Distinct(e, c);
        }

        public static IEnumerable<IGrouping<K, T>> GroupBy<T, K>(this IEnumerable<T> e, Func<T, K> p)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (p == null) throw new ArgumentNullException("p");

            var ret = new List<IGrouping<K, T>>();
            var gLookup = new Dictionary<K, Grouping<K, T>>();

            using (var i = e.GetEnumerator())
            {
                while (i.MoveNext())
                {
                    var k = p(i.Current);
                    Grouping<K, T> g;

                    if (!gLookup.TryGetValue(k, out g))
                    {
                        gLookup[k] = g = new Grouping<K, T>(k);

                        ret.Add(g);
                    }

                    g.Add(i.Current);
                }
            }

            return ret;
        }

        private static IEnumerable<T> _OrderBy<T, V>(this IEnumerable<Tuple<T, V>> e)
        {
            var arr = e.ToArray();

            Array.Sort(arr, AscendingOrderComparer<T, V>.Singleton);

            for (var i = 0; i < arr.Length; i++)
            {
                yield return arr[i].Item1;
            }
        }

        public static IEnumerable<T> OrderBy<T, V>(this IEnumerable<T> e, Func<T, V> p)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (p == null) throw new ArgumentNullException("p");

            return _OrderBy(e.Select(x => Tuple.Create(x, p(x))));
        }

        private static IEnumerable<T> _OrderByDescending<T, V>(this IEnumerable<Tuple<T, V>> e)
        {
            var arr = e.ToArray();

            Array.Sort(arr, DescendingOrderComparer<T, V>.Singleton);

            for (var i = 0; i < arr.Length; i++)
            {
                yield return arr[i].Item1;
            }
        }

        public static IEnumerable<T> OrderByDescending<T, V>(this IEnumerable<T> e, Func<T, V> p)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (p == null) throw new ArgumentNullException("p");

            return _OrderByDescending(e.Select(x => Tuple.Create(x, p(x))));
        }
    }
}
