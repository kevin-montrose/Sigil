using System.Reflection;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Loads a field onto the stack.</para>
        /// <para>Instance fields expect a reference on the stack, which is popped.</para>
        /// </summary>
        public Emit LoadField(FieldInfo field, bool? isVolatile = null, int? unaligned = null)
        {
            InnerEmit.LoadField(field, isVolatile, unaligned);
            return this;
        }
    }
}
