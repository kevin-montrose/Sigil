
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Loads the value in the given local onto the stack.
        /// 
        /// To create a local, use DeclareLocal().
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
