#if !COREFX // see https://github.com/dotnet/corefx/issues/4543 item 4
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
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
    }
}
#endif
