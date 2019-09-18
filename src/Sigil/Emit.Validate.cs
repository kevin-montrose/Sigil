using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        private void FailStackUnderflow(int expected, string method)
        {
            if (expected == 1)
            {
                throw new SigilVerificationException(method + " expects a value on the stack, but it was empty", IL.Instructions(AllLocals));
            }

            throw new SigilVerificationException(method + " expects " + expected + " values on the stack", IL.Instructions(AllLocals));
        }

        private void FailOwnership(IOwned obj)
        {
            throw new ArgumentException(obj + " is not owned by this Emit, and thus cannot be used");
        }

        private void FailUnverifiable(string method)
        {
            throw new InvalidOperationException(method + " isn't verifiable");
        }

        private void ValidateTryCatchFinallyBlocks()
        {
            foreach (var kv in TryBlocks.AsEnumerable())
            {
                if (kv.Value.Item2 == -1)
                {
                    throw 
                        new SigilVerificationException(
                            "Unended ExceptionBlock " + kv.Key,
                            IL.Instructions(AllLocals)
                        );
                }
            }

            foreach (var kv in CatchBlocks.AsEnumerable())
            {
                if (kv.Value.Item2 == -1)
                {
                    throw new Exception("Invalid State, all ExceptionBlocks are ended but CatchBlock " + kv.Key + " isn't ended");
                }
            }

            foreach (var kv in FinallyBlocks.AsEnumerable())
            {
                if (kv.Value.Item2 == -1)
                {
                    throw new Exception("Invalid State, all ExceptionBlocks are ended but FinallyBlock " + kv.Key + " isn't ended");
                }
            }
        }

        private void ValidateTryCatchFinallyBranches()
        {
            foreach (var branch in Branches.AsEnumerable())
            {
                var instr = BranchPatches[branch.Item3];

                var toLabel = branch.Item2;
                var fromIndex = branch.Item3;

                var toIndex = Marks[toLabel];

                var fromTryBlocks = TryBlocks.Where(t => fromIndex >= t.Value.Item1 && fromIndex <= t.Value.Item2).ToList();
                var fromCatchBlocks = CatchBlocks.Where(c => fromIndex >= c.Value.Item1 && fromIndex <= c.Value.Item2).ToList();
                var fromFinallyBlocks = FinallyBlocks.Where(f => fromIndex >= f.Value.Item1 && fromIndex <= f.Value.Item2).ToList();

                var toTryBlocks = TryBlocks.Where(t => toIndex >= t.Value.Item1 && toIndex <= t.Value.Item2).ToList();
                var toCatchBlocks = CatchBlocks.Where(c => toIndex >= c.Value.Item1 && toIndex <= c.Value.Item2).ToList();
                var toFinallyBlocks = FinallyBlocks.Where(f => toIndex >= f.Value.Item1 && toIndex <= f.Value.Item2).ToList();

                var fromTryBlock = fromTryBlocks.OrderByDescending(t => t.Value.Item1).Select(t => t.Key).FirstOrDefault();
                var fromCatchBlock = fromCatchBlocks.OrderByDescending(c => c.Value.Item1).Select(c => c.Key).FirstOrDefault();
                var fromFinallyBlock = fromFinallyBlocks.OrderByDescending(f => f.Value.Item1).Select(f => f.Key).FirstOrDefault();

                var toTryBlock = toTryBlocks.OrderByDescending(t => t.Value.Item1).Select(t => t.Key).FirstOrDefault();
                var toCatchBlock = toCatchBlocks.OrderByDescending(c => c.Value.Item1).Select(c => c.Key).FirstOrDefault();
                var toFinallyBlock = toFinallyBlocks.OrderByDescending(f => f.Value.Item1).Select(f => f.Key).FirstOrDefault();

                // Nothing funky going on, carry on
                if (fromTryBlock == null && fromCatchBlock == null && fromFinallyBlock == null && toTryBlock == null && toCatchBlock == null && toFinallyBlock == null)
                {
                    continue;
                }

                if (fromCatchBlock != null && toCatchBlock != fromCatchBlock)
                {
                    if (instr.Item3 != OpCodes.Leave)
                    {
                        throw 
                            new SigilVerificationException(
                                "Cannot branch from inside " + fromCatchBlock + " to outside, exit the ExceptionBlock first",
                                IL.Instructions(AllLocals)
                            );
                    }
                }

                if (fromFinallyBlock != null && toFinallyBlock != fromFinallyBlock)
                {
                    throw 
                        new SigilVerificationException(
                            "Cannot branch from inside " + fromFinallyBlock + " to outside, exit the ExceptionBlock first",
                            IL.Instructions(AllLocals)
                        );
                }

                if (toFinallyBlock != null && fromFinallyBlock != toFinallyBlock)
                {
                    throw 
                        new SigilVerificationException(
                            "Cannot branch into a FinallyBlock",
                            IL.Instructions(AllLocals)
                        );
                }

                if (fromTryBlock != null && toTryBlock != fromTryBlock)
                {
                    if (instr.Item3 != OpCodes.Leave)
                    {
                        throw 
                            new SigilVerificationException(
                                "Cannot branch from inside " + fromTryBlock + " to outside, exit the ExceptionBlock first",
                                IL.Instructions(AllLocals)
                            );
                    }
                }
            }
        }

        /// <summary>
        /// <para>Called to confirm that the IL emit'd to date can be turned into a delegate without error.</para>
        /// <para>Checks that the stack is empty, that all paths returns, that all labels are marked, etc. etc.</para>
        /// </summary>
        private void Validate()
        {
            if (!IsVerifying) return;

            var tracer = new ReturnTracer(Branches, Marks, Returns, Throws);

            var result = tracer.Verify();
            if (!result.IsSuccess)
            {
                throw new SigilVerificationException("All execution paths must end with Return", result, IL.Instructions(AllLocals));
            }

            ValidateTryCatchFinallyBlocks();

            ValidateTryCatchFinallyBranches();
        }
    }
}
