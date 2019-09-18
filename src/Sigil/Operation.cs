using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Sigil
{
    /// <summary>
    /// Represents a call to an Emit, used when providing introspection details about the generated IL stream.
    /// </summary>
    public class Operation<DelegateType>
    {
        /// <summary>
        /// The OpCode that corresponds to an Emit call.
        /// Note that the opcode may not correspond to the final short forms and other optimizations.
        /// </summary>
        public OpCode OpCode { get; internal set; }

        /// <summary>
        /// The parameters passsed to a call to Emit.
        /// </summary>
        public IEnumerable<object> Parameters { get; internal set; }

        /// <summary>
        /// This operation marks the beginning of an exception block,
        /// which is analogous to a call to Emit.BeginExceptionBlock.
        /// </summary>
        public bool IsExceptionBlockStart { get; internal set; }

        /// <summary>
        /// This operation marks the end of an exception block,
        /// which is analogous to a call to Emit.EndExceptionBlock.
        /// </summary>
        public bool IsExceptionBlockEnd { get; internal set; }

        /// <summary>
        /// This operation marks the beginning of a catch block,
        /// which is analogous to a call to Emit.BeginCatchBlock.
        /// </summary>
        public bool IsCatchBlockStart { get; internal set; }

        /// <summary>
        /// This operation marks the end of a catch block,
        /// which is analogous to a call to Emit.EndCatchBlock.
        /// </summary>
        public bool IsCatchBlockEnd { get; internal set; }

        /// <summary>
        /// This operation marks the beginning of a finally block,
        /// which is analogous to a call to Emit.BeginFinallyBlock.
        /// </summary>
        public bool IsFinallyBlockStart { get; internal set; }

        /// <summary>
        /// This operation marks the end of a finally block,
        /// which is analogous to a call to Emit.EndFinallyBlock.
        /// </summary>
        public bool IsFinallyBlockEnd { get; internal set; }

        /// <summary>
        /// This operation marks a label, the name of the label is given in LabelName.
        /// </summary>
        public bool IsMarkLabel { get; internal set; }

        /// <summary>
        /// If this operation marks a label, which is indicated by IsMarkLabel, then this property
        /// returns the name of the label being marked.
        /// </summary>
        public string LabelName { get; internal set; }

        /// <summary>
        /// Returns true if this operation is emitted a CIL opcode.
        /// </summary>
        public bool IsOpCode
        {
            get
            {
                return
                    !IsExceptionBlockStart &&
                    !IsExceptionBlockEnd &&
                    !IsCatchBlockStart &&
                    !IsCatchBlockEnd &&
                    !IsFinallyBlockStart &&
                    !IsFinallyBlockEnd &&
                    !IsMarkLabel &&
                    !IsIgnored;
            }
        }

        internal Action<Emit<DelegateType>> Replay { get; set; }

        internal bool IsIgnored { get; set; }

        internal PrefixTracker Prefixes { get; set; }

        /// <summary>
        /// A string representation of this Operation.
        /// </summary>
        public override string ToString()
        {
            if (IsExceptionBlockStart)
            {
                return "--Start Exception Block--";
            }

            if (IsExceptionBlockEnd)
            {
                return "--End Exception Block--";
            }

            if (IsCatchBlockStart)
            {
                return "--Start Catch Block--";
            }

            if (IsCatchBlockEnd)
            {
                return "--End Catch Block--";
            }

            if (IsFinallyBlockStart)
            {
                return "--Start Finally Block--";
            }

            if (IsFinallyBlockEnd)
            {
                return "--End Finally Block--";
            }

            if (IsMarkLabel)
            {
                return "--Mark Label " + LabelName + "--";
            }

            var ps =
                string.Join(
                    ", ",
                    LinqAlternative.Select(
                        Parameters,
                        o =>
                        {
                            if (o == null) return "(null)";

                            return o.ToString();
                        }
                    ).ToArray()
                );

            if (ps.Length == 0)
            {
                return OpCode.ToString();
            }

            return OpCode + " " + ps;
        }

        internal void Apply(Emit<DelegateType> emit)
        {
            if (Replay == null)
            {
                throw new InvalidOperationException("Cannot apply an Operation that didn't come from Disassembler");
            }

            Replay(emit);
        }
    }
}
