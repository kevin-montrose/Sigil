using System.Collections.Generic;
using Sigil.Impl;

namespace Sigil
{
    /// <summary>
    /// Provides a way to lookup labels declared with an emit.
    /// </summary>
    public class LabelLookup
    {
        /// <summary>
        /// Returns the label with the given name.
        /// 
        /// Throws KeyNotFoundException if no label by that name is found".
        /// </summary>
        public Label this[string name]
        {
            get
            {
                if (!InnerLookup.ContainsKey(name))
                {
                    throw new KeyNotFoundException("No label with name '" + name + "' found");
                }

                return InnerLookup[name];
            }
        }

        /// <summary>
        /// Returns the number of labels declared
        /// </summary>
        public int Count { get { return Names.Count(); } }

        /// <summary>
        /// Returns the names of all the declared labels
        /// </summary>
        public IEnumerable<string> Names { get { return InnerLookup.Keys.Where(k => !k.StartsWith("__")).ToList(); } }

        private LinqDictionary<string, Label> InnerLookup;

        internal LabelLookup(LinqDictionary<string, Label> innerLookup)
        {
            InnerLookup = innerLookup;
        }
    }
}
