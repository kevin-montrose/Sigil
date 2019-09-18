using System.Collections.Generic;

namespace Sigil.Impl
{
    // Just a dinky little class to automatically generate names for things like methods, labels, and locals
    internal static class AutoNamer
    {
        private static readonly object NullKey = new object();
        private static readonly Dictionary<SigilTuple<object, string>, int> State = new Dictionary<SigilTuple<object, string>, int>();

        public static string Next(string root)
        {
            return Next(NullKey, root);
        }

        public static string Next(object on, string root, params IEnumerable<string>[] inUse)
        {
            var key = SigilTuple.Create(on, root);

            lock (State)
            {
                int next;
                if (!State.TryGetValue(key, out next))
                {
                    next = 0;
                    State[key] = next;
                }

                State[key]++;

                var ret = root + next;

                if (LinqAlternative.Any(inUse, a => LinqAlternative.Any(a, x => x == ret)))
                {
                    return Next(on, root, inUse);
                }

                return ret;
            }
        }

        public static void Release(object on)
        {
            lock (State)
            {
                var deadKeys = LinqAlternative.Where(State.Keys,k => k.Item1 == on).ToList();

                foreach (var key in deadKeys.AsEnumerable())
                {
                    State.Remove(key);
                }
            }
        }
    }
}
