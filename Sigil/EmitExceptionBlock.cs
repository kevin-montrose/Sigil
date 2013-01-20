using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    public class EmitExceptionBlock
    {
        internal object Owner { get { return Label.Owner; } }

        public EmitLabel Label { get; private set; }

        internal EmitExceptionBlock(EmitLabel label)
        {
            Label = label;
        }
    }
}
