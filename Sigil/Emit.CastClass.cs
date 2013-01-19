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
        public void CastClass<ReferenceType>()
        {
            CastClass(typeof(ReferenceType));
        }

        public void CastClass(Type referenceType)
        {
            if (referenceType == null)
            {
                throw new ArgumentNullException("referenceType");
            }

            if (referenceType.IsValueType)
            {
                throw new SigilException("Can only cast to ReferenceTypes, found " + referenceType, Stack);
            }

            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilException("CastClass expects a value on the stack, but it was empty", Stack);
            }

            UpdateState(OpCodes.Castclass, TypeOnStack.Get(referenceType), pop: 1);
        }
    }
}
