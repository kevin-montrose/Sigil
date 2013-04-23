using Sigil.Impl;
using System.Collections.Generic;

namespace Sigil
{
    /// <summary>
    /// Provides a way to lookup locals in scope by name.
    /// </summary>
    public class LocalLookup
    {
        /// <summary>
        /// Returns the local with the given name.
        /// 
        /// Throws KeyNotFoundException if no local by that name is found".
        /// </summary>
        public Local this[string name]
        {
            get
            {
                if (!InnerLookup.ContainsKey(name))
                {
                    throw new KeyNotFoundException("No local with name '" + name + "' found");
                }

                return InnerLookup[name];
            }
        }

        /// <summary>
        /// Returns the number of locals in scope
        /// </summary>
        public int Count { get { return _Names.Count(); } }

        private LinqRoot<string> _Names { get { return InnerLookup.Keys.Where(k => !k.StartsWith("__")).ToList(); } }

        /// <summary>
        /// Returns the names of all the locals in scope
        /// </summary>
        public IEnumerable<string> Names { get { return _Names.AsEnumerable(); } }

        private LinqDictionary<string, Local> InnerLookup;

        internal LocalLookup(LinqDictionary<string, Local> innerLookup)
        {
            InnerLookup = innerLookup;
        }
    }
}