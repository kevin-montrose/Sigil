
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Pops a size from the stack, allocates size bytes on the local dynamic memory pool, and pushes a pointer to the allocated block.</para>
        /// <para>LocalAllocate can only be called if the stack is empty aside from the size value.</para>
        /// <para>Memory allocated with LocalAllocate is released when the current method ends execution.</para>
        /// </summary>
        public Emit LocalAllocate()
        {
            InnerEmit.LocalAllocate();
            return this;
        }
    }
}
