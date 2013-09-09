using System.Reflection;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Loads a field onto the stack.
        /// 
        /// Instance fields expect a reference on the stack, which is popped.
        /// </summary>
        public Emit LoadField(FieldInfo field, bool? isVolatile = null, int? unaligned = null)
        {
            InnerEmit.LoadField(field, isVolatile, unaligned);
            return this;
        }
    }
}
