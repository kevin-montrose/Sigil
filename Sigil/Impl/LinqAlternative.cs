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
        private sealed class DescendingComparer<T> : IComparer<T>
        {
            public static readonly IComparer<T> Default = new DescendingComparer<T>();

            private DescendingComparer() { }

            public int Compare(T x, T y)
            {
                return Comparer<T>.Default.Compare(y, x);
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

        private static IEnumerable<T> _Where<T>(IEnumerable<T> e, SigilFunc<T, bool> pred)
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

        public static LinqRoot<T> Where<T>(IEnumerable<T> e, SigilFunc<T, bool> p)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (p == null) throw new ArgumentNullException("p");

            return LinqEnumerable<T>.For(_Where(e, p));
        }

        public static int Count<T>(IEnumerable<T> e)
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

        public static int Count<T>(IEnumerable<T> e, SigilFunc<T, bool> p)
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

        public static bool Any<T>(IEnumerable<T> e, SigilFunc<T, bool> p)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (p == null) throw new ArgumentNullException("p");

            var asList = e as IList<T>;
            if (asList != null)
            {
                for (var i = 0; i < asList.Count; i++)
                {
                    if (p(asList[i])) return true;
                }

                return false;
            }

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

        public static IEnumerable<V> _Select<T, V>(IEnumerable<T> e, SigilFunc<T, V> p)
        {
            using (var i = e.GetEnumerator())
            {
                while (i.MoveNext())
                {
                    yield return p(i.Current);
                }
            }
        }

        public static LinqRoot<V> Select<T, V>(IEnumerable<T> e, SigilFunc<T, V> p)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (p == null) throw new ArgumentNullException("p");

            return LinqEnumerable<V>.For(_Select(e, p));
        }

        public static IEnumerable<V> _Select<T, V>(IEnumerable<T> e, SigilFunc<T, int, V> p)
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

        public static LinqRoot<V> Select<T, V>(IEnumerable<T> e, SigilFunc<T, int, V> p)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (p == null) throw new ArgumentNullException("p");

            return LinqEnumerable<V>.For(_Select(e, p));
        }

        public static LinqList<T> ToList<T>(IEnumerable<T> e)
        {
            if (e == null) throw new ArgumentNullException();

            LinqList<T> ret;

            var col = e as ICollection<T>;
            if (col != null)
            {
                ret = new LinqList<T>(col.Count);
            }
            else
            {
                ret = new LinqList<T>();
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

        public static T[] ToArray<T>(IEnumerable<T> e)
        {
            var l = ToList(e);

            var ret = new T[l.Count];
            l.CopyTo(ret);

            return ret;
        }

        public static LinqDictionary<Key, Value> ToDictionary<T, Key, Value>(IEnumerable<T> e, SigilFunc<T, Key> k, SigilFunc<T, Value> v)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (k == null) throw new ArgumentNullException("k");
            if (v == null) throw new ArgumentNullException("v");

            var ret = new LinqDictionary<Key, Value>();

            using (var i = e.GetEnumerator())
            {
                while (i.MoveNext())
                {
                    ret.Add(k(i.Current), v(i.Current));
                }
            }

            return ret;
        }

        public static LinqRoot<T> AsEnumerable<T>(LinqRoot<T> e)
        {
            if (e == null) throw new ArgumentNullException("e");

            return e;
        }

        public static T FirstOrDefault<T>(IEnumerable<T> e)
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

        public static T FirstOrDefault<T>(IEnumerable<T> e, SigilFunc<T, bool> p)
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

        public static T First<T>(IEnumerable<T> e)
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

        public static T ElementAt<T>(IEnumerable<T> e, int n)
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

        public static T Last<T>(IEnumerable<T> e)
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

        public static LinqRoot<T> Reverse<T>(IEnumerable<T> e)
        {
            if (e == null) throw new ArgumentNullException("e");

            return LinqEnumerable<T>.For(_Reverse(e));
        }

        public static LinqRoot<T> Reverse<T>(LinqRoot<T> e)
        {
            return Reverse(e != null ? e.AsEnumerable() : null);
        }

        public static LinqRoot<T> Reverse<T>(LinqArray<T> e)
        {
            return Reverse(e != null ? e.AsEnumerable() : null);
        }

        public static bool All<T>(IEnumerable<T> e, SigilFunc<T, bool> p)
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

        public static LinqRoot<T> Skip<T>(IEnumerable<T> e, int n)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (n < 0) throw new ArgumentException("n");

            return LinqEnumerable<T>.For(_Skip(e, n));
        }

        public static T Single<T>(IEnumerable<T> e)
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

        public static T SingleOrDefault<T>(IEnumerable<T> e)
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

        public static T SingleOrDefault<T>(IEnumerable<T> e, SigilFunc<T, bool> p)
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

        public static LinqRoot<T> Cast<T>(IEnumerable e)
        {
            if (e == null) throw new ArgumentNullException("e");

            return LinqEnumerable<T>.For(_Cast<T>(e));
        }

        public static bool Contains<T>(IEnumerable<T> e, T a)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (a == null) throw new ArgumentNullException("a");

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

        public static V Aggregate<T, V>(IEnumerable<T> e, V seed, SigilFunc<V, T, V> a)
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

        private static IEnumerable<V> _SelectMany<T, V>(IEnumerable<T> e, SigilFunc<T, IEnumerable<V>> p)
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

        public static LinqRoot<V> SelectMany<T, V>(IEnumerable<T> e, SigilFunc<T, IEnumerable<V>> p)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (p == null) throw new ArgumentNullException("p");

            return LinqEnumerable<V>.For(_SelectMany(e, p));
        }

        private static IEnumerable<T> _Distinct<T>(IEnumerable<T> e, IEqualityComparer<T> c)
        {
#if COREFX
            return System.Linq.Enumerable.Distinct(e, c);
#else
            var h = new Hashtable();
            using (var i = e.GetEnumerator())
            {
                while (i.MoveNext())
                {
                    if (!h.Contains(i.Current))
                    {
                        h.Add(i.Current, "");

                        yield return i.Current;
                    }
                }
            }
#endif
        }

        public static LinqRoot<T> Distinct<T>(IEnumerable<T> e)
        {
            if (e == null) throw new ArgumentNullException("e");

            return LinqEnumerable<T>.For(_Distinct(e, EqualityComparer<T>.Default));
        }

        public static LinqRoot<T> Distinct<T>(IEnumerable<T> e, IEqualityComparer<T> c)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (c == null) throw new ArgumentNullException("c");

            return LinqEnumerable<T>.For(_Distinct(e, c));
        }

        public static LinqRoot<IGrouping<K, T>> GroupBy<T, K>(IEnumerable<T> e, SigilFunc<T, K> p)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (p == null) throw new ArgumentNullException("p");

            var ret = new LinqList<IGrouping<K, T>>();
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

        private static IEnumerable<T> _QuickSort<T, V>(T[] data, int[] ixs, V[] keys, IComparer<V> c)
        {
            var nextYield = 0;

            var stack = new Stack<SigilTuple<int, int>>();
            stack.Push(SigilTuple.Create(0, ixs.Length - 1));
            while (stack.Count > 0)
            {
                var leftRight = stack.Pop();
                var left = leftRight.Item1;
                var right = leftRight.Item2;
                if (right > left)
                {
                    int pivot = left + (right - left) / 2;
                    int pivotPosition = _Partition(ixs, keys, left, right, pivot, c);
                    stack.Push(SigilTuple.Create(pivotPosition + 1, right));
                    stack.Push(SigilTuple.Create(left, pivotPosition - 1));
                }
                else
                {
                    while (nextYield <= right)
                    {
                        yield return data[ixs[nextYield]];
                        nextYield++;
                    }
                }
            }
        }

        private static int _Partition<V>(int[] ixs, V[] keys, int left, int right, int pivot, IComparer<V> c)
        {
            var pivotIndex = ixs[pivot];
            var pivotKey = keys[pivotIndex];

            ixs[pivot] = ixs[right];
            ixs[right] = pivotIndex;

            var storeIndex = left;

            for (var i = left; i < right; i++)
            {
                var candidateIndex = ixs[i];
                var candidateKey = keys[candidateIndex];
                var comparison = c.Compare(candidateKey, pivotKey);
                if (comparison < 0 || (comparison == 0 && candidateIndex < pivotIndex))
                {
                    ixs[i] = ixs[storeIndex];
                    ixs[storeIndex] = candidateIndex;
                    storeIndex++;
                }
            }

            var tmp = ixs[storeIndex];
            ixs[storeIndex] = ixs[right];
            ixs[right] = tmp;

            return storeIndex;
        }

        private static IEnumerable<T> _Order<T, V>(IEnumerable<T> e, SigilFunc<T, V> p, IComparer<V> c)
        {
            var data = ToArray(e);

            var length = data.Length;

            var indexes = new int[length];
            for (int i = 0; i < length; i++)
            {
                indexes[i] = i;
            }

            var keys = new V[length];
            for (int i = 0; i < length; i++)
            {
                keys[i] = p(data[i]);
            }

            return _QuickSort(data, indexes, keys, c);
        }

        public static LinqRoot<T> OrderBy<T, V>(IEnumerable<T> e, SigilFunc<T, V> p)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (p == null) throw new ArgumentNullException("p");

            return LinqEnumerable<T>.For(_Order(e, p, Comparer<V>.Default));
        }

        public static LinqRoot<T> OrderByDescending<T, V>(IEnumerable<T> e, SigilFunc<T, V> p)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (p == null) throw new ArgumentNullException("p");

            return LinqEnumerable<T>.For(_Order(e, p, DescendingComparer<V>.Default));
        }

        public static void Each<T>(IEnumerable<T> e, Action<T> a)
        {
            var arr = e as T[];
            if (arr != null)
            {
                for (var i = 0; i < arr.Length; i++)
                {
                    a(arr[i]);
                }

                return;
            }

            foreach (var x in e)
            {
                a(x);
            }
        }
    }
}