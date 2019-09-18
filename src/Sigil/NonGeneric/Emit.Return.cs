
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Ends the execution of the current method.</para>
        /// <para>If the current method does not return void, pops a value from the stack and returns it to the calling method.</para>
        /// <para>Return should leave the stack empty.</para>
        /// </summary>
        public Emit Return()
        {
            InnerEmit.Return();

            return this;
        }
    }
}
