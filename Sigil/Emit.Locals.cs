using Sigil.Impl;
using System;

namespace Sigil
{
    public partial class Emit<DelegateType>
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
            return DeclareLocal(typeof(Type), name);
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
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            name = name ?? AutoNamer.Next(this, "_local");

            var local = IL.DeclareLocal(type);

            var localIndex = NextLocalIndex;
            NextLocalIndex++;

            var ret = new Local(this, localIndex, type, local, name);

            UnusedLocals.Add(ret);

            Locals[localIndex] = ret;

            return ret;
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
        public Emit<DelegateType> DeclareLocal<Type>(out Local local, string name = null)
        {
            return DeclareLocal(typeof(Type), out local, name);
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
        public Emit<DelegateType> DeclareLocal(Type type, out Local local, string name = null)
        {
            local = DeclareLocal(type, name);

            return this;
        }
    }
}
