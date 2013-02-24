using System;
using System.Collections.Generic;
using System.Linq;

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
        public int Count { get { return Names.Count(); } }

        /// <summary>
        /// Returns the names of all the locals in scope
        /// </summary>
        public IEnumerable<string> Names { get { return InnerLookup.Keys.Where(k => !k.StartsWith("__")).ToList(); } }

        private Dictionary<string, Local> InnerLookup;

        internal LocalLookup(Dictionary<string, Local> innerLookup)
        {
            InnerLookup = innerLookup;
        }
    }
}