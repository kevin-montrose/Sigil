#if !COREFX // see https://github.com/dotnet/corefx/issues/4543 item 4
using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Converts a TypedReference on the stack into a RuntimeTypeHandle for the type contained with it.
        /// 
        /// __makeref(int) on the stack would become the RuntimeTypeHandle for typeof(int), for example.
        /// </summary>
        public Emit<DelegateType> ReferenceAnyType()
        {
            var transitions =
                new[] {
                    new StackTransition(new[] { typeof(TypedReference) }, new [] { typeof(RuntimeTypeHandle) })
                };

            UpdateState(OpCodes.Refanytype, Wrap(transitions, "ReferenceAnyType"));

            return this;
        }
    }
}
#endif