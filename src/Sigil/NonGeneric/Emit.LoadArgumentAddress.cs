
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Loads a pointer to the argument at index (starting at zero) onto the stack.
        /// </summary>
        public Emit LoadArgumentAddress(ushort index)
        {
            InnerEmit.LoadArgumentAddress(index);
            return this;
        }
    }
}
