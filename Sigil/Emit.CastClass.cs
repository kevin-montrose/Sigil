using Sigil.Impl;
using System;
using System.Reflection.Emit;

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
                FailStackUnderflow(1);
            }

            var newType = TypeOnStack.Get(referenceType);
            bool isTrivial = false;

            try
            {
                isTrivial = newType.IsAssignableFrom(top[0]);
            }
            catch { /* not always possible to detect, due to builders etc  */ }
            if (isTrivial)
            {
                // already trivially castable; we don't need any IL for this
                Stack = Stack.Pop().Push(newType);
            }
            else
            {
                UpdateState(OpCodes.Castclass, referenceType, newType, pop: 1);
            }
            return this;
        }
    }
}
