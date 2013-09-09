
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Ends the execution of the current method.
        /// 
        /// If the current method does not return void, pops a value from the stack and returns it to the calling method.
        /// 
        /// Return should leave the stack empty.
        /// </summary>
        public Emit Return()
        {
            InnerEmit.Return();

            return this;
        }
    }
}
