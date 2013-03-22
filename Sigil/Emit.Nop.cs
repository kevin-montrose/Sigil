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
            UpdateState(OpCodes.Nop, Wrap(StackTransition.None(), "Nop"));

            return this;
        }
    }
}
