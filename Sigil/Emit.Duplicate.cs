using Sigil.Impl;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pushes a copy of the current top value on the stack.
        /// </summary>
        public Emit<DelegateType> Duplicate()
        {
            UpdateState(OpCodes.Dup, (new [] { new StackTransition(isDuplicate: true) }).Wrap("Duplicate"));

            return this;
        }
    }
}
