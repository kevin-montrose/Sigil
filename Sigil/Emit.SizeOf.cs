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
        public void SizeOf<ValueType>()
        {
            SizeOf(typeof(ValueType));
        }

        public void SizeOf(Type valueType)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException("valueType");
            }

            if (!valueType.IsValueType)
            {
                throw new ArgumentException("valueType must be a ValueType");
            }

            UpdateState(OpCodes.Sizeof, valueType, TypeOnStack.Get<int>());
        }
    }
}
