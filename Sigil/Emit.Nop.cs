using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Emits an instruction that does nothing.
        /// </summary>
        public Emit<DelegateType> Nop()
        {
            UpdateState(OpCodes.Nop);

            return this;
        }
    }
}
