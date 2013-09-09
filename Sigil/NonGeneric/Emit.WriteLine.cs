
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Emits IL that calls Console.WriteLine(string) for the given string if no locals are passed.
        /// 
        /// If any locals are passed, line is treated as a format string and local values are used in a call
        /// to Console.WriteLine(string, object[]).
        /// </summary>
        public Emit WriteLine(string line, params Local[] locals)
        {
            InnerEmit.WriteLine(line, locals);
            return this;
        }
    }
}