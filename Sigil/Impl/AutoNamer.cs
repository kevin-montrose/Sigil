using System;
using System.Collections.Generic;
using System.Linq;

namespace Sigil.Impl
{
    // Just a dinky little class to automatically generate names for things like methods, labels, and locals
    internal static class AutoNamer
    {
        private static readonly object NullKey = new object();
        private static readonly Dictionary<Tuple<object, string>, int> State = new Dictionary<Tuple<object, string>, int>();

        public static string Next(string root)
        {
            return Next(NullKey, root);
        }

        public static string Next(object on, string root)
        {
            var key = Tuple.Create(on, root);

            lock (State)
            {
                int next;
                if (!State.TryGetValue(key, out next))
                {
                    next = 0;
                    State[key] = next;
                }

                State[key]++;

                return root + next;
            }
        }

        public static void Release(object on)
        {
            lock (State)
            {
                var deadKeys = State.Keys.Where(k => k.Item1 == on).ToList();

                foreach (var key in deadKeys)
                {
                    State.Remove(key);
                }
            }
        }
    }
}
