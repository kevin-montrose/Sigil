
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Expects a pointer, an initialization value, and a count on the stack.  Pops all three.
        /// 
        /// Writes the initialization value to count bytes at the passed pointer.
        /// </summary>
        public Emit InitializeBlock(bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.InitializeBlock(isVolatile, unaligned);
            return this;
        }
    }
}
