
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// From within a catch block, rethrows the exception that caused the catch block to be entered.
        /// </summary>
        public Emit ReThrow()
        {
            InnerEmit.ReThrow();
            return this;
        }
    }
}
