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
        /// <summary>
        /// Expects an instance of the type to be initialized on the stack.
        /// 
        /// Initializes all the fields on a value type to null or an appropriate zero value.
        /// </summary>
        public void InitializeObject<ValueType>()
        {
            InitializeObject(typeof(ValueType));
        }

        /// <summary>
        /// Expects an instance of the type to be initialized on the stack.
        /// 
        /// Initializes all the fields on a value type to null or an appropriate zero value.
        /// </summary>
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

            if (obj != TypeOnStack.Get<NativeInt>() && obj != TypeOnStack.Get(valueType.MakePointerType()) && obj != TypeOnStack.Get(valueType.MakeByRefType()))
            {
                throw new SigilException("InitializeObject expected a reference or pointer to a " + valueType + ", or a native int, to be on the stack; found " + obj, Stack);
            }

            UpdateState(OpCodes.Initobj, valueType, pop: 1);
        }
    }
}
