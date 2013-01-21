using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    public class FinallyBlock
    {
        public ExceptionBlock ExceptionBlock { get; private set; }

        internal object Owner { get; private set; }

        internal FinallyBlock(object owner, ExceptionBlock forTry)
        {
            ExceptionBlock = forTry;
            Owner = owner;
        }
    }
}
