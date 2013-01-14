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
        public Type LocalType { get; private set; }

        internal TypeOnStack StackType { get; private set; }
        internal int Index { get; private set; }

        //internal LocalBuilder Builder { get; private set; }
        internal BufferedILGenerator.DeclareLocallDelegate Local { get; private set; }
        internal object Owner { get; private set; }

        internal EmitLocal(object owner, int index, Type localType, BufferedILGenerator.DeclareLocallDelegate local, string name)
        {
            Owner = owner;
            Local = local;
            Name = name;

            Index = index;
            LocalType = localType;
            StackType = TypeOnStack.Get(localType);
        }

        public override string ToString()
        {
            return LocalType.FullName + " " + Name;
        }
    }
}
