
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Removes the top value on the stack.
        /// </summary>
        public Emit Pop()
        {
            InnerEmit.Pop();
            return this;
        }
    }
}
