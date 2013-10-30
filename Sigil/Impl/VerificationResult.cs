using System.Collections.Generic;

namespace Sigil.Impl
{
    internal class VerificationResult
    {
        public bool Success { get; private set; }
        public LinqStack<LinqList<TypeOnStack>> Stack { get; private set; }
        public int StackSize { get { return Stack != null ? Stack.Count : 0; } }

        public VerifiableTracker Verifier { get; private set; }

        // Set when the stack is underflowed
        public bool IsStackUnderflow { get; private set; }
        public int ExpectedStackSize { get; private set; }

        // Set when stacks don't match during an incoming
        public bool IsStackMismatch { get; private set; }
        public LinqStack<LinqList<TypeOnStack>> ExpectedStack { get; private set; }
        public LinqStack<LinqList<TypeOnStack>> IncomingStack { get; private set; }

        // Set when types are dodge
        public bool IsTypeMismatch { get; private set; }
        public int? TransitionIndex { get; private set; }
        public int StackIndex { get; private set; }
        public LinqRoot<TypeOnStack> ExpectedAtStackIndex { get; private set; }

        // Set when the stack was expected to be a certain size, but it wasn't
        public bool IsStackSizeFailure { get; private set; }

        public Label InvolvingLabel { get; private set; }

        public LinqRoot<TypeOnStack> ExpectedOnStack { get; private set; }
        public LinqRoot<TypeOnStack> ActuallyOnStack { get; private set; }

        public static VerificationResult Successful()
        {
            return new VerificationResult { Success = true };
        }

        public static VerificationResult Successful(VerifiableTracker verifier, LinqStack<LinqList<TypeOnStack>> stack)
        {
            return new VerificationResult { Success = true, Stack = stack, Verifier = verifier };
        }

        public static VerificationResult FailureUnderflow(Label involving, int expectedSize)
        {
            return
                new VerificationResult
                {
                    Success = false,

                    IsStackUnderflow = true,
                    ExpectedStackSize = expectedSize,

                    InvolvingLabel = involving
                };
        }

        public static VerificationResult FailureUnderflow(VerifiableTracker verifier, int transitionIndex, int expectedSize, LinqStack<LinqList<TypeOnStack>> stack)
        {
            return
                new VerificationResult
                {
                    Success = false,

                    Verifier = verifier.Clone(),
                    TransitionIndex = transitionIndex,

                    IsStackUnderflow = true,
                    ExpectedStackSize = expectedSize,
                    Stack = stack
                };
        }

        public static VerificationResult FailureStackMismatch(VerifiableTracker verifier, LinqStack<LinqList<TypeOnStack>> expected, LinqStack<LinqList<TypeOnStack>> incoming)
        {
            return
                new VerificationResult
                {
                    Success = false,

                    Verifier = verifier.Clone(),

                    IsStackMismatch = true,
                    ExpectedStack = expected,
                    IncomingStack = incoming
                };
        }

        public static VerificationResult FailureTypeMismatch(Label involving, LinqList<TypeOnStack> expected, LinqList<TypeOnStack> actual)
        {
            return
                new VerificationResult
                {
                    Success = false,

                    IsTypeMismatch = true,
                    ExpectedOnStack = expected,
                    ActuallyOnStack = actual
                };
        }

        public static VerificationResult FailureTypeMismatch(VerifiableTracker verifier, int transitionIndex, int stackIndex, IEnumerable<TypeOnStack> expectedTypes, LinqStack<LinqList<TypeOnStack>> stack)
        {
            return
                new VerificationResult
                {
                    Success = false,

                    Verifier = verifier.Clone(),
                    TransitionIndex = transitionIndex,

                    IsTypeMismatch = true,
                    StackIndex = stackIndex,
                    ExpectedAtStackIndex = LinqEnumerable<TypeOnStack>.For(expectedTypes),
                    Stack = stack
                };
        }

        public static VerificationResult FailureStackSize(VerifiableTracker verifier, int transitionIndex, int expectedSize)
        {
            return
                new VerificationResult
                {
                    Success = false,

                    Verifier = verifier.Clone(),
                    TransitionIndex = transitionIndex,

                    IsStackSizeFailure = true,
                    ExpectedStackSize = expectedSize
                };
        }
    }
}
