using System.Reflection;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Transfers control to another method.</para>
        /// <para>The parameters and calling convention of method must match the current one's.</para>
        /// <para>The stack must be empty to jump.</para>
        /// <para>Like the branching instructions, Jump cannot leave exception blocks.</para>
        /// </summary>
        public Emit Jump(MethodInfo method)
        {
            InnerEmit.Jump(method);
            return this;
        }
    }
}
