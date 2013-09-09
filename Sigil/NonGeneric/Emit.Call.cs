using System;
using System.Reflection;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Calls the given method.  Pops its arguments in reverse order (left-most deepest in the stack), and pushes the return value if it is non-void.
        /// 
        /// If the given method is an instance method, the `this` reference should appear before any parameters.
        /// 
        /// Call does not respect overrides, the implementation defined by the given MethodInfo is what will be called at runtime.
        /// 
        /// To call overrides of instance methods, use CallVirtual.
        /// 
        /// When calling VarArgs methods, arglist should be set to the types of the extra parameters to be passed.
        /// </summary>
        public Emit Call(MethodInfo method, Type[] arglist = null)
        {
            InnerEmit.Call(method, arglist);
            return this;
        }
    }
}
