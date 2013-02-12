using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Removes the top value on the stack.
        /// </summary>
        public Emit<DelegateType> Pop()
        {
            var onStack = Stack.Top();

            if (onStack == null)
            {
                FailStackUnderflow(1);
            }

            UpdateState(OpCodes.Pop, pop: 1);

            return this;
        }
    }
}
