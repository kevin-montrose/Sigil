using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    /// <summary>
    /// Represents a Label in a CIL stream, and thus a Leave and Branch target.
    /// 
    /// To create a Label call DefineLabel().
    /// 
    /// Before creating a delegate, all Labels must be marked.  To mark a label, call MarkLabel().
    /// </summary>
    public class Label
    {
        /// <summary>
        /// The name of this Label.
        /// 
        /// If one is omitted during creation a random one is created instead.
        /// 
        /// Names are purely for debugging aid, and will not appear in the generated delegate.
        /// </summary>
        public string Name { get; private set; }

        internal BufferedILGenerator.DefineLabelDelegate LabelDel { get; private set; }

        internal object Owner { get; private set; }

        internal Label(object owner, BufferedILGenerator.DefineLabelDelegate label, string name)
        {
            Owner = owner;
            Name = name;
            LabelDel = label;
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
