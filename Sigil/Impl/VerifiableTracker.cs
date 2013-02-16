using System;
using System.Collections.Generic;
using System.Linq;

namespace Sigil.Impl
{
    // Represents a type that *could be* anything
    internal class WildcardType { }

    internal class StackTransition
    {
        // on the stack, first item is on the top of the stack
        public IEnumerable<TypeOnStack> PoppedFromStack { get; private set; }

        // pushed onto the stack, first item is first pushed (ends up lowest on the stack)
        public IEnumerable<TypeOnStack> PushedToStack { get; private set; }

        public StackTransition(IEnumerable<TypeOnStack> popped, IEnumerable<TypeOnStack> pushed)
        {
            PoppedFromStack = popped.ToList().AsReadOnly();
            PushedToStack = pushed.ToList().AsReadOnly();
        }
    }

    internal class VerifiableTracker
    {
        // When the stack is "unbased" or "baseless", underflowing it results in wildcards
        //   eventually they'll be fixed up to actual types
        private bool BaselessStack;
        private Stack<IEnumerable<TypeOnStack>> Stack = new Stack<IEnumerable<TypeOnStack>>();
        private int Index;

        public VerifiableTracker() { }

        private IEnumerable<TypeOnStack>[] Peek(int count)
        {
            if (Stack.Count < count && !BaselessStack) return null;

            var ret = new IEnumerable<TypeOnStack>[count];

            int i;
            for (i = 0; i < count && i < Stack.Count; i++)
            {
                ret[i] = Stack.ElementAt(i);
            }

            for(;i < ret.Length; i++)
            {
                ret[i] = new[] { TypeOnStack.Get<WildcardType>() };
            }

            return ret;
        }

        private bool HasAssignableType(IEnumerable<TypeOnStack> legal, TypeOnStack found)
        {
            return legal.Any(t => t.IsAssignableFrom(found));
        }

        public bool Transition(out VerifiableTracker next, params StackTransition[] legalTransitions)
        {
            var applicable =
                legalTransitions
                    .Where(
                        t =>
                        {
                            var onStack = Peek(t.PoppedFromStack.Count());

                            if(onStack == null) return false;

                            for (var i = 0; i < onStack.Length; i++)
                            {
                                var shouldBe = t.PoppedFromStack.ElementAt(i);
                                var actuallyIs = onStack[i];

                                if (!HasAssignableType(actuallyIs, shouldBe))
                                {
                                    return false;
                                }
                            }

                            return true;
                        }
                    ).ToList();

            if (applicable.Count == 0)
            {
                next = null;
                return false;
            }

            if (applicable.GroupBy(g => new { popped = g.PoppedFromStack.Count(), pushed = g.PushedToStack.Count() }).Count() > 1)
            {
                throw new Exception("legal transitions should all modify the stack in the same way");
            }

            var pop = applicable.First().PoppedFromStack.Count();
            
            var copy = new Stack<IEnumerable<TypeOnStack>>(Stack);

            // remember, baseless-ness means `copy.Count` is not necessarily less than `pop`
            while (pop > 0 && copy.Count > 0)
            {
                copy.Pop();
                pop--;
            }

            var push = applicable.First().PushedToStack.Count();

            for (var i = 0; i < push; i++)
            {
                var toPush = new List<TypeOnStack>();
                foreach (var possible in applicable)
                {
                    var atPos = possible.PushedToStack.ElementAt(i); 
                    toPush.Add(atPos);
                }

                copy.Push(toPush.Distinct().ToList());
            }

            next = new VerifiableTracker
            {
                BaselessStack = BaselessStack,
                Stack = copy,
                Index = Index + 1
            };

            return true;
        }
    }
}
