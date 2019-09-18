using Sigil.Impl;
using System;

namespace Sigil
{
    /// <summary>
    /// <para>Represents a Label in a CIL stream, and thus a Leave and Branch target.</para>
    /// <para>To create a Label call DefineLabel().</para>
    /// <para>Before creating a delegate, all Labels must be marked.  To mark a label, call MarkLabel().</para>
    /// </summary>
    public class Label : IOwned
    {
        /// <summary>
        /// <para>The name of this Label.</para>
        /// <para>If one is omitted during creation a random one is created instead.</para>
        /// <para>Names are purely for debugging aid, and will not appear in the generated delegate.</para>
        /// </summary>
        public string Name { get; private set; }

        internal DefineLabelDelegate LabelDel { get; private set; }

        private object _Owner;
        object IOwned.Owner { get { return _Owner; } }

        internal Label(object owner, DefineLabelDelegate label, string name)
        {
            _Owner = owner;
            Name = name;
            LabelDel = label;
        }

        internal void SetOwner(object owner)
        {
            if (_Owner != null && owner != null) throw new Exception("Cannot set ownership of an owner Label");

            _Owner = owner;
        }

        /// <summary>
        /// Equivalent to Name.
        /// </summary>
        public override string ToString()
        {
            return Name;
        }
    }
}
