namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Converts a TypedReference on the stack into a RuntimeTypeHandle for the type contained with it.</para>
        /// <para>__makeref(int) on the stack would become the RuntimeTypeHandle for typeof(int), for example.</para>
        /// </summary>
        public Emit ReferenceAnyType()
        {
            InnerEmit.ReferenceAnyType();
            return this;
        }
    }
}
