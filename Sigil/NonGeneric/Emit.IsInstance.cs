using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Pops a value from the stack and casts to the given type if possible pushing the result, otherwise pushes a null.
        /// 
        /// This is analogous to C#'s `as` operator.
        /// </summary>
        public Emit IsInstance<Type>()
        {
            InnerEmit.IsInstance<Type>();
            return this;
        }

        /// <summary>
        /// Pops a value from the stack and casts to the given type if possible pushing the result, otherwise pushes a null.
        /// 
        /// This is analogous to C#'s `as` operator.
        /// </summary>
        public Emit IsInstance(Type type)
        {
            InnerEmit.IsInstance(type);
            return this;
        }
    }
}
