
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Pops a value off the stack and stores it into the argument to the current method identified by index.
        /// </summary>
        public Emit StoreArgument(ushort index)
        {
            InnerEmit.StoreArgument(index);
            return this;
        }
    }
}
