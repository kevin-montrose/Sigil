using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Cast a reference on the stack to the given reference type.
        /// 
        /// If the cast is not legal, a CastClassException will be thrown at runtime.
        /// </summary>
        public Emit CastClass<ReferenceType>()
            where ReferenceType : class
        {
            InnerEmit.CastClass<ReferenceType>();
            return this;
        }

        /// <summary>
        /// Cast a reference on the stack to the given reference type.
        /// 
        /// If the cast is not legal, a CastClassException will be thrown at runtime.
        /// </summary>
        public Emit CastClass(Type referenceType)
        {
            InnerEmit.CastClass(referenceType);
            return this;
        }
    }
}
