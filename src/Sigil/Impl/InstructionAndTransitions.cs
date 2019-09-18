using System.Reflection.Emit;

namespace Sigil.Impl
{
    internal class InstructionAndTransitions
    {
        public LinqList<StackTransition> Transitions { get; private set; }
        public OpCode? Instruction { get; private set; }
        public int? InstructionIndex { get; private set; }

        public InstructionAndTransitions(OpCode? instr, int? ix, LinqList<StackTransition> trans)
        {
            Instruction = instr;
            Transitions = trans;
            InstructionIndex = ix;
        }

        public override string ToString()
        {
            if (!Instruction.HasValue) return string.Join(", ", Transitions.Select(t => t.ToString()).ToArray());

            return "[" + Instruction + " @" + InstructionIndex + "] " + string.Join(", ", Transitions.Select(t => t.ToString()).ToArray());
        }
    }
}
