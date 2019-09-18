
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Pops a value off the stack and throws it as an exception.</para>
        /// <para>Throw expects the value to be or extend from a System.Exception.</para>
        /// </summary>
        public Emit Throw()
        {
            InnerEmit.Throw();
            return this;
        }
    }
}
