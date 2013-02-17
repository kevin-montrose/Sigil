using Sigil.Impl;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Emits an instruction that does nothing.
        /// </summary>
        public Emit<DelegateType> Nop()
        {
            UpdateState(OpCodes.Nop, StackTransition.None().Wrap("Nop"));

            return this;
        }
    }
}
