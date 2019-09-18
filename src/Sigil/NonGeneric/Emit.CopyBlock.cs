
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Expects a destination pointer, a source pointer, and a length on the stack.  Pops all three values.</para>
        /// <para>Copies length bytes from destination to the source.</para>
        /// </summary>
        public Emit CopyBlock(bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.CopyBlock(isVolatile, unaligned);
            return this;
        }
    }
}
