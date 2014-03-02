using System;
using System.Collections.Generic;
using System.Text;

namespace Sigil.Impl
{
    internal class RollingVerifierWithoutVerification : RollingVerifier
    {
        public RollingVerifierWithoutVerification(Label beginAt)
            : base(beginAt, strictBranchVerification: false)
        { }

        public override VerificationResult ConditionalBranch(params Label[] toLabels)
        {
            return VerificationResult.Successful();
        }

        public override LinqStack<TypeOnStack> InferStack(int ofDepth)
        {
            return null;
        }

        public override VerificationResult Mark(Label label)
        {
            return VerificationResult.Successful();
        }

        public override VerificationResult ReThrow()
        {
            return VerificationResult.Successful();
        }

        public override VerificationResult Return()
        {
            return VerificationResult.Successful();
        }

        public override VerificationResult Throw()
        {
            return VerificationResult.Successful();
        }

        public override VerificationResult Transition(InstructionAndTransitions legalTransitions)
        {
            return VerificationResult.Successful();
        }

        public override VerificationResult UnconditionalBranch(Label to)
        {
            return VerificationResult.Successful();
        }
    }
}
