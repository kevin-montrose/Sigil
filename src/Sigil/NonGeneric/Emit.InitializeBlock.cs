
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Expects a pointer, an initialization value, and a count on the stack.  Pops all three.</para>
        /// <para>Writes the initialization value to count bytes at the passed pointer.</para>
        /// </summary>
        public Emit InitializeBlock(bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.InitializeBlock(isVolatile, unaligned);
            return this;
        }
    }
}
