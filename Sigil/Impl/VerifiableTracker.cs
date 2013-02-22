using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Sigil.Impl
{
    // Represents a type that *could be* anything
    internal class WildcardType { }

    // Represents *any* pointer
    internal class AnyPointerType { }

    // Something that's *only* assignable from object
    internal class OnlyObjectType { }

    // Something that means "pop the entire damn stack"
    internal class PopAllType { }

    public class StackTransition
    {
        internal int PoppedCount { get { return PoppedFromStack.Count(); } }

        // on the stack, first item is on the top of the stack
        internal IEnumerable<TypeOnStack> PoppedFromStack { get; private set; }

        // pushed onto the stack, first item is first pushed (ends up lowest on the stack)
        internal IEnumerable<TypeOnStack> PushedToStack { get; private set; }

        internal int? StackSizeMustBe { get; private set; }

        internal bool IsDuplicate { get; private set; }

        public StackTransition(IEnumerable<Type> popped, IEnumerable<Type> pushed)
            : this
            (
                popped.Select(s => TypeOnStack.Get(s)),
                pushed.Select(s => TypeOnStack.Get(s))
            )
        { }

        internal StackTransition(int sizeMustBe)
            : this(new TypeOnStack[0], new TypeOnStack[0])
        {
            StackSizeMustBe = sizeMustBe;
        }

        internal StackTransition(bool isDuplicate)
            : this(new TypeOnStack[0], new [] { TypeOnStack.Get<WildcardType>() })
        {
            IsDuplicate = isDuplicate;
        }

        internal StackTransition(IEnumerable<TypeOnStack> popped, IEnumerable<TypeOnStack> pushed)
        {
            PoppedFromStack = popped.ToList().AsReadOnly();
            PushedToStack = pushed.ToList().AsReadOnly();
        }

        public override string ToString()
        {
            return "(" + string.Join(", ", PoppedFromStack.Select(p => p.ToString()).ToArray()) + ") => (" + string.Join(", ", PushedToStack.Select(p => p.ToString()).ToArray()) + ")";
        }

        public static StackTransition[] None() { return new[] { new StackTransition(Type.EmptyTypes, Type.EmptyTypes) }; }
        public static StackTransition[] Push<PushType>() { return Push(typeof(PushType)); }
        public static StackTransition[] Push(Type pushType) { return Push(TypeOnStack.Get(pushType)); }
        internal static StackTransition[] Push(TypeOnStack pushType) { return new[] { new StackTransition(new TypeOnStack[0], new[] { pushType }) }; }

        public static StackTransition[] Pop<PopType>() { return Pop(typeof(PopType)); }
        public static StackTransition[] Pop(Type popType) { return Pop(TypeOnStack.Get(popType)); }
        internal static StackTransition[] Pop(TypeOnStack popType) { return new[] { new StackTransition(new[] { popType }, new TypeOnStack[0]) }; }
    }

    public class InstrAndTransitions
    {
        public IEnumerable<StackTransition> Transitions { get; private set; }
        public OpCode? Instruction { get; private set; }

        public InstrAndTransitions(OpCode? instr, IEnumerable<StackTransition> trans)
        {
            Instruction = instr;
            Transitions = trans;
        }
    }

    public class VerificationResult
    {
        public bool Success { get; private set; }
        internal Stack<IEnumerable<TypeOnStack>> Stack { get; private set; }
        public int StackSize { get { return Stack.Count(); } }

        // Set when the stack is underflowed
        public bool IsStackUnderflow { get; private set; }
        public int ExpectedStackSize { get; private set; }

        // Set when stacks don't match during an incoming
        public bool IsStackMismatch { get; private set; }
        internal Stack<IEnumerable<TypeOnStack>> ExpectedStack { get; private set; }
        internal Stack<IEnumerable<TypeOnStack>> IncomingStack { get; private set; }

        // Set when types are dodge
        public bool IsTypeMismatch { get; private set; }
        public int TransitionIndex { get; private set; }
        public int StackIndex { get; private set; }
        internal IEnumerable<TypeOnStack> ExpectedAtStackIndex { get; private set; }

        // Set when the stack was expected to be a certain size, but it wasn't
        public bool IsStackSizeFailure {get; private set;}

        internal static VerificationResult Successful(Stack<IEnumerable<TypeOnStack>> stack)
        {
            return new VerificationResult { Success = true, Stack = stack };
        }

        internal static VerificationResult FailureUnderflow(int expectedSize)
        {
            return
                new VerificationResult
                {
                    Success = false,

                    IsStackUnderflow = true,
                    ExpectedStackSize = expectedSize
                };
        }

        internal static VerificationResult FailureStackMismatch(Stack<IEnumerable<TypeOnStack>> expected, Stack<IEnumerable<TypeOnStack>> incoming)
        {
            return
                new VerificationResult
                {
                    Success = false,

                    IsStackMismatch = true,
                    ExpectedStack = expected,
                    IncomingStack = incoming
                };
        }

        internal static VerificationResult FailureTypeMismatch(int transitionIndex, int stackIndex, IEnumerable<TypeOnStack> expectedTypes, Stack<IEnumerable<TypeOnStack>> stack)
        {
            return
                new VerificationResult
                {
                    Success = false,

                    IsTypeMismatch = true,
                    TransitionIndex =transitionIndex,
                    StackIndex = stackIndex,
                    ExpectedAtStackIndex = expectedTypes,
                    Stack = stack
                };
        }

        internal static VerificationResult FailureStackSize(int expectedSize)
        {
            return 
                new VerificationResult
                {
                    Success = false,

                    IsStackSizeFailure = true,
                    ExpectedStackSize = expectedSize
                };
        }
    }

    public class VerifiableTracker
    {
        // When the stack is "unbased" or "baseless", underflowing it results in wildcards
        //   eventually they'll be fixed up to actual types
        private bool Baseless;
        private List<InstrAndTransitions> Transitions = new List<InstrAndTransitions>();

        public VerifiableTracker(bool baseless = false) { Baseless = baseless; }

        public VerificationResult Transition(InstrAndTransitions legalTransitions)
        {
            Transitions.Add(legalTransitions);
            var ret = CollapseAndVerify();

            // revert!
            if (!ret.Success)
            {
                Transitions.RemoveAt(Transitions.Count - 1);
            }

            return ret;
        }

        private static Stack<IEnumerable<TypeOnStack>> GetStack(VerifiableTracker tracker)
        {
            var retStack = new Stack<IEnumerable<TypeOnStack>>();
            foreach (var t in tracker.Transitions)
            {
                UpdateStack(retStack, t);
            }

            return retStack;
        }

        private bool IsEquivalent(VerifiableTracker other)
        {
            var ourStack = GetStack(this);
            var otherStack = GetStack(other);

            if (ourStack.Count != otherStack.Count) return false;

            for (var i = 0; i < ourStack.Count; i++)
            {
                var ours = ourStack.ElementAt(i);
                var theirs = otherStack.ElementAt(i);

                if (ours.Count() != theirs.Count()) return false;

                if (ours.Any(o => !theirs.Any(t => o == t))) return false;
            }

            return true;
        }

        public VerificationResult Incoming(VerifiableTracker other)
        {
            // If we're not baseless, we can't modify ourselves; but we have to make sure the other one is equivalent
            if (!Baseless)
            {
                var isEquivalent = IsEquivalent(other);

                if (isEquivalent)
                {
                    return VerificationResult.Successful(GetStack(this));
                }

                return VerificationResult.FailureStackMismatch(GetStack(this), GetStack(other));
            }

            var old = Transitions;

            Transitions = new List<InstrAndTransitions>();
            Transitions.AddRange(other.Transitions);
            Transitions.AddRange(old);

            var ret = CollapseAndVerify();
            
            if (!ret.Success)
            {
                // revert!
                Transitions = old;
            }
            else
            {
                // we're no longer baseless if the other guy isn't
                this.Baseless = other.Baseless;
            }

            return ret;
        }

        private static void UpdateStack(Stack<IEnumerable<TypeOnStack>> stack, InstrAndTransitions wrapped)
        {
            var legal = wrapped.Transitions;
            var instr = wrapped.Instruction;

            if (legal.Any(l => l.PoppedFromStack.Any(u => u == TypeOnStack.Get<PopAllType>())))
            {
                if (instr.HasValue)
                {
                    for (var i = 0; i < stack.Count; i++)
                    {
                        //stack.Each(x => x.Each(y => y.Mark(instr.Value)));
                        var ix = stack.Count - i - 1;
                        stack.ElementAt(i).Each(y => y.Mark(instr.Value, ix));
                    }
                }

                stack.Clear();
            }
            else
            {
                var toPop = legal.First().PoppedCount;

                for (var j = 0; j < toPop && stack.Count > 0; j++)
                {
                    var popped = stack.Pop();

                    if (instr.HasValue)
                    {
                        var ix = toPop - j - 1;
                        popped.Each(y => y.Mark(instr.Value, ix));
                    }
                }
            }

            var toPush = new List<TypeOnStack>();
            foreach (var op in legal)
            {
                toPush.AddRange(op.PushedToStack);
            }

            if (toPush.Count > 0)
            {
                stack.Push(toPush.Distinct().ToList());
            }
        }

        public VerificationResult CollapseAndVerify()
        {
            var runningStack = new Stack<IEnumerable<TypeOnStack>>();

            for (var i = 0; i < Transitions.Count; i++)
            {
                var wrapped = Transitions[i];
                var ops = wrapped.Transitions;

                if(ops.Any(o => o.StackSizeMustBe.HasValue))
                {
                    if(ops.Count() > 1) throw new Exception("Shouldn't have multiple 'must be size' transitions at the same point");
                    var doIt = ops.Single();

                    if(doIt.StackSizeMustBe != runningStack.Count)
                    {
                        return VerificationResult.FailureStackSize(doIt.StackSizeMustBe.Value);
                    }
                }

                var legal =
                    ops.Where(
                        w =>
                        {
                            if (w.PoppedFromStack.All(u => u == TypeOnStack.Get<PopAllType>())) return true;

                            var onStack = runningStack.Peek(Baseless, w.PoppedCount);

                            if (onStack == null)
                            {
                                return false;
                            }

                            for (var j = 0; j < w.PoppedCount; j++)
                            {
                                var shouldBe = w.PoppedFromStack.ElementAt(j);
                                var actuallyIs = onStack[j];

                                if (!actuallyIs.Any(a => shouldBe.IsAssignableFrom(a)))
                                {
                                    return false;
                                }
                            }

                            return true;
                        }
                    ).ToList();

                if (legal.Count == 0)
                {
                    var wouldPop = ops.GroupBy(g => g.PoppedFromStack.Count()).Single().Key;

                    if (runningStack.Count < wouldPop)
                    {
                        return VerificationResult.FailureUnderflow(wouldPop);
                    }

                    IEnumerable<TypeOnStack> expected;
                    var stackI = FindStackFailureIndex(runningStack, ops, out expected);

                    return VerificationResult.FailureTypeMismatch(i, stackI, expected, runningStack);
                }

                if (legal.GroupBy(g => new { a = g.PoppedCount, b = g.PushedToStack.Count() }).Count() > 1)
                {
                    throw new Exception("Shouldn't be possible; legal transitions should have same push/pop #s");
                }

                // No reason to do all this work again
                Transitions[i] = new InstrAndTransitions(wrapped.Instruction, legal);

                bool popAll = legal.Any(l => l.PoppedFromStack.Contains(TypeOnStack.Get<PopAllType>()));
                if (popAll && legal.Count() != 1)
                {
                    throw new Exception("PopAll cannot coexist with any other transitions");
                }

                if(!popAll)
                {
                    var toPop = legal.First().PoppedCount;

                    if (toPop > runningStack.Count && !Baseless)
                    {
                        return VerificationResult.FailureUnderflow(toPop);
                    }
                }

                bool isDuplicate = legal.Any(l => l.IsDuplicate);
                if (isDuplicate && legal.Count() > 1)
                {
                    throw new Exception("Duplicate must be only transition");
                }

                if (isDuplicate)
                {
                    if (!Baseless && runningStack.Count == 0) return VerificationResult.FailureUnderflow(1);

                    IEnumerable<TypeOnStack> toPush = runningStack.Count > 0 ? runningStack.Peek() : new[] { TypeOnStack.Get<WildcardType>() };

                    UpdateStack(runningStack, new InstrAndTransitions(wrapped.Instruction, new StackTransition[] { new StackTransition(new TypeOnStack[0], toPush) }));
                }
                else
                {
                    UpdateStack(runningStack, new InstrAndTransitions(wrapped.Instruction, legal));
                }
            }

            return VerificationResult.Successful(runningStack);
        }

        private int FindStackFailureIndex(Stack<IEnumerable<TypeOnStack>> types, IEnumerable<StackTransition> ops, out IEnumerable<TypeOnStack> expected)
        {
            var stillLegal = new List<StackTransition>(ops);

            for (var i = 0; i < types.Count; i++)
            {
                var actuallyIs = types.ElementAt(i);

                var legal = stillLegal.Where(l => actuallyIs.Any(a => l.PoppedFromStack.ElementAt(i).IsAssignableFrom(a))).ToList();

                if (legal.Count == 0)
                {
                    expected = stillLegal.Select(l => l.PoppedFromStack.ElementAt(i)).Distinct().ToList();
                    return i;
                }

                stillLegal = legal;
            }

            throw new Exception("Shouldn't be possible");
        }

        public VerifiableTracker Clone()
        {
            return
                new VerifiableTracker
                {
                    Baseless = Baseless,
                    Transitions = Transitions.ToList()
                };
        }

        // Returns the current stack *if* it can be inferred down to single types *and* is either based or verifiable to the given depth
        internal Stack<TypeOnStack> InferStack(int ofDepth)
        {
            var res = CollapseAndVerify();

            if(res.Stack.Count < ofDepth) return null;

            var ret = new Stack<TypeOnStack>();
            for (var i = ofDepth - 1; i >= 0; i--)
            {
                var couldBe = res.Stack.ElementAt(i);

                if (couldBe.Count() > 1) return null;

                ret.Push(couldBe.Single());
            }

            return ret;
        }
    }
}
