
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Pushes a pointer to the current argument list onto the stack.</para>
        /// <para>This instruction can only be used in VarArgs methods.</para>
        /// </summary>
        public Emit ArgumentList()
        {
            InnerEmit.ArgumentList();
            return this;
        }
    }
}
