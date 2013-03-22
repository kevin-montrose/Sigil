using Sigil.Impl;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Throws an ArithmeticException on runtime if the value on the stack is not a finite number.
        /// 
        /// This leaves the value checked on the stack, rather than popping it as might be expected.
        /// </summary>
        public Emit<DelegateType> CheckFinite()
        {
            var transitions =
                new[]
                {
                    new StackTransition(new [] { typeof(float) }, new [] { typeof(float) }),
                    new StackTransition(new [] { typeof(double) }, new [] { typeof(double) })
                };

            // ckfinite leaves the value on the stack, oddly enough
            UpdateState(OpCodes.Ckfinite, Wrap(transitions, "CheckFinite"));

            return this;
        }
    }
}
