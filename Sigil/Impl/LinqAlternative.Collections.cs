using System;
using System.Collections.Generic;

namespace Sigil.Impl
{
    // Want extension methods in .NET 3.0?  Tough
    internal abstract class LinqRoot<T>
    {
        protected abstract IEnumerable<T> InnerEnumerable();

        public LinqRoot<T> Where(SigilFunc<T, bool> p)
        {
            return LinqAlternative.Where(InnerEnumerable(), p);
        }

        public int Count()
        {
            return LinqAlternative.Count(InnerEnumerable());
        }

        public int Count(SigilFunc<T, bool> p)
        {
            return LinqAlternative.Count(InnerEnumerable(), p);
        }

        public bool Any(SigilFunc<T, bool> p)
        {
            return LinqAlternative.Any(InnerEnumerable(), p);
        }

        public LinqRoot<V> Select<V>(SigilFunc<T, V> p)
        {
            return LinqAlternative.Select(InnerEnumerable(), p);
        }

        public LinqRoot<V> Select<V>(SigilFunc<T, int, V> p)
        {
            return LinqAlternative.Select(InnerEnumerable(), p);
        }

        public LinqList<T> ToList()
        {
            return LinqAlternative.ToList(InnerEnumerable());
        }

        public T[] ToArray()
        {
            return LinqAlternative.ToArray(InnerEnumerable());
        }

        public LinqDictionary<Key, Value> ToDictionary<Key, Value>(SigilFunc<T, Key> k, SigilFunc<T, Value> v)
        {
            return LinqAlternative.ToDictionary(InnerEnumerable(), k, v);
        }

        public IEnumerable<T> AsEnumerable()
        {
            return InnerEnumerable();
        }

        public T FirstOrDefault()
        {
            return LinqAlternative.FirstOrDefault(InnerEnumerable());
        }

        public T FirstOrDefault(SigilFunc<T, bool> p)
        {
            return LinqAlternative.FirstOrDefault(InnerEnumerable(), p);
        }

        public T First()
        {
            return LinqAlternative.First(InnerEnumerable());
        }

        public T ElementAt(int n)
        {
            return LinqAlternative.ElementAt(InnerEnumerable(), n);
        }

        public T Last()
        {
            return LinqAlternative.Last(InnerEnumerable());
        }

        public LinqRoot<T> Reverse()
        {
            return LinqAlternative.Reverse(InnerEnumerable());
        }

        public bool All(SigilFunc<T, bool> p)
        {
            return LinqAlternative.All(InnerEnumerable(), p);
        }

        public LinqRoot<T> Skip(int n)
        {
            return LinqAlternative.Skip(InnerEnumerable(), n);
        }

        public T Single()
        {
            return LinqAlternative.Single(InnerEnumerable());
        }

        public T SingleOrDefault()
        {
            return LinqAlternative.SingleOrDefault(InnerEnumerable());
        }

        public T SingleOrDefault(SigilFunc<T, bool> p)
        {
            return LinqAlternative.SingleOrDefault(InnerEnumerable(), p);
        }

        public LinqRoot<V> Cast<V>()
        {
            return LinqAlternative.Cast<V>(InnerEnumerable());
        }

        public bool Contains(T i)
        {
            return LinqAlternative.Contains(InnerEnumerable(), i);
        }

        public V Aggregate<V>(V seed, SigilFunc<V, T, V> p)
        {
            return LinqAlternative.Aggregate(InnerEnumerable(), seed, p);
        }

        public LinqRoot<V> SelectMany<V>(SigilFunc<T, IEnumerable<V>> p)
        {
            return LinqAlternative.SelectMany(InnerEnumerable(), p);
        }

        public LinqRoot<V> SelectMany<V>(SigilFunc<T, LinqRoot<V>> p)
        {
            return LinqAlternative.SelectMany(InnerEnumerable(), x => p(x).AsEnumerable());
        }

        public LinqRoot<T> Distinct()
        {
            return LinqAlternative.Distinct(InnerEnumerable());
        }

        public LinqRoot<T> Distinct(IEqualityComparer<T> c)
        {
            return LinqAlternative.Distinct(InnerEnumerable(), c);
        }

