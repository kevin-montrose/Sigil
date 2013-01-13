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
        private StackState SecondStack;

        internal SigilException(string message, StackState stack) : base(message)
        {
            Stack = stack;
        }

        internal SigilException(string message, StackState atBranch, StackState atLabel)
            : base(message)
        {
            Stack = atBranch;
            SecondStack = atLabel;
        }

        public string PrintStacks()
        {
            throw new NotImplementedException();
        }
    }
}
