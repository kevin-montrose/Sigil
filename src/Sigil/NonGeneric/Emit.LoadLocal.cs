
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Loads the value in the given local onto the stack.</para>
        /// <para>To create a local, use DeclareLocal().</para>
        /// </summary>
        public Emit LoadLocal(Local local)
        {
            InnerEmit.LoadLocal(local);
            return this;
        }

        /// <summary>
        /// Loads the value in the local with the given name onto the stack.
        /// </summary>
        public Emit LoadLocal(string name)
        {
            InnerEmit.LoadLocal(name);
            return this;
        } 
    }
}
