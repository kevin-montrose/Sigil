
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Pushes a pointer to the given local onto the stack.</para>
        /// <para>To create a local, use DeclareLocal.</para>
        /// </summary>
        public Emit LoadLocalAddress(Local local)
        {
            InnerEmit.LoadLocalAddress(local);
            return this;
        }

        /// <summary>
        /// Pushes a pointer to the local with the given name onto the stack.
        /// </summary>
        public Emit LoadLocalAddress(string name)
        {
            InnerEmit.LoadLocalAddress(name);
            return this;
        }
    }
}
