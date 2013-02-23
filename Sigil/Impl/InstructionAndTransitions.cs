using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Sigil.Impl
{
    internal class InstructionAndTransitions
    {
        public IEnumerable<StackTransition> Transitions { get; private set; }
        public OpCode? Instruction { get; private set; }

        public InstructionAndTransitions(OpCode? instr, IEnumerable<StackTransition> trans)
        {
            Instruction = instr;
            Transitions = trans;
        }
    }
}
