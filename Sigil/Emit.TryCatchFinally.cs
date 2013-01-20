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
            var label = new EmitLabel(this, labelDel, Guid.NewGuid().ToString().Replace("-", ""));

            var ret = new EmitExceptionBlock(label);

            TryBlocks[ret] = Tuple.Create(IL.Index, -1);

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

            // Can't close an exception block while there are outstanding catch blocks
            foreach (var kv in CatchBlocks)
            {
                if (kv.Key.ExceptionBlock != forTry) continue;

                if (kv.Value.Item2 == -1)
                {
                    throw new SigilException("Cannot end EmitExceptionBlock, EmitCatchBlock " + kv.Key + " has not been ended", Stack);
                }
            }

            Sigil.Impl.BufferedILGenerator.UpdateOpCodeDelegate ignored;
            IL.Emit(OpCodes.Leave, forTry.Label.Label, out ignored);
            IL.EndExceptionBlock();

            TryBlocks[forTry] = Tuple.Create(location.Item1, IL.Index);

            Stack = new StackState();
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

            if (!Stack.IsRoot)
            {
                throw new SigilException("Stack should be empty when BeginCatchBlock is called", Stack);
            }

            if(!typeof(Exception).IsAssignableFrom(exceptionType))
            {
                throw new SigilException("BeginCatchBlock expects a type descending from Exception, found "+exceptionType, Stack);
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

            Sigil.Impl.BufferedILGenerator.UpdateOpCodeDelegate ignored;
            IL.Emit(OpCodes.Leave, forCatch.ExceptionBlock.Label.Label, out ignored);

            CatchBlocks[forCatch] = Tuple.Create(location.Item1, IL.Index);
        }
    }
}
