using System.Reflection;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Loads the address of the given field onto the stack.</para>
        /// <para>If the field is an instance field, a `this` reference is expected on the stack and will be popped.</para>
        /// </summary>
        public Emit LoadFieldAddress(FieldInfo field)
        {
            InnerEmit.LoadFieldAddress(field);
            return this;
        }
    }
}
