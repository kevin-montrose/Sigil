using System.Reflection;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Transfers control to another method.
        /// 
        /// The parameters and calling convention of method must match the current one's.
        /// 
        /// The stack must be empty to jump.
        /// 
        /// Like the branching instructions, Jump cannot leave exception blocks.
        /// </summary>
        public Emit Jump(MethodInfo method)
        {
            InnerEmit.Jump(method);
            return this;
        }
    }
}
