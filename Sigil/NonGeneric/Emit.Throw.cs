
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Pops a value off the stack and throws it as an exception.
        /// 
        /// Throw expects the value to be or extend from a System.Exception.
        /// </summary>
        public Emit Throw()
        {
            InnerEmit.Throw();
            return this;
        }
    }
}
