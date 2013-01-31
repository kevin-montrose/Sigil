using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    /// <summary>
    /// A SigilException is thrown whenever a CIL stream becomes invalid.
    /// 
    /// There are many possible causes of this including: operator type mismatches, underflowing the stack, and branching from one stack state to another.
    /// 
    /// Invalid arguments, non-sensical parameters, and other non-correctness related errors will throw more specific exceptions.
    /// 
    /// SigilExceptions will typically include the state of the stack (or stacks) at the instruction in error.
    /// </summary>
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

        /// <summary>
        /// Returns a string representation of any stacks attached to this exception.
        /// 
        /// This is meant for debugging purposes, and should not be called during normal operation.
        /// </summary>
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

        /// <summary>
        /// Implementation for ISerializable.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new System.ArgumentNullException("info");
            }

            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Returns the message and stacks on this exception, in string form.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return
                Message + "\r\n\r\n" + PrintStacks();
        }
    }
}
