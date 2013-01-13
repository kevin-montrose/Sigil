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
        /// Called to confirm that the IL emit'd to date can be turned into a delegate without error.
        /// 
        /// Checks that the stack is empty, that all paths returns, that all labels are marked, etc. etc.
        /// </summary>
        private void Validate()
        {
            if (!Stack.IsRoot)
            {
                throw new SigilException("Delegates must leave their stack empty when they end", Stack);
            }

            var lastInstr = InstructionStream.LastOrDefault();

            if (lastInstr == null || lastInstr.Item1 != OpCodes.Ret)
            {
                throw new SigilException("Delegate must end with Return", Stack);
            }
        }
    }
}
