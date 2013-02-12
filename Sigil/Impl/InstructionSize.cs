using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Sigil.Impl
{
    internal static class InstructionSize
    {
        public static int Get(OpCode op)
        {
            var baseSize = op.Size;
            int operandSize;

            switch (op.OperandType)
            {
                case OperandType.InlineBrTarget: operandSize = 4; break;
                case OperandType.InlineField: operandSize = 4; break;
                case OperandType.InlineI: operandSize = 4; break;
                case OperandType.InlineI8: operandSize = 8; break;
                case OperandType.InlineMethod: operandSize = 4; break;
                case OperandType.InlineNone: operandSize = 0; break;
                case OperandType.InlineR: operandSize = 8; break;
                case OperandType.InlineSig: operandSize = 4; break;
                case OperandType.InlineString: operandSize = 4; break;
                case OperandType.InlineSwitch: operandSize = 4; break;
                case OperandType.InlineTok: operandSize = 4; break;
                case OperandType.InlineType: operandSize = 4; break;
                case OperandType.InlineVar: operandSize = 2; break;
                case OperandType.ShortInlineBrTarget: operandSize = 1; break;
                case OperandType.ShortInlineI: operandSize = 1; break;
                case OperandType.ShortInlineR: operandSize = 4; break;
                case OperandType.ShortInlineVar: operandSize = 1; break;
                default: throw new Exception("Unexpected operand type [" + op.OperandType + "]");
            }

            return baseSize + operandSize;
        }

        public static int BeginCatchBlock() { return Get(OpCodes.Leave); }
        public static int EndCatchBlock() { return Get(OpCodes.Leave); }

        public static int BeginExceptionBlock() { return 0; }
        public static int EndExceptionBlock() { return 0; }

        public static int BeginFinallyBlock() { return Get(OpCodes.Leave); }
        public static int EndFinallyBlock() { return Get(OpCodes.Endfinally); }

        public static int DeclareLocal() { return 0; }
        public static int DefineLabel() { return 0; }
        public static int MarkLabel() { return 0; }
    }
}
