using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public EmitExceptionBlock BeginExceptionBlock()
        {
            if (!Stack.IsRoot)
            {
                throw new SigilException("Stack should be empty when BeginExceptionBlock is called", Stack);
            }

            var labelDel = IL.BeginExceptionBlock();
            var label = new EmitLabel(this, labelDel, "__exceptionBlockEnd");

            var ret = new EmitExceptionBlock(label);

            TryBlocks[ret] = Tuple.Create(IL.Index, -1);

            CurrentExceptionBlock.Push(ret);

            return ret;
        }

        public void EndExceptionBlock(EmitExceptionBlock forTry)
        {
            if (forTry == null)
            {
                throw new ArgumentNullException("forTry");
            }

            if (forTry.Owner != this)
            {
                throw new ArgumentException("forTry is not owned by this Emit, and thus cannot be used");
            }

            var location = TryBlocks[forTry];

            // Can't close the same exception block twice
            if (location.Item2 != -1)
            {
                throw new SigilException("EmitExceptionBlock has already been ended", Stack);
            }

            if (forTry != CurrentExceptionBlock.Peek())
            {
                throw new SigilException("Cannot end outer EmitExceptionBlock " + forTry + " while inner EmitExceptionBlock " + CurrentExceptionBlock.Peek() + " is open", Stack);
            }

            // Can't close an exception block while there are outstanding catch blocks
            foreach (var kv in CatchBlocks)
            {
                if (kv.Key.ExceptionBlock != forTry) continue;

                if (kv.Value.Item2 == -1)
                {
                    throw new SigilException("Cannot end EmitExceptionBlock, EmitCatchBlock " + kv.Key + " has not been ended", Stack);
                }
            }

            foreach (var kv in FinallyBlocks)
            {
                if (kv.Key.ExceptionBlock != forTry) continue;

                if (kv.Value.Item2 == -1)
                {
                    throw new SigilException("Cannot end EmitExceptionBlock, EmitFinallyBlock " + kv.Key + " has not been ended", Stack);
                }
            }

            if (!CatchBlocks.Any(k => k.Key.ExceptionBlock == forTry) && !FinallyBlocks.Any(k => k.Key.ExceptionBlock == forTry))
            {
                throw new SigilException("Cannot end EmitExceptionBlock without defining at least one of a catch or finally block", Stack);
            }

            IL.EndExceptionBlock();

            TryBlocks[forTry] = Tuple.Create(location.Item1, IL.Index);

            Stack = new StackState();

            Marks[forTry.Label] = Tuple.Create(Stack, IL.Index);

            CurrentExceptionBlock.Pop();
        }

        public EmitCatchBlock BeginCatchBlock<ExceptionType>(EmitExceptionBlock forTry)
        {
            return BeginCatchBlock(forTry, typeof(ExceptionType));
        }

        public EmitCatchBlock BeginCatchAllBlock(EmitExceptionBlock forTry)
        {
            return BeginCatchBlock<Exception>(forTry);
        }

        public EmitCatchBlock BeginCatchBlock(EmitExceptionBlock forTry, Type exceptionType)
        {
            if (exceptionType == null)
            {
                throw new ArgumentNullException("exceptionType");
            }

            if (forTry == null)
            {
                throw new ArgumentNullException("forTry");
            }

            if (forTry.Owner != this)
            {
                throw new ArgumentException("forTry is not owned by this Emit, and thus cannot be used");
            }

            if (forTry != CurrentExceptionBlock.Peek())
            {
                throw new SigilException("Cannot start EmitCatchBlock on " + forTry + " while inner EmitExceptionBlock is still open", Stack);
            }

            if (!Stack.IsRoot)
            {
                throw new SigilException("Stack should be empty when BeginCatchBlock is called", Stack);
            }

            if(!typeof(Exception).IsAssignableFrom(exceptionType))
            {
                throw new SigilException("BeginCatchBlock expects a type descending from Exception, found " + exceptionType, Stack);
            }

            var tryBlock = TryBlocks[forTry];

            if (tryBlock.Item2 != -1)
            {
                throw new SigilException("BeginCatchBlock expects an unclosed exception block, but " + forTry + " is already closed", Stack);
            }

            var currentlyOpen = CatchBlocks.Where(c => c.Key.ExceptionBlock == forTry && c.Value.Item2 == -1).Select(s => s.Key).SingleOrDefault();
            if (currentlyOpen != null)
            {
                throw new SigilException("Cannot start a new catch block, " + currentlyOpen + " has not been ended", Stack);
            }

            IL.BeginCatchBlock(exceptionType);
            Stack = new StackState();
            Stack = Stack.Push(TypeOnStack.Get(exceptionType));

            var ret = new EmitCatchBlock(this, exceptionType, forTry);

            CatchBlocks[ret] = Tuple.Create(IL.Index, -1);

            return ret;
        }

        public void EndCatchBlock(EmitCatchBlock forCatch)
        {
            if (forCatch == null)
            {
                throw new ArgumentNullException("forCatch");
            }

            if (forCatch.Owner != this)
            {
                throw new ArgumentException("forCatch is not owned by this Emit, and thus cannot be used");
            }

            if (!Stack.IsRoot)
            {
                throw new SigilException("Stack should be empty when EndCatchBlock is called", Stack);
            }

            var location = CatchBlocks[forCatch];

            if (location.Item2 != -1)
            {
                throw new SigilException("EmitCatchBlock  has already been ended", Stack);
            }

            // There's no equivalent to EndCatchBlock in raw ILGenerator, so no call here.
            //   But that's kind of weird from a just-in-time validation standpoint.

            Sigil.Impl.BufferedILGenerator.UpdateOpCodeDelegate update;
            UpdateState(OpCodes.Leave, forCatch.ExceptionBlock.Label.Label, out update);

            Branches[Stack] = Tuple.Create(forCatch.ExceptionBlock.Label, IL.Index);

            BranchPatches[IL.Index] = Tuple.Create(forCatch.ExceptionBlock.Label, update, OpCodes.Leave);

            CatchBlocks[forCatch] = Tuple.Create(location.Item1, IL.Index);
        }

        public EmitFinallyBlock BeginFinallyBlock(EmitExceptionBlock forTry)
        {
            if (forTry == null)
            {
                throw new ArgumentNullException("forTry");
            }

            if (forTry.Owner != this)
            {
                throw new ArgumentException("forTry is not owned by this Emit, and thus cannot be used");
            }

            var tryBlock = TryBlocks[forTry];

            if (tryBlock.Item2 != -1)
            {
                throw new SigilException("BeginFinallyBlock expects an unclosed exception block, but " + forTry + " is already closed", Stack);
            }

            if (forTry != CurrentExceptionBlock.Peek())
            {
                throw new SigilException("Cannot begin EmitFinallyBlock on " + tryBlock + " while inner EmitExceptionBlock " + CurrentExceptionBlock.Peek() + " is still open", Stack);
            }

            if (!Stack.IsRoot)
            {
                throw new SigilException("Stack should be empty when BeginFinallyBlock is called", Stack);
            }

            if (FinallyBlocks.Any(kv => kv.Key.ExceptionBlock == forTry))
            {
                throw new SigilException("There can only be one finally block per EmitExceptionBlock, and one is already defined for " + forTry, Stack);
            }

            var ret = new EmitFinallyBlock(this, forTry);

            IL.BeginFinallyBlock();

            FinallyBlocks[ret] = Tuple.Create(IL.Index, -1);

            return ret;
        }

        public void EndFinallyBlock(EmitFinallyBlock forFinally)
        {
            if (forFinally == null)
            {
                throw new ArgumentNullException("forFinally");
            }

            if (forFinally.Owner != this)
            {
                throw new ArgumentException("forFinally is not owned by this Emit, and thus cannot be used");
            }

            var finallyBlock = FinallyBlocks[forFinally];

            if (finallyBlock.Item2 != -1)
            {
                throw new SigilException("EndFinallyBlock expects an unclosed finally block, but " + forFinally + " is already closed", Stack);
            }

            if (!Stack.IsRoot)
            {
                throw new SigilException("Stack should be empty when EndFinallyBlock is called", Stack);
            }

            // There's no equivalent to EndFinallyBlock in raw ILGenerator, so no call here.
            //   But that's kind of weird from a just-in-time validation standpoint.

            UpdateState(OpCodes.Endfinally);

            FinallyBlocks[forFinally] = Tuple.Create(finallyBlock.Item1, IL.Index);
        }
    }
}
