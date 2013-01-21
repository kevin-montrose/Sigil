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

        public Label Label { get; private set; }

        internal EmitExceptionBlock(Label label)
        {
            Label = label;
        }
    }
}
