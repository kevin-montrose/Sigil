using System.Collections.Generic;

namespace Sigil.Impl
{
    internal class TransitionWrapper
    {
        public LinqList<StackTransition> Transitions { get; private set; }
        public string MethodName { get; private set; }

        private TransitionWrapper() { }

        public static TransitionWrapper Get(string name, IEnumerable<StackTransition> transitions)
        {
            return new TransitionWrapper { MethodName = name, Transitions = new LinqList<StackTransition>(transitions) };
        }
    }
}
