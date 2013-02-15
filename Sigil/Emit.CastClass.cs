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

            var onStack = top[0];

            var newType = TypeOnStack.Get(referenceType);
            bool isTrivial = false;

            isTrivial = newType.IsAssignableFrom(onStack);

            if (isTrivial)
            {
                // already trivially castable; we don't need any IL for this

                if (onStack.IsMarkable)
                {
                    newType = TypeOnStack.Get(referenceType, makeMarkable: true);
                }

                Stack = Stack.Pop().Push(newType);

                onStack.ReplacedWith(newType);
            }
            else
            {
                UpdateState(OpCodes.Castclass, referenceType, newType, pop: 1);
            }
            return this;
        }
    }
}
