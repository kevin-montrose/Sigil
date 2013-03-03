using System.Collections.Generic;
using System.Linq;

namespace Sigil.Impl
{
    internal class TransitionWrapper
    {
        public List<StackTransition> Transitions { get; private set; }
        public string MethodName { get; private set; }

        private TransitionWrapper() { }

        public static TransitionWrapper Get(string name, IEnumerable<StackTransition> transitions)
        {
            return new TransitionWrapper { MethodName = name, Transitions = transitions.ToList() };
        }
    }
}
