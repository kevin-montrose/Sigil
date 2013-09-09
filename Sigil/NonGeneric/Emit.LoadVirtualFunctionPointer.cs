using System.Reflection;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Pops an object reference off the stack, and pushes a pointer to the given method's implementation on that object.
        /// 
        /// For static or non-virtual functions, use LoadFunctionPointer
        /// </summary>
        public Emit LoadVirtualFunctionPointer(MethodInfo method)
        {
            InnerEmit.LoadVirtualFunctionPointer(method);
            return this;
        }
    }
}
