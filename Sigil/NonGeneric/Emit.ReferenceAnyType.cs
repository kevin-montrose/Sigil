
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
#if !COREFXTODO
        /// <summary>
        /// Converts a TypedReference on the stack into a RuntimeTypeHandle for the type contained with it.
        /// 
        /// __makeref(int) on the stack would become the RuntimeTypeHandle for typeof(int), for example.
        /// </summary>
        public Emit ReferenceAnyType()
        {
            InnerEmit.ReferenceAnyType();
            return this;
        }
#endif
    }
}
