using System;
using System.Collections.Generic;
using System.Linq;

namespace Sigil.Impl
{
    // Represents a type that *could be* anything
    internal class WildcardType { }

    public class StackTransition
    {
        // on the stack, first item is on the top of the stack
        internal IEnumerable<TypeOnStack> PoppedFromStack { get; private set; }

        // pushed onto the stack, first item is first pushed (ends up lowest on the stack)
        internal IEnumerable<TypeOnStack> PushedToStack { get; private set; }

        public StackTransition(IEnumerable<Type> popped, IEnumerable<Type> pushed)
        {


            PoppedFromStack = popped.Select(s => TypeOnStack.Get(s)).ToList().AsReadOnly();
            PushedToStack = pushed.Select(s => TypeOnStack.Get(s)).ToList().AsReadOnly();
        }
    }

    public class VerifiableTracker
    {
        // When the stack is "unbased" or "baseless", underflowing it results in wildcards
        //   eventually they'll be fixed up to actual types
        private bool Baseless;
        private List<IEnumerable<StackTransition>> Transitions = new List<IEnumerable<StackTransition>>();
        private int Index;

        public VerifiableTracker(bool baseless = false) { Baseless = baseless; }

        public bool Transition(IEnumerable<StackTransition> legalTransitions)
        {
            Transitions.Add(legalTransitions);
            var ret = CollapseAndVerify();

            // revert!
            if(!ret) Transitions.RemoveAt(Transitions.Count - 1);

            return ret;
        }

        public bool Incoming(VerifiableTracker other)
        {
            var old = Transitions;

            Transitions = new List<IEnumerable<StackTransition>>();
            Transitions.AddRange(other.Transitions);
            Transitions.AddRange(old);

            var ret = CollapseAndVerify();

            // revert!
            if (!ret) Transitions = old;

            return ret;
        }

        public bool CollapseAndVerify()
        {
            var runningStack = new Stack<IEnumerable<TypeOnStack>>();

            for (var i = 0; i < Transitions.Count; i++)
            {
                var ops = Transitions[i];

                var legal =
                    ops.Where(
                        w =>
                        {
                            var onStack = runningStack.Peek(Baseless, w.PoppedFromStack.Count());

                            if (onStack == null) return false;

                            for (var j = 0; j < w.PoppedFromStack.Count(); j++)
                            {
                                var shouldBe = w.PoppedFromStack.ElementAt(j);
                                var actuallyIs = onStack[j];

                                if (!actuallyIs.Any(a => shouldBe.IsAssignableFrom(a))) return false;
                            }

                            return true;
                        }
                    ).ToList();

                if (legal.Count == 0) return false;

                if (legal.GroupBy(g => new { a = g.PoppedFromStack.Count(), b = g.PushedToStack.Count() }).Count() > 1)
                {
                    throw new Exception("Shouldn't be possible; legal transitions should have same push/pop #s");
                }

                // No reason to do all this work again
                Transitions[i] = legal;

                var toPop = legal.First().PoppedFromStack.Count();

                if (toPop > runningStack.Count && !Baseless) return false;

                for (var j = 0; j < toPop && runningStack.Count > 0; j++)
                {
                    runningStack.Pop();
                }

                var toPush = new List<TypeOnStack>();

                foreach (var op in legal)
                {
                    toPush.AddRange(op.PushedToStack);
                }

                if(toPush.Count > 0)
                {
                    runningStack.Push(toPush.Distinct().ToList());
                }
            }

            return true;
        }
    }
}
