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
    /// A SigilVerificationException is thrown whenever a CIL stream becomes invalid.
    /// 
    /// There are many possible causes of this including: operator type mismatches, underflowing the stack, and branching from one stack state to another.
    /// 
    /// Invalid arguments, non-sensical parameters, and other non-correctness related errors will throw more specific exceptions.
    /// 
    /// SigilVerificationException will typically include the state of the stack (or stacks) at the instruction in error.
    /// </summary>
    [Serializable]
    public class SigilVerificationException : Exception, ISerializable
    {
        private StackState Stack;
        private StackState SecondStack;
        private string[] Instructions;

        private int? BranchLoc;
        private int? LabelLoc;

        internal SigilVerificationException(string message, BufferedILGenerator il) : base(message)
        {
            Instructions = il.Instructions();
        }

        internal SigilVerificationException(string message,BufferedILGenerator il, StackState stack) : this(message, il)
        {
            Stack = stack;
        }

        internal SigilVerificationException(string message, BufferedILGenerator il, StackState atBranch, int branchLoc, StackState atLabel, int labelLoc)
            : this(message, il, atBranch)
        {
            SecondStack = atLabel;

            BranchLoc = branchLoc;
            LabelLoc = labelLoc;
        }

        /// <summary>
        /// Returns a string representation of any stacks attached to this exception.
        /// 
        /// This is meant for debugging purposes, and should not be called during normal operation.
        /// </summary>
        public string GetDebugInfo()
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

            if (Instructions.Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Instruction stream");
                sb.AppendLine("------------------");

                if (BranchLoc.HasValue && LabelLoc.HasValue)
                {
                    sb.AppendLine(AddBranchAndLabelMarkers());
                }
                else
                {
                    foreach (var line in Instructions)
                    {
                        sb.AppendLine(line);
                    }
                }
            }

            return sb.ToString();
        }

        private string AddBranchAndLabelMarkers()
        {
            var ret = new StringBuilder();

            for (var i = 0; i < Instructions.Length; i++)
            {
                var line = Instructions[i];

                if (i == LabelLoc - 1) line = line + " // Failure label";
                if (i == BranchLoc - 1) line = line + " // Failure branch";

                ret.AppendLine(line);
            }

            return ret.ToString().Trim();
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
                Message + Environment.NewLine + Environment.NewLine + GetDebugInfo();
        }
    }
}
