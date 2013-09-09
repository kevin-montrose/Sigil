
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Emits an instruction that does nothing.
        /// </summary>
        public Emit Nop()
        {
            InnerEmit.Nop();
            return this;
        }
    }
}
