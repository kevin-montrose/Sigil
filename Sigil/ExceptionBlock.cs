using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    public class ExceptionBlock
    {
        internal object Owner { get { return Label.Owner; } }

        public Label Label { get; private set; }

        internal ExceptionBlock(Label label)
        {
            Label = label;
        }
    }
}
