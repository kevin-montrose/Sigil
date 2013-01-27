using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    [Serializable]
    public class SigilException : Exception, ISerializable
    {
        private StackState Stack;
        private StackState SecondStack;
        
        internal SigilException(string message) : base(message) { }

        internal SigilException(string message, StackState stack) : base(message)
        {
            Stack = stack;
        }

        internal SigilException(string message, StackState atBranch, StackState atLabel)
            : base(message)
        {
            Stack = atBranch;
            SecondStack = atLabel;
        }

        public string PrintStacks()
        {
            var sb = new StringBuilder();

            if (Stack == null)
            {
                return "";
            }

            if (SecondStack == null)
            {
                sb.AppendLine("Top of stack");
                sb.AppendLine("------------");
            }
            else
            {
                sb.AppendLine("Top of stack at branch");
                sb.AppendLine("----------------------");
            }

            EmitStack(sb, Stack);

            if (SecondStack != null)
            {
                sb.AppendLine();
                sb.AppendLine("Top of stack at label");
                sb.AppendLine("---------------------");

                EmitStack(sb, SecondStack);
            }

            return sb.ToString();
        }

        private static void EmitStack(StringBuilder sb, StackState stack)
        {
            if (stack.IsRoot)
            {
                sb.AppendLine("!!EMPTY!!");
            }

            while (!stack.IsRoot)
            {
                sb.AppendLine(stack.Value.ToString());
                stack = stack.Pop();
            }
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new System.ArgumentNullException("info");
            }

            base.GetObjectData(info, context);
        }
    }
}