        public LinqRoot<IGrouping<K, T>> GroupBy<K>(SigilFunc<T, K> p)
        {
            return LinqAlternative.GroupBy(InnerEnumerable(), p);
        }

        public LinqRoot<T> OrderBy<V>(SigilFunc<T, V> p)
        {
            return LinqAlternative.OrderBy(InnerEnumerable(), p);
        }

        public LinqRoot<T> OrderByDescending<V>(SigilFunc<T, V> p)
        {
            return LinqAlternative.OrderByDescending(InnerEnumerable(), p);
        }

        public void Each(Action<T> a)
        {
            LinqAlternative.Each(InnerEnumerable(), a);
        }
    }

    internal class LinqEnumerable<T> : LinqRoot<T>
    {
        private IEnumerable<T> Inner;

        private LinqEnumerable(IEnumerable<T> i)
        {
            Inner = i;
        }

        protected override IEnumerable<T> InnerEnumerable()
        {
            return Inner;
        }

        public static LinqEnumerable<T> For(IEnumerable<T> e)
        {
            return new LinqEnumerable<T>(e);
        }

        public static LinqEnumerable<T> For(LinqRoot<T> e)
        {
            return For(e.AsEnumerable());
        }
    }

    internal class LinqDictionary<K, V> : LinqRoot<KeyValuePair<K, V>>
    {
        private Dictionary<K, V> Inner;

        public V this[K key]
        {
            get
            {
                return Inner[key];
            }
            set
            {
                Inner[key] = value;
            }
        }

        public LinqRoot<K> Keys
        {
            get
            {
                return new LinqList<K>(Inner.Keys);
            }
        }

        private LinqDictionary(Dictionary<K, V> d) { Inner = d; }
        
        public LinqDictionary() : this(new Dictionary<K, V>()) { }
        public LinqDictionary(LinqDictionary<K, V> dict)
        {
            var copy = new Dictionary<K, V>(dict.Count());
            foreach (var k in dict.Keys.AsEnumerable())
            {
                copy[k] = dict[k];
            }

            Inner = copy;
        }

        protected override IEnumerable<KeyValuePair<K, V>> InnerEnumerable()
        {
            return Inner;
        }

        public bool Remove(K key)
        {
            return Inner.Remove(key);
        }

        public bool ContainsKey(K key)
        {
            return Inner.ContainsKey(key);
        }

        public bool TryGetValue(K key, out V value)
        {
            return Inner.TryGetValue(key, out value);
        }

        public void Add(K key, V value)
        {
            Inner.Add(key, value);
        }
    }

    internal class LinqList<T> : LinqRoot<T>
    {
        private List<T> Inner;

        public T this[int ix]
        {
            get
            {
                return Inner[ix];
            }
            set
            {
                Inner[ix] = value;
            }
        }

        public new int Count
        {
            get
            {
                return Inner.Count;
            }
        }

        private LinqList(List<T> l)
        {
            Inner = l;
        }

        public LinqList() : this(new List<T>()) { }
        public LinqList(int n) : this(new List<T>(n)) { }
        public LinqList(IEnumerable<T> e) : this(new List<T>(e)) { }
        public LinqList(LinqRoot<T> e) : this(e.AsEnumerable()) { }

        protected override IEnumerable<T> InnerEnumerable()
        {
            return Inner;
        }

        public int IndexOf(T item)
        {
            return Inner.IndexOf(item);
        }

        public void Add(T item)
        {
            Inner.Add(item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            Inner.AddRange(items);
        }

        public void Insert(int ix, T item)
        {
            Inner.Insert(ix, item);
        }

        public void RemoveAt(int ix)
        {
            Inner.RemoveAt(ix);
        }

        public void AddRange(LinqRoot<T> other)
        {
            Inner.AddRange(other.AsEnumerable());
        }

        public bool Remove(T item)
        {
            return Inner.Remove(item);
        }

        public void CopyTo(T[] arr)
        {
            Inner.CopyTo(arr);
        }
    }

