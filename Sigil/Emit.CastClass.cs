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
        /// <summary>
        /// Cast a reference on the stack to the given reference type.
        /// 
        /// If the cast is not legal, a CastClassException will be thrown at runtime.
        /// </summary>
        public Emit<DelegateType> CastClass<ReferenceType>()
            where ReferenceType : class
        {
            return CastClass(typeof(ReferenceType));
        }

        /// <summary>
        /// Cast a reference on the stack to the given reference type.
        /// 
        /// If the cast is not legal, a CastClassException will be thrown at runtime.
        /// </summary>
        public Emit<DelegateType> CastClass(Type referenceType)
        {
            if (referenceType == null)
            {
                throw new ArgumentNullException("referenceType");
            }

            if (referenceType.IsValueType)
            {
                throw new ArgumentException("Can only cast to ReferenceTypes, found " + referenceType);
            }

            var top = Stack.Top();

            if (top == null)
            {
                throw new SigilVerificationException("CastClass expects a value on the stack, but it was empty", IL, Stack);
            }

            UpdateState(OpCodes.Castclass, referenceType, TypeOnStack.Get(referenceType), pop: 1);

            return this;
        }
    }
}
