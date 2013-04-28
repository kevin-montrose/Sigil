using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Sigil
{
    /// <summary>
    /// Represents a call to an Emit, used when providing introspection details about the generated IL stream.
    /// </summary>
    public class Operation
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

        internal Action<dynamic> Replay { get; set; }

        /// <summary>
        /// A string representation of this Operation.
        /// </summary>
        public override string ToString()
        {
            var ps = 
                string.Join(
                    ", ", 
                    LinqAlternative.Select(
                        Parameters, 
                        o => 
                        {
                            if(o == null) return "(null)";
                            
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

        internal void Apply<DelegateType>(Emit<DelegateType> emit)
        {
            if (Replay == null)
            {
                throw new InvalidOperationException("Cannot apply an Operation that didn't come from Disassembler");
            }

            Replay(emit);
        }
    }
}
