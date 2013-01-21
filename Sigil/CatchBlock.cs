using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;

namespace Sigil
{
    public class CatchBlock
    {
        public EmitExceptionBlock ExceptionBlock { get; private set; }
        public bool IsCatchAll { get { return ExceptionType == typeof(Exception); } }

        public Type ExceptionType { get; private set; }

        internal object Owner { get; private set; }

        internal CatchBlock(object owner, Type exceptionType, EmitExceptionBlock forTry)
        {
            Owner = owner;
            ExceptionType = exceptionType;
            ExceptionBlock = forTry;
        }
    }
}
