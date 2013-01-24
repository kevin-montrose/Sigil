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
        public void ReThrow()
        {
            if(!CatchBlocks.Any(c => c.Value.Item2 == -1))
            {
                throw new SigilException("ReThrow is only legal in a catch block", Stack);
            }

            UpdateState(OpCodes.Rethrow);
        }
    }
}
