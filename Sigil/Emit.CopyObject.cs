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
        /// Takes a destination pointer, a source pointer as arguments.  Pops both off the stack.
        /// 
        /// Copies the given value type from the source to the destination.
        /// </summary>
        public Emit<DelegateType> CopyObject<ValueType>()
            where ValueType : struct
        {
            return CopyObject(typeof(ValueType));
        }

        /// <summary>
        /// Takes a destination pointer, a source pointer as arguments.  Pops both off the stack.
        /// 
        /// Copies the given value type from the source to the destination.
        /// </summary>
        public Emit<DelegateType> CopyObject(Type valueType)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException("valueType");
            }

            if (!valueType.IsValueType)
            {
                throw new ArgumentException("CopyObject expects a ValueType; found " + valueType);
            }

            var onStack = Stack.Top(2);

            if (onStack == null)
            {
                FailStackUnderflow(2);
            }

            var dest = onStack[1];
            var source = onStack[0];

            if (!source.IsPointer && !source.IsReference && source != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilVerificationException("CopyObject expects the source value to be a pointer, reference, or native int; found " + source, IL, Stack);
            }

            if (!dest.IsPointer && !dest.IsReference && dest != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilVerificationException("CopyObject expects the destination value to be a pointer, reference, or native int; found " + dest, IL, Stack);
            }

            if (source != dest)
            {
                throw new SigilVerificationException("CopyObject expects the source and destination types to match; found " + source + " and " + dest, IL, Stack);
            }

            UpdateState(OpCodes.Cpobj, valueType, pop: 2);

            return this;
        }
    }
}
