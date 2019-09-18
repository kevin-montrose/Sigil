
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Throws an ArithmeticException on runtime if the value on the stack is not a finite number.</para>
        /// <para>This leaves the value checked on the stack, rather than popping it as might be expected.</para>
        /// </summary>
        public Emit CheckFinite()
        {
            InnerEmit.CheckFinite();
            return this;
        }
    }
}
