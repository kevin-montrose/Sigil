using Sigil.Impl;
using System;
using System.Collections.Generic;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        private Dictionary<string, ExceptionBlock> DisassembledExceptionBlocks;
        private Dictionary<string, CatchBlock> DisassembledCatchBlocks;
        private Dictionary<string, FinallyBlock> DisassembledFinallyBlocks;

        // A version of BeginExceptionBlock used by the Disassembler to keep track of
        //   exception blocks while re-emitting
        internal void BeginExceptionBlock(string storeUnderName)
        {
            var block = BeginExceptionBlock();

            if (DisassembledExceptionBlocks == null)
            {
                DisassembledExceptionBlocks = new Dictionary<string, ExceptionBlock>();
            }

            DisassembledExceptionBlocks[storeUnderName] = block;
        }

        internal void EndExceptionBlock(string lookupByName)
        {
            var block = DisassembledExceptionBlocks[lookupByName];

            EndExceptionBlock(block);

            DisassembledExceptionBlocks.Remove(lookupByName);
        }

        internal void BeginCatchBlock(string lookupExcName, Type exceptionType, string storeUnderName)
        {
            var block = DisassembledExceptionBlocks[lookupExcName];
            if (DisassembledCatchBlocks == null)
            {
                DisassembledCatchBlocks = new Dictionary<string, CatchBlock>();
            }

            DisassembledCatchBlocks[storeUnderName] = BeginCatchBlock(block, exceptionType);
        }

        internal void EndCatchBlock(string lookupByName)
        {
            var c = DisassembledCatchBlocks[lookupByName];
            EndCatchBlock(c);

            DisassembledCatchBlocks.Remove(lookupByName);
        }

        internal void BeginFinallyBlock(string lookupExcName, string storeUnderName)
        {
            var block = DisassembledExceptionBlocks[lookupExcName];
            if (DisassembledFinallyBlocks == null)
            {
                DisassembledFinallyBlocks = new Dictionary<string, FinallyBlock>();
            }

            DisassembledFinallyBlocks[storeUnderName] = BeginFinallyBlock(block);
        }

        internal void EndFinallyBlock(string lookupByName)
        {
            var f = DisassembledFinallyBlocks[lookupByName];
            EndFinallyBlock(f);

            DisassembledFinallyBlocks.Remove(lookupByName);
        }

        /// <summary>
        /// Start a new exception block.  This is roughly analogous to a `try` block in C#, but an exception block contains it's catch and finally blocks.
        /// </summary>
        public ExceptionBlock BeginExceptionBlock()
        {
            if (MustMark)
            {
                MarkLabel(DefineLabel(AutoNamer.Next(this, "__autolabel")));
            }

            UpdateState(Wrap(new[] { new StackTransition(0) }, "BeginExceptionBlock"));

            var labelDel = IL.BeginExceptionBlock();
            var label = new Label(this, labelDel, AutoNamer.Next(this, "__exceptionBlockEnd"));

            CurrentLabels[label.Name] = label;

            var ret = new ExceptionBlock(label);

            TryBlocks[ret] = SigilTuple.Create(IL.Index, -1);

            CurrentExceptionBlock.Push(ret);

            return ret;
        }

        /// <summary>
        /// Start a new exception block.  This is roughly analogous to a `try` block in C#, but an exception block contains it's catch and finally blocks.
        /// </summary>
        public Emit<DelegateType> BeginExceptionBlock(out ExceptionBlock forTry)
        {
            forTry = BeginExceptionBlock();

            return this;
        }

        /// <summary>
        /// Ends the given exception block.
        /// 
        /// All catch and finally blocks associated with the given exception block must be ended before this method is called.
        /// </summary>
        public Emit<DelegateType> EndExceptionBlock(ExceptionBlock forTry)
        {
            if (forTry == null)
            {
                throw new ArgumentNullException("forTry");
            }

            if (((IOwned)forTry).Owner != this)
            {
                FailOwnership(forTry);
            }

            var location = TryBlocks[forTry];

            // Can't close the same exception block twice
            if (location.Item2 != -1)
            {
                throw new InvalidOperationException("ExceptionBlock has already been ended");
            }

            if (CurrentExceptionBlock.Count > 0 && forTry != CurrentExceptionBlock.Peek())
            {
                throw new InvalidOperationException("Cannot end outer ExceptionBlock " + forTry + " while inner EmitExceptionBlock " + CurrentExceptionBlock.Peek() + " is open");
            }

            // Can't close an exception block while there are outstanding catch blocks
            foreach (var kv in CatchBlocks.AsEnumerable())
            {
                if (kv.Key.ExceptionBlock != forTry) continue;

                if (kv.Value.Item2 == -1)
                {
                    throw new InvalidOperationException("Cannot end ExceptionBlock, CatchBlock " + kv.Key + " has not been ended");
                }
            }

            foreach (var kv in FinallyBlocks.AsEnumerable())
            {
                if (kv.Key.ExceptionBlock != forTry) continue;

                if (kv.Value.Item2 == -1)
                {
                    throw new InvalidOperationException("Cannot end ExceptionBlock, FinallyBlock " + kv.Key + " has not been ended");
                }
            }

            if (!CatchBlocks.Any(k => k.Key.ExceptionBlock == forTry) && !FinallyBlocks.Any(k => k.Key.ExceptionBlock == forTry))
            {
                throw new InvalidOperationException("Cannot end ExceptionBlock without defining at least one of a catch or finally block");
            }

            IL.EndExceptionBlock();

            TryBlocks[forTry] = SigilTuple.Create(location.Item1, IL.Index);

            Marks[forTry.Label] = IL.Index;

            CurrentExceptionBlock.Pop();

            if (MustMark)
            {
                MarkLabel(DefineLabel(AutoNamer.Next(this, "__autolabel")));
            }

            return this;
        }

        /// <summary>
        /// Begins a catch block for the given exception type in the given exception block.
        /// 
        /// The given exception block must still be open.
        /// </summary>
        public CatchBlock BeginCatchBlock<ExceptionType>(ExceptionBlock forTry)
        {
            return BeginCatchBlock(forTry, typeof(ExceptionType));
        }

        /// <summary>
        /// Begins a catch block for the given exception type in the given exception block.
        /// 
        /// The given exception block must still be open.
        /// </summary>
        public Emit<DelegateType> BeginCatchBlock<ExceptionType>(ExceptionBlock forTry, out CatchBlock forCatch)
        {
            forCatch = BeginCatchBlock<ExceptionType>(forTry);

            return this;
        }

        /// <summary>
        /// Begins a catch block for all exceptions in the given exception block
        ///
        /// The given exception block must still be open.
        /// 
        /// Equivalent to BeginCatchBlock(typeof(Exception), forTry).
        /// </summary>
        public CatchBlock BeginCatchAllBlock(ExceptionBlock forTry)
        {
            return BeginCatchBlock<Exception>(forTry);
        }

        /// <summary>
        /// Begins a catch block for all exceptions in the given exception block
        ///
        /// The given exception block must still be open.
        /// 
        /// Equivalent to BeginCatchBlock(typeof(Exception), forTry).
        /// </summary>
        public Emit<DelegateType> BeginCatchAllBlock(ExceptionBlock forTry, out CatchBlock forCatch)
        {
            forCatch = BeginCatchAllBlock(forTry);

            return this;
        }

        /// <summary>
        /// Begins a catch block for the given exception type in the given exception block.
        /// 
        /// The given exception block must still be open.
        /// </summary>
        public CatchBlock BeginCatchBlock(ExceptionBlock forTry, Type exceptionType)
        {
            if (exceptionType == null)
            {
                throw new ArgumentNullException("exceptionType");
            }

            if (forTry == null)
            {
                throw new ArgumentNullException("forTry");
            }

            if (((IOwned)forTry).Owner != this)
            {
                FailOwnership(forTry);
            }

            if (CurrentExceptionBlock.Count > 0 && forTry != CurrentExceptionBlock.Peek())
            {
                throw new InvalidOperationException("Cannot start CatchBlock on " + forTry + " while inner ExceptionBlock is still open");
            }

            if (!TypeHelpers.IsAssignableFrom(typeof(Exception), exceptionType))
            {
                throw new ArgumentException("BeginCatchBlock expects a type descending from Exception, found " + exceptionType, "exceptionType");
            }

            var currentlyOpen = CatchBlocks.Where(c => c.Key.ExceptionBlock == forTry && c.Value.Item2 == -1).Select(s => s.Key).SingleOrDefault();
            if (currentlyOpen != null)
            {
                throw new InvalidOperationException("Cannot start a new catch block, " + currentlyOpen + " has not been ended");
            }

            if (MustMark)
            {
                MarkLabel(DefineLabel(AutoNamer.Next(this, "__autolabel")));
            }

            UpdateState(Wrap(new[] { new StackTransition(0) }, "BeginCatchBlock"));

            var tryBlock = TryBlocks[forTry];

            if (tryBlock.Item2 != -1)
            {
                throw new SigilVerificationException("BeginCatchBlock expects an unclosed exception block, but " + forTry + " is already closed", IL.Instructions(AllLocals));
            }

            IL.BeginCatchBlock(exceptionType);

            UpdateState(Wrap(StackTransition.Push(exceptionType), "BeginCatchBlock"));

            var ret = new CatchBlock(exceptionType, forTry);

            CatchBlocks[ret] = SigilTuple.Create(IL.Index, -1);

            return ret;
        }

        /// <summary>
        /// Begins a catch block for the given exception type in the given exception block.
        /// 
        /// The given exception block must still be open.
        /// </summary>
        public Emit<DelegateType> BeginCatchBlock(ExceptionBlock forTry, Type exceptionType, out CatchBlock forCatch)
        {
            forCatch = BeginCatchBlock(forTry, exceptionType);

            return this;
        }

        /// <summary>
        /// Ends the given catch block.
        /// </summary>
        public Emit<DelegateType> EndCatchBlock(CatchBlock forCatch)
        {
            if (forCatch == null)
            {
                throw new ArgumentNullException("forCatch");
            }

            if (((IOwned)forCatch).Owner != this)
            {
                FailOwnership(forCatch);
            }

            if (MustMark)
            {
                MarkLabel(DefineLabel(AutoNamer.Next(this, "__autolabel")));
            }

            UpdateState(Wrap(new[] { new StackTransition(0) }, "EndCatchBlock"));

            var location = CatchBlocks[forCatch];

            if (location.Item2 != -1)
            {
                throw new InvalidOperationException("CatchBlock has already been ended");
            }

            IL.EndCatchBlock();

            CatchBlocks[forCatch] = SigilTuple.Create(location.Item1, IL.Index);

            return this;
        }

        /// <summary>
        /// Begins a finally block on the given exception block.
        /// 
        /// Only one finally block can be defined per exception block, and the block cannot appear within a catch block.
        /// 
        /// The given exception block must still be open.
        /// </summary>
        public Emit<DelegateType> BeginFinallyBlock(ExceptionBlock forTry, out FinallyBlock forFinally)
        {
            forFinally = BeginFinallyBlock(forTry);

            return this;
        }

        /// <summary>
        /// Begins a finally block on the given exception block.
        /// 
        /// Only one finally block can be defined per exception block, and the block cannot appear within a catch block.
        /// 
        /// The given exception block must still be open.
        /// </summary>
        public FinallyBlock BeginFinallyBlock(ExceptionBlock forTry)
        {
            if (forTry == null)
            {
                throw new ArgumentNullException("forTry");
            }

            if (((IOwned)forTry).Owner != this)
            {
                FailOwnership(forTry);
            }

            var tryBlock = TryBlocks[forTry];

            if (tryBlock.Item2 != -1)
            {
                throw new InvalidOperationException("BeginFinallyBlock expects an unclosed exception block, but " + forTry + " is already closed");
            }

            if (CurrentExceptionBlock.Count > 0 && forTry != CurrentExceptionBlock.Peek())
            {
                throw new InvalidOperationException("Cannot begin FinallyBlock on " + forTry + " while inner ExceptionBlock " + CurrentExceptionBlock.Peek() + " is still open");
            }

            if (FinallyBlocks.Any(kv => kv.Key.ExceptionBlock == forTry))
            {
                throw new InvalidOperationException("There can only be one finally block per ExceptionBlock, and one is already defined for " + forTry);
            }

            if (MustMark)
            {
                MarkLabel(DefineLabel(AutoNamer.Next(this, "__autolabel")));
            }

            UpdateState(Wrap(new[] { new StackTransition(0) }, "BeginFinallyBlock"));

            var ret = new FinallyBlock(forTry);

            IL.BeginFinallyBlock();

            FinallyBlocks[ret] = SigilTuple.Create(IL.Index, -1);

            return ret;
        }

        /// <summary>
        /// Ends the given finally block.
        /// </summary>
        public Emit<DelegateType> EndFinallyBlock(FinallyBlock forFinally)
        {
            if (forFinally == null)
            {
                throw new ArgumentNullException("forFinally");
            }

            if (((IOwned)forFinally).Owner != this)
            {
                FailOwnership(forFinally);
            }

            var finallyBlock = FinallyBlocks[forFinally];

            if (finallyBlock.Item2 != -1)
            {
                throw new InvalidOperationException("EndFinallyBlock expects an unclosed finally block, but " + forFinally + " is already closed");
            }

            if (MustMark)
            {
                MarkLabel(DefineLabel(AutoNamer.Next(this, "__autolabel")));
            }

            UpdateState(Wrap(new[] { new StackTransition(0) }, "EndFinallyBlock"));

            IL.EndFinallyBlock();

            FinallyBlocks[forFinally] = SigilTuple.Create(finallyBlock.Item1, IL.Index);

            return this;
        }
    }
}