    internal class LinqStack<T> : LinqRoot<T>
        where T : class
    {
        public new int Count { get { return Inner.Count; } }

        private Stack<T> Inner;

        private LinqStack(Stack<T> i)
        {
            Inner = i;
        }

        public LinqStack() : this(new Stack<T>()) { }
        public LinqStack(IEnumerable<T> e) : this(new Stack<T>(e)) { }
        public LinqStack(int n) : this(new Stack<T>(n)) { }
        public LinqStack(LinqRoot<T> e) : this(e.AsEnumerable()) { }

        protected override IEnumerable<T> InnerEnumerable()
        {
            return Inner;
        }

        public T Pop()
        {
            return Inner.Pop();
        }

        public void Push(T t)
        {
            Inner.Push(t);
        }

        public T Peek()
        {
            return Inner.Peek();
        }

        public void Clear()
        {
            Inner.Clear();
        }

        private static LinqList<TypeOnStack> _PeekWildcard = new LinqList<TypeOnStack>(new[] { TypeOnStack.Get<WildcardType>() });
        public LinqList<TypeOnStack>[] Peek(bool baseless, int n)
        {
            var stack = this;

            if (stack.Count < n && !baseless) return null;

            var ret = new LinqList<TypeOnStack>[n];

            int i;
            for (i = 0; i < n && i < stack.Count; i++)
            {
                ret[i] = stack.ElementAt(i) as LinqList<TypeOnStack>;
            }

            while (i < n)
            {
                ret[i] = _PeekWildcard;
                i++;
            }

            return ret;
        }
    }

    internal class LinqArray<T> : LinqRoot<T>
    {
        public static implicit operator LinqArray<T>(T[] t)
        {
            return new LinqArray<T>(t);
        }

        private T[] Inner;

        private LinqArray(T[] i)
        {
            Inner = i;
        }

        protected override IEnumerable<T> InnerEnumerable()
        {
            return Inner;
        }
    }

    internal class LinqHashSet<T> : LinqRoot<T>
    {
#if !COREFX
        private class Comparer : System.Collections.IEqualityComparer
        {
            private IEqualityComparer<T> Inner;
            public Comparer(IEqualityComparer<T> i) { Inner = i; }

            public new bool Equals(object x, object y)
            {
                var xAs = (T)x;
                var yAs = (T)y;

                return Inner.Equals(xAs, yAs);
            }

            public int GetHashCode(object obj)
            {
                return Inner.GetHashCode((T)obj);
            }
        }
#endif




#if COREFX
        private System.Collections.Generic.HashSet<T> Inner;
        private LinqHashSet(System.Collections.Generic.HashSet<T> h)
        {
            Inner = h;
        }
        public LinqHashSet() : this(new System.Collections.Generic.HashSet<T>()) { }
        public LinqHashSet(IEqualityComparer<T> c) : this(new System.Collections.Generic.HashSet<T>(c)) { }
        public LinqHashSet(IEnumerable<T> e) : this(new HashSet<T>(e))  { }
#else
        private System.Collections.Hashtable Inner;
        private LinqHashSet(System.Collections.Hashtable h)
        {
            Inner = h;
        }
        public LinqHashSet() : this(new System.Collections.Hashtable()) { }
        public LinqHashSet(IEqualityComparer<T> c) : this(new System.Collections.Hashtable(new Comparer(c))) { }
        public LinqHashSet(IEnumerable<T> e) : this()
        {
            foreach (var x in e)
            {
                Inner.Add(x, "");
            }
        }
#endif
        public new int Count { get { return Inner.Count; } }

#if COREFX
        protected override IEnumerable<T> InnerEnumerable()
        {
            foreach (var x in Inner)
            {
                yield return x;
            }
        }
        public void Add(T item)
        {
            Inner.Add(item);
        }
        public bool Remove(T item)
        {
            return Inner.Remove(item);
        }
#else
        protected override IEnumerable<T> InnerEnumerable()
        {
            foreach (var x in Inner.Keys)
            {
                yield return (T)x;
            }
        }
        public void Add(T item)
        {
            Inner[item] = "";
        }
        public bool Remove(T item)
        {
            if (!Inner.ContainsKey(item)) return false;

            Inner.Remove(item);

            return true;
        }
#endif

        public new bool Contains(T item)
        {
            return Inner.Contains(item);
        }
    }
}
