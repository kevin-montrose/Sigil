using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Pushes a pointer to the given function onto the stack, as a native int.</para>
        /// <para>To resolve a method at runtime using an object, use LoadVirtualFunctionPointer instead.</para>
        /// </summary>
        public Emit LoadFunctionPointer(MethodInfo method)
        {
            InnerEmit.LoadFunctionPointer(method);
            return this;
        }

        /// <summary>
        /// <para>Pushes a pointer to the given function onto the stack, as a native int.</para>
        /// <para>To resolve a method at runtime using an object, use LoadVirtualFunctionPointer instead.</para>
        /// <para>
        /// This method is provided as MethodBuilder cannot be inspected for parameter information at runtime.  If the passed parameterTypes
        /// do not match the given method, the produced code will be invalid.
        /// </para>
        /// </summary>
        public Emit LoadFunctionPointer(MethodBuilder method, Type[] parameterTypes)
        {
            InnerEmit.LoadFunctionPointer(method, parameterTypes);
            return this;
        }
    }
}
