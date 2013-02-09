using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    /// <summary>
    /// Represents a finally block, which appears within an ExceptionBlock.
    /// 
    /// This is roughly analogous to `finally` in C#.
    /// </summary>
    public class FinallyBlock : IOwned
    {
        /// <summary>
        /// The ExceptionBlock this FinallyBlock appears as part of.
        /// </summary>
        public ExceptionBlock ExceptionBlock { get; private set; }

        object IOwned.Owner { get { return ((IOwned)ExceptionBlock).Owner; } }

        internal FinallyBlock(ExceptionBlock forTry)
        {
            ExceptionBlock = forTry;
        }
    }
}
