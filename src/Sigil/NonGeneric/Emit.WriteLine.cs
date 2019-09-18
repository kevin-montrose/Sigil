
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Emits IL that calls Console.WriteLine(string) for the given string if no locals are passed.</para>
        /// <para>
        /// If any locals are passed, line is treated as a format string and local values are used in a call
        /// to Console.WriteLine(string, object[]).
        /// </para>
        /// </summary>
        public Emit WriteLine(string line, params Local[] locals)
        {
            InnerEmit.WriteLine(line, locals);
            return this;
        }
    }
}