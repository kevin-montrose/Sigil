using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        private void ValidateLabels()
        {
            if (UnusedLocals.Count != 0)
            {
                throw 
                    new SigilVerificationException(
                        "Locals [" + string.Join(", ", UnusedLocals.Select(u => u.Name)) + "] were declared but never used", 
                        IL,
                        Stack
                    );
            }

            if (UnusedLabels.Count != 0)
            {
                throw 
                    new SigilVerificationException(
                        "Labels [" + string.Join(", ", UnusedLabels.Select(l => l.Name)) + "] were declared but never used", 
                        IL,
                        Stack
                    );
            }
        }

        private void ValidateBranches()
        {
            foreach (var kv in Branches)
            {
                var mark = Marks[kv.Value.Item1].Item1;

                if (!kv.Key.AreEquivalent(mark))
                {
                    throw 
                        new SigilVerificationException(
                            "Branch to " + kv.Value.Item1 + " has a stack that doesn't match the destination", 
                            IL,
                            kv.Key, 
                            mark
                        );
                }
            }
        }

        private void ValidateTryCatchFinallyBlocks()
        {
            foreach (var kv in TryBlocks)
            {
                if (kv.Value.Item2 == -1)
                {
                    throw 
                        new SigilVerificationException(
                            "Unended ExceptionBlock " + kv.Key, 
                            IL,
                            Stack
                        );
                }
            }

            foreach (var kv in CatchBlocks)
            {
                if (kv.Value.Item2 == -1)
                {
                    throw new Exception("Invalid State, all ExceptionBlocks are ended but CatchBlock " + kv.Key + " isn't ended");
                }
            }

            foreach (var kv in FinallyBlocks)
            {
                if (kv.Value.Item2 == -1)
                {
                    throw new Exception("Invalid State, all ExceptionBlocks are ended but FinallyBlock " + kv.Key + " isn't ended");
                }
            }
        }

        private void ValidateTryCatchFinallyBranches()
        {
            foreach (var branch in Branches)
            {
                var instr = BranchPatches[branch.Value.Item2];

                var toLabel = branch.Value.Item1;
                var fromIndex = branch.Value.Item2;

                var toIndex = Marks[toLabel].Item2;

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
                                IL
                            );
                    }
                }

                if (fromFinallyBlock != null && toFinallyBlock != fromFinallyBlock)
                {
                    throw 
                        new SigilVerificationException(
                            "Cannot branch from inside " + fromFinallyBlock + " to outside, exit the ExceptionBlock first",
                            IL
                        );
                }

                if (toFinallyBlock != null && fromFinallyBlock != toFinallyBlock)
                {
                    throw 
                        new SigilVerificationException(
                            "Cannot branch into a FinallyBlock",
                            IL
                        );
                }

                if (fromTryBlock != null && toTryBlock != fromTryBlock)
                {
                    if (instr.Item3 != OpCodes.Leave)
                    {
                        throw 
                            new SigilVerificationException(
                                "Cannot branch from inside " + fromTryBlock + " to outside, exit the ExceptionBlock first",
                                IL
                            );
                    }
                }
            }
        }

        /// <summary>
        /// Called to confirm that the IL emit'd to date can be turned into a delegate without error.
        /// 
        /// Checks that the stack is empty, that all paths returns, that all labels are marked, etc. etc.
        /// </summary>
        private void Validate()
        {
            if (!Stack.IsRoot)
            {
                throw new SigilVerificationException("Delegates must leave their stack empty when they end", IL, Stack);
            }

            var lastInstr = InstructionStream.LastOrDefault();

            if (lastInstr == null || lastInstr.Item1 != OpCodes.Ret)
            {
                throw new SigilVerificationException("Delegate must end with Return", IL, Stack);
            }

            ValidateLabels();

            ValidateBranches();

            ValidateTryCatchFinallyBlocks();

            ValidateTryCatchFinallyBranches();
        }
    }
}
