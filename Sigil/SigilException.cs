using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    public class SigilException : Exception
    {
        private StackState Stack;

        internal SigilException(string message, StackState stack) : base(message)
        {
            Stack = stack;
        }

        public string PrintStack()
        {
            throw new NotImplementedException();
        }
    }
}
