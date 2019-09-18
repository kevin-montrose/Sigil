
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Loads the argument at the given index (starting at 0) for the current method onto the stack.
        /// </summary>
        public Emit LoadArgument(ushort index)
        {
            InnerEmit.LoadArgument(index);
            return this;
        }
    }
}
