using Sigil.Impl;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Pushes a pointer to the given function onto the stack, as a native int.
        /// 
        /// To resolve a method at runtime using an object, use LoadVirtualFunctionPointer instead.
        /// </summary>
        public Emit LoadFunctionPointer(MethodInfo method)
        {
            InnerEmit.LoadFunctionPointer(method);
            return this;
        }
    }
}
