using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public void CheckFinite()
        {
            var onStack = Stack.Top();

            if (onStack == null)
            {
                throw new SigilException("CheckFinite expects a value to be on the stack, but it was empty", Stack);
            }

            var val = onStack[0];

            if (val != TypeOnStack.Get<float>() && val != TypeOnStack.Get<double>())
            {
                throw new SigilException("CheckFinite expects a floating point value, found " + val, Stack);
            }

            // ckfinite leaves the value on the stack, oddly enough
            UpdateState(OpCodes.Ckfinite);
        }
    }
}
