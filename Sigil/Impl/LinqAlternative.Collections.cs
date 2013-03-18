using System;
using System.Collections.Generic;

namespace Sigil.Impl
{
    // Want extension methods in .NET 3.0?  Tough
    internal abstract class LinqRoot<T>
    {
        protected abstract IEnumerable<T> InnerEnumerable();

        public IEnumerable<T> Where(Func<T, bool> p)
        {
            return LinqAlternative.Where(InnerEnumerable(), p);
        }

        public int Count()
        {
            return LinqAlternative.Count(InnerEnumerable());
        }

        public int Count(Func<T, bool> p)
        {
            return LinqAlternative.Count(InnerEnumerable(), p);
        }

        public bool Any(Func<T, bool> p)
        {
            return LinqAlternative.Any(InnerEnumerable(), p);
        }

        public IEnumerable<V> Select<V>(Func<T, V> p)
        {
            return LinqAlternative.Select(InnerEnumerable(), p);
        }

        public IEnumerable<V> Select<V>(Func<T, int, V> p)
        {
            return LinqAlternative.Select(InnerEnumerable(), p);
        }

        public List<T> ToList()
        {
            return LinqAlternative.ToList(InnerEnumerable());
        }

        public T[] ToArray()
        {
            return LinqAlternative.ToArray(InnerEnumerable());
        }

        public Dictionary<Key, Value> ToDictionary<Key, Value>(Func<T, Key> k, Func<T, Value> v)
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

        public T FirstOrDefault(Func<T, bool> p)
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

        public IEnumerable<T> Reverse()
        {
            return LinqAlternative.Reverse(InnerEnumerable());
        }

        public bool All(Func<T, bool> p)
        {
            return LinqAlternative.All(InnerEnumerable(), p);
        }

        public IEnumerable<T> Skip(int n)
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

        public T SingleOrDefault(Func<T, bool> p)
        {
            return LinqAlternative.SingleOrDefault(InnerEnumerable(), p);
        }

        public IEnumerable<V> Cast<V>()
        {
            return LinqAlternative.Cast<V>(InnerEnumerable());
        }

        public bool Contains(T i)
        {
            return LinqAlternative.Contains(InnerEnumerable(), i);
        }

        public V Aggregate<V>(V seed, Func<V, T, V> p)
        {
            return LinqAlternative.Aggregate(InnerEnumerable(), seed, p);
        }

        public IEnumerable<V> SelectMany<V>(Func<T, IEnumerable<V>> p)
        {
            return LinqAlternative.SelectMany(InnerEnumerable(), p);
        }

        public IEnumerable<T> Distinct()
        {
            return LinqAlternative.Distinct(InnerEnumerable());
        }

        public IEnumerable<T> Distinct(IEqualityComparer<T> c)
        {
            return LinqAlternative.Distinct(InnerEnumerable(), c);
        }

        public IEnumerable<IGrouping<K, T>> GroupBy<K>(Func<T, K> p)
        {
            return LinqAlternative.GroupBy(InnerEnumerable(), p);
        }

        public IEnumerable<T> OrderBy<V>(Func<T, V> p)
        {
            return LinqAlternative.OrderBy(InnerEnumerable(), p);
        }

        public IEnumerable<T> OrderByDescending<V>(Func<T, V> p)
        {
            return LinqAlternative.OrderByDescending(InnerEnumerable(), p);
        }

        public void Each(Action<T> a)
        {
            LinqAlternative.Each(InnerEnumerable(), a);
        }
    }

    internal class LinqDictionary<K, V> : LinqRoot<KeyValuePair<K, V>>
    {
        private Dictionary<K, V> Inner;

        public static explicit operator Dictionary<K, V>(LinqDictionary<K, V> d)
        {
            return d.Inner;
        }

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
    }

    internal class LinqList<T> : LinqRoot<T>
    {
        private List<T> Inner;

        public static explicit operator List<T>(LinqList<T> l)
        {
            return l.Inner;
        }

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

        public int Count
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

        protected override IEnumerable<T> InnerEnumerable()
        {
            return Inner;
        }

        public void Add(T item)
        {
            Inner.Add(item);
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
}
