using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// <para>Converts a TypedReference on the stack into a reference to the contained object, given the type contained in the TypedReference.</para>
        /// <para>__makeref(int) on the stack would become an int&amp;, for example.</para>
        /// </summary>
        public Emit<DelegateType> ReferenceAnyValue<Type>()
        {
            return ReferenceAnyValue(typeof(Type));
        }

        /// <summary>
        /// <para>Converts a TypedReference on the stack into a reference to the contained object, given the type contained in the TypedReference.</para>
        /// <para>__makeref(int) on the stack would become an int&amp;, for example.</para>
        /// </summary>
        public Emit<DelegateType> ReferenceAnyValue(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            var transitions =
                new[]
                {
                    new StackTransition(new[] { typeof(TypedReference) }, new[] { type.MakeByRefType() })
                };

            UpdateState(OpCodes.Refanyval, type, Wrap(transitions, "ReferenceAnyValue"));

            return this;
        }
    }
}