using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    public class EmitLocal
    {
        public string Name { get; private set; }
        public Type LocalType { get { return Builder.LocalType; } }

        internal TypeOnStack StackType { get; private set; }
        internal int Index { get { return Builder.LocalIndex; } }

        internal LocalBuilder Builder { get; private set; }
        internal object Owner { get; private set; }

        internal EmitLocal(object owner, LocalBuilder local, string name)
        {
            Owner = owner;
            Builder = local;
            Name = name;

            StackType = TypeOnStack.Get(Builder.LocalType);
        }

        public override string ToString()
        {
            return LocalType.FullName + " " + Name;
        }
    }
}
