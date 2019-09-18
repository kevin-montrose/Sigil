using System;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Declare a new local of the given type in the current method.</para>
        /// <para>
        /// Name is optional, and only provided for debugging purposes.  It has no
        /// effect on emitted IL.
        /// </para>
        /// <para>
        /// Be aware that each local takes some space on the stack, inefficient use of locals
        /// could lead to StackOverflowExceptions at runtime.
        /// </para>
        /// </summary>
        public Local DeclareLocal<Type>(string name = null)
        {
            return InnerEmit.DeclareLocal<Type>(name);
        }

        /// <summary>
        /// <para>Declare a new local of the given type in the current method.</para>
        /// <para>
        /// Name is optional, and only provided for debugging purposes.  It has no
        /// effect on emitted IL.
        /// </para>
        /// <para>
        /// Be aware that each local takes some space on the stack, inefficient use of locals
        /// could lead to StackOverflowExceptions at runtime.
        /// </para>
        /// </summary>
        public Local DeclareLocal(Type type, string name = null)
        {
            return InnerEmit.DeclareLocal(type, name);
        }

        /// <summary>
        /// <para>Declare a new local of the given type in the current method.</para>
        /// <para>
        /// Name is optional, and only provided for debugging purposes.  It has no
        /// effect on emitted IL.
        /// </para>
        /// <para>
        /// Be aware that each local takes some space on the stack, inefficient use of locals
        /// could lead to StackOverflowExceptions at runtime.
        /// </para>
        /// </summary>
        public Emit DeclareLocal<Type>(out Local local, string name = null)
        {
            InnerEmit.DeclareLocal<Type>(out local, name);
            return this;
        }

        /// <summary>
        /// <para>Declare a new local of the given type in the current method.</para>
        /// <para>
        /// Name is optional, and only provided for debugging purposes.  It has no
        /// effect on emitted IL.
        /// </para>
        /// <para>
        /// Be aware that each local takes some space on the stack, inefficient use of locals
        /// could lead to StackOverflowExceptions at runtime.
        /// </para>
        /// </summary>
        public Emit DeclareLocal(Type type, out Local local, string name = null)
        {
            InnerEmit.DeclareLocal(type, out local, name);
            return this;
        }
    }
}
