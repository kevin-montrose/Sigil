using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;

namespace Sigil
{
    /// <summary>
    /// Represents a catch block which appears in an ExceptionBlock.
    /// 
    /// To create a CatchBlock, call BeginCatchBlock(Type) or BeginCatchAllBlock().
    /// </summary>
    public class CatchBlock
    {
        /// <summary>
        /// The ExceptionBlock this CatchBlock appears in.
        /// </summary>
        public ExceptionBlock ExceptionBlock { get; private set; }

        /// <summary>
        /// Returns true if this CatchBlock will catch all exceptions.
        /// 
        /// This is equivalent to `catch(Exception e)` in C#.
        /// </summary>
        public bool IsCatchAll { get { return ExceptionType == typeof(Exception); } }

        /// <summary>
        /// The type of exception being caught by this CatchBlock.
        /// 
        /// When the CatchBlock is entered, an exception of this type will
        /// be pushed onto the stack.
        /// </summary>
        public Type ExceptionType { get; private set; }

        internal object Owner { get; private set; }

        internal CatchBlock(object owner, Type exceptionType, ExceptionBlock forTry)
        {
            Owner = owner;
            ExceptionType = exceptionType;
            ExceptionBlock = forTry;
        }
    }
}
