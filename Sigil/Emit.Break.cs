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
        /// Emits a break instruction for use with a debugger.
        /// </summary>
        public Emit<DelegateType> Break()
        {
            UpdateState(OpCodes.Break);

            return this;
        }
    }
}
