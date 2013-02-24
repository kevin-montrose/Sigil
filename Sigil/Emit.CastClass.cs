using Sigil.Impl;
using System;
using System.Linq;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        private void ElideCasts()
        {
            foreach (var ix in ElidableCasts.OrderByDescending(_ => _))
            {
                RemoveInstruction(ix);
            }
        }

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

            var curIndex = IL.Index;
            bool elided = false;

            VerificationCallback before =
                (stack, baseless) =>
                {
                    // Can't reason about stack unless it's completely known
                    if (baseless || elided) return;

                    var onStack = stack.First();

                    if (onStack.All(a => referenceType.IsAssignableFrom(a)))
                    {
                        ElidableCasts.Add(curIndex);
                        elided = true;
                    }
                };

            var newType = TypeOnStack.Get(referenceType);

            var transitions =
                    new[] 
                    {
                        new StackTransition(new [] { typeof(object) }, new [] { referenceType }, before: before)
                    };

            UpdateState(OpCodes.Castclass, referenceType, transitions.Wrap("CastClass"));

            return this;
        }
    }
}
