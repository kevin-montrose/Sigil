using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Expects an instance of the type to be initialized on the stack.</para>
        /// <para>Initializes all the fields on a value type to null or an appropriate zero value.</para>
        /// </summary>
        public Emit InitializeObject<ValueType>()
        {
            InnerEmit.InitializeObject<ValueType>();
            return this;
        }

        /// <summary>
        /// <para>Expects an instance of the type to be initialized on the stack.</para>
        /// <para>Initializes all the fields on a value type to null or an appropriate zero value.</para>
        /// </summary>
        public Emit InitializeObject(Type valueType)
        {
            InnerEmit.InitializeObject(valueType);
            return this;
        }
    }
}
