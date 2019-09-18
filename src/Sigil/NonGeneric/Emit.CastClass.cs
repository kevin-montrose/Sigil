using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Cast a reference on the stack to the given reference type.</para>
        /// <para>If the cast is not legal, a CastClassException will be thrown at runtime.</para>
        /// </summary>
        public Emit CastClass<ReferenceType>()
            where ReferenceType : class
        {
            InnerEmit.CastClass<ReferenceType>();
            return this;
        }

        /// <summary>
        /// <para>Cast a reference on the stack to the given reference type.</para>
        /// <para>If the cast is not legal, a CastClassException will be thrown at runtime.</para>
        /// </summary>
        public Emit CastClass(Type referenceType)
        {
            InnerEmit.CastClass(referenceType);
            return this;
        }
    }
}
