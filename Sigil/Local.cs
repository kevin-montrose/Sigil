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
    /// Represents a variable local to the delegate being created.
    /// 
    /// To create a Local, call DeclareLocal().
    /// </summary>
    public class Local
    {
        /// <summary>
        /// The name of this local.
        /// 
        /// If one is omitted during creation a random one is created instead.
        /// 
        /// Names are purely for debugging aid, and will not appear in the generated delegate.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The type stored in this local.
        /// </summary>
        public Type LocalType { get; private set; }

        internal TypeOnStack StackType { get; private set; }
        internal int Index { get; private set; }

        internal BufferedILGenerator.DeclareLocallDelegate LocalDel { get; private set; }
        internal object Owner { get; private set; }

        internal Local(object owner, int index, Type localType, BufferedILGenerator.DeclareLocallDelegate local, string name)
        {
            Owner = owner;
            LocalDel = local;
            Name = name;

            Index = index;
            LocalType = localType;
            StackType = TypeOnStack.Get(localType);
        }

        /// <summary>
        /// Returns the type and name of this Local, in string form.
        /// </summary>
        public override string ToString()
        {
            return LocalType.FullName + " " + Name;
        }
    }
}
