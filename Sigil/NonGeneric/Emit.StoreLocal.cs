
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Pops a value off the stack and stores it into the given local.
        /// 
        /// To create a local, use DeclareLocal().
        /// </summary>
        public Emit StoreLocal(Local local)
        {
            InnerEmit.StoreLocal(local);
            return this;
        }

        /// <summary>
        /// Pops a value off the stack and stores it in the local with the given name.
        /// </summary>
        public Emit StoreLocal(string name)
        {
            InnerEmit.StoreLocal(name);
            return this;
        } 
    }
}
