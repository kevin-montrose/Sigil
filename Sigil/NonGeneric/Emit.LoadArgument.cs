using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Loads the argument at the given index (starting at 0) for the current method onto the stack.
        /// </summary>
        public Emit LoadArgument(ushort index)
        {
            InnerEmit.LoadArgument(index);
            return this;
        }
    }
}
