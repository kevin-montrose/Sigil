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
    public class FinallyBlock
    {
        /// <summary>
        /// The ExceptionBlock this FinallyBlock appears as part of.
        /// </summary>
        public ExceptionBlock ExceptionBlock { get; private set; }

        internal object Owner { get; private set; }

        internal FinallyBlock(object owner, ExceptionBlock forTry)
        {
            ExceptionBlock = forTry;
            Owner = owner;
        }
    }
}
