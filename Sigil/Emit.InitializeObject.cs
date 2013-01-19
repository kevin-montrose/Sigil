using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public void InitializeObject<ValueType>()
        {
            InitializeObject(typeof(ValueType));
        }

        public void InitializeObject(Type valueType)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException("valueType");
            }

            var onStack = Stack.Top();

            if (onStack == null)
            {
                throw new SigilException("InitializeObject expects a value to be on the stack, but it was empty", Stack);
            }

            var obj = onStack[0];

            if (obj != TypeOnStack.Get(valueType))
            {
                throw new SigilException("InitializeObject expected " + valueType + " to be on the stack; found " + obj, Stack);
            }

            UpdateState(OpCodes.Initobj, valueType, pop: 1);
        }
    }
}
