using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pops a reference to a rank 1 array off the stack, and pushes it's length onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadLength()
        {
            var onStack = Stack.Top();

            if (onStack == null)
            {
                throw new SigilVerificationException("LoadLength expects a value on the stack, but it was empty", IL, Stack);
            }

            var arr = onStack[0];

            if (arr.IsReference || arr.IsPointer || !arr.Type.IsArray || arr.Type.GetArrayRank() != 1)
            {
                throw new SigilVerificationException("LoadLength expects a rank 1 array, found " + arr, IL, Stack);
            }

            UpdateState(OpCodes.Ldlen, TypeOnStack.Get<int>(), pop: 1);

            return this;
        }
    }
}
