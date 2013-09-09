
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Emits a break instruction for use with a debugger.
        /// </summary>
        public Emit Break()
        {
            InnerEmit.Break();
            return this;
        }
    }
}
