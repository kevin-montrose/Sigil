using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public EmitLabel CreateLabel()
        {
            return CreateLabel("_" + Guid.NewGuid().ToString().Replace("-", ""));
        }

        public EmitLabel CreateLabel(string name)
        {
            var label = IL.DefineLabel();

            var ret = new EmitLabel(this, label, name);

            UnusedLabels.Add(ret);
            UnmarkedLabels.Add(ret);

            return ret;
        }

        public void MarkLabel(EmitLabel label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (label.Owner != this)
            {
                throw new ArgumentException("label is not owner by this Emit, and thus cannot be used");
            }

            if (!UnmarkedLabels.Contains(label))
            {
                throw new SigilException("label [" + label.Name + "] has already been marked, and cannot be marked a second time", Stack);
            }

            UnmarkedLabels.Remove(label);

            IL.MarkLabel(label.Label);
        }
    }
}
