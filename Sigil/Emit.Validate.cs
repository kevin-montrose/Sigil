using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        private void FailStackUnderflow(int expected
#if NET35
            , string method = "caller"
#else
            , [CallerMemberName]string method = null
#endif
)
        {
            if (expected == 1)
            {
                throw new SigilVerificationException(method + " expects a value on the stack, but it was empty", IL.Instructions(LocalsByIndex), Stack);
            }

            throw new SigilVerificationException(method + " expects " + expected + " values on the stack", IL.Instructions(LocalsByIndex), Stack);
        }

        private void FailOwnership(IOwned obj)
        {
            throw new ArgumentException(obj + " is not owned by this Emit, and thus cannot be used");
        }

        private void FailUnverifiable(
#if NET35
            string method = "caller"
#else
            [CallerMemberName]string method = null
#endif
            )
        {
            throw new InvalidOperationException(method + " isn't verifiable");
        }

        private void ValidateBranches()
        {
            foreach (var kv in Branches)
            {
                var mark = Marks[kv.Value.Item1].Item1;

                var branchLoc = kv.Value.Item2;
                var markLoc = Marks[kv.Value.Item1].Item2;

                if (!kv.Key.AreEquivalent(mark))
                {
                    throw 
                        new SigilVerificationException(
                            "Branch to " + kv.Value.Item1 + " has a stack that doesn't match the destination",
                            IL.Instructions(LocalsByIndex),
                            kv.Key,
                            branchLoc,
                            mark,
                            markLoc
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
                            IL.Instructions(LocalsByIndex),
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
                                IL.Instructions(LocalsByIndex)
                            );
                    }
                }

                if (fromFinallyBlock != null && toFinallyBlock != fromFinallyBlock)
                {
                    throw 
                        new SigilVerificationException(
                            "Cannot branch from inside " + fromFinallyBlock + " to outside, exit the ExceptionBlock first",
                            IL.Instructions(LocalsByIndex)
                        );
                }

                if (toFinallyBlock != null && fromFinallyBlock != toFinallyBlock)
                {
                    throw 
                        new SigilVerificationException(
                            "Cannot branch into a FinallyBlock",
                            IL.Instructions(LocalsByIndex)
                        );
                }

                if (fromTryBlock != null && toTryBlock != fromTryBlock)
                {
                    if (instr.Item3 != OpCodes.Leave)
                    {
                        throw 
                            new SigilVerificationException(
                                "Cannot branch from inside " + fromTryBlock + " to outside, exit the ExceptionBlock first",
                                IL.Instructions(LocalsByIndex)
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
                var stackSize = Stack.Count();
                var mark = new List<int>();
                for (var i = 0; i < stackSize; i++)
                {
                    mark.Add(i);
                }

                throw new SigilVerificationException("Delegates must leave their stack empty when they end", IL.Instructions(LocalsByIndex), Stack, mark.ToArray());
            }

            var lastInstr = InstructionStream.LastOrDefault();

            if (lastInstr == null || lastInstr.Item1 != OpCodes.Ret)
            {
                throw new SigilVerificationException("Delegate must end with Return", IL.Instructions(LocalsByIndex), Stack);
            }

            ValidateBranches();

            ValidateTryCatchFinallyBlocks();

            ValidateTryCatchFinallyBranches();
        }
    }
}
