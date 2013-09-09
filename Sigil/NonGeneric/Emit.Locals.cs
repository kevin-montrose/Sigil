using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Declare a new local of the given type in the current method.
        /// 
        /// Name is optional, and only provided for debugging purposes.  It has no
        /// effect on emitted IL.
        /// 
        /// Be aware that each local takes some space on the stack, inefficient use of locals
        /// could lead to StackOverflowExceptions at runtime.
        /// </summary>
        public Local DeclareLocal<Type>(string name = null)
        {
            return InnerEmit.DeclareLocal<Type>(name);
        }

        /// <summary>
        /// Declare a new local of the given type in the current method.
        /// 
        /// Name is optional, and only provided for debugging purposes.  It has no
        /// effect on emitted IL.
        /// 
        /// Be aware that each local takes some space on the stack, inefficient use of locals
        /// could lead to StackOverflowExceptions at runtime.
        /// </summary>
        public Local DeclareLocal(Type type, string name = null)
        {
            return InnerEmit.DeclareLocal(type, name);
        }

        /// <summary>
        /// Declare a new local of the given type in the current method.
        /// 
        /// Name is optional, and only provided for debugging purposes.  It has no
        /// effect on emitted IL.
        /// 
        /// Be aware that each local takes some space on the stack, inefficient use of locals
        /// could lead to StackOverflowExceptions at runtime.
        /// </summary>
        public Emit DeclareLocal<Type>(out Local local, string name = null)
        {
            InnerEmit.DeclareLocal<Type>(out local, name);
            return this;
        }

        /// <summary>
        /// Declare a new local of the given type in the current method.
        /// 
        /// Name is optional, and only provided for debugging purposes.  It has no
        /// effect on emitted IL.
        /// 
        /// Be aware that each local takes some space on the stack, inefficient use of locals
        /// could lead to StackOverflowExceptions at runtime.
        /// </summary>
        public Emit DeclareLocal(Type type, out Local local, string name = null)
        {
            InnerEmit.DeclareLocal(type, out local, name);
            return this;
        }
    }
}
