using Sigil.Impl;
using System;

namespace Sigil
{
    /// <summary>
    /// <para>Represents a catch block which appears in an ExceptionBlock.</para>
    /// <para>To create a CatchBlock, call BeginCatchBlock(Type) or BeginCatchAllBlock().</para>
    /// </summary>
    public class CatchBlock : IOwned
    {
        /// <summary>
        /// The ExceptionBlock this CatchBlock appears in.
        /// </summary>
        public ExceptionBlock ExceptionBlock { get; private set; }

        /// <summary>
        /// <para>Returns true if this CatchBlock will catch all exceptions.</para>
        /// <para>This is equivalent to `catch(Exception e)` in C#.</para>
        /// </summary>
        public bool IsCatchAll { get { return ExceptionType == typeof(Exception); } }

        /// <summary>
        /// <para>The type of exception being caught by this CatchBlock.</para>
        /// <para>
        /// When the CatchBlock is entered, an exception of this type will
        /// be pushed onto the stack.
        /// </para>
        /// </summary>
        public Type ExceptionType { get; private set; }

        object IOwned.Owner { get { return ((IOwned)ExceptionBlock).Owner; } }

        internal CatchBlock(Type exceptionType, ExceptionBlock forTry)
        {
            ExceptionType = exceptionType;
            ExceptionBlock = forTry;
        }
    }
}
