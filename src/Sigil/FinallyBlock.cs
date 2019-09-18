using Sigil.Impl;

namespace Sigil
{
    /// <summary>
    /// <para>Represents a finally block, which appears within an ExceptionBlock.</para>
    /// <para>This is roughly analogous to `finally` in C#.</para>
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
