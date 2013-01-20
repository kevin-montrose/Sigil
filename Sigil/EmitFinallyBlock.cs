using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    public class EmitFinallyBlock
    {
        public EmitExceptionBlock ExceptionBlock { get; private set; }

        internal object Owner { get; private set; }

        internal EmitFinallyBlock(object owner, EmitExceptionBlock forTry)
        {
            ExceptionBlock = forTry;
            Owner = owner;
        }
    }
}
