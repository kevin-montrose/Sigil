using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    public class Label
    {
        public string Name { get; private set; }

        internal BufferedILGenerator.DefineLabelDelegate LabelDel { get; private set; }

        internal object Owner { get; private set; }

        internal Label(object owner, BufferedILGenerator.DefineLabelDelegate label, string name)
        {
            Owner = owner;
            Name = name;
            LabelDel = label;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
