
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Pushes a copy of the current top value on the stack.
        /// </summary>
        public Emit Duplicate()
        {
            InnerEmit.Duplicate();
            return this;
        }
    }
}
