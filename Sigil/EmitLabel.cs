using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    public class EmitLabel
    {
        public string Name { get; private set; }

        internal Label Label { get; private set; }

        internal object Owner { get; private set; }

        internal EmitLabel(object owner, Label label, string name)
        {
            Owner = owner;
            Name = name;
            Label = label;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
