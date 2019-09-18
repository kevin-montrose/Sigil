using System;
using System.Reflection;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Calls the given method.  Pops its arguments in reverse order (left-most deepest in the stack), and pushes the return value if it is non-void.</para>
        /// <para>If the given method is an instance method, the `this` reference should appear before any parameters.</para>
        /// <para>Call does not respect overrides, the implementation defined by the given MethodInfo is what will be called at runtime.</para>
        /// <para>To call overrides of instance methods, use CallVirtual.</para>
        /// <para>When calling VarArgs methods, arglist should be set to the types of the extra parameters to be passed.</para>
        /// </summary>
        public Emit Call(MethodInfo method, Type[] arglist = null)
        {
            InnerEmit.Call(method, arglist);
            return this;
        }


        /// <summary>
        /// <para>Calls the given constructor.  Pops its arguments in reverse order (left-most deepest in the stack).</para>
        /// <para>The `this` reference should appear before any parameters.</para>
        /// </summary>
        public Emit Call(ConstructorInfo constructor)
        {
            InnerEmit.Call(constructor);
            return this;
        }

        /// <summary>
        /// <para>Calls the method being constructed by the given emit.  Emits so used must have been constructed with BuildMethod or related methods.</para>
        /// <para>Pops its arguments in reverse order (left-most deepest in the stack), and pushes the return value if it is non-void.</para>
        /// <para>If the given method is an instance method, the `this` reference should appear before any parameters.</para>
        /// <para>Call does not respect overrides, the implementation defined by the given MethodInfo is what will be called at runtime.</para>
        /// <para>
        /// To call overrides of instance methods, use CallVirtual.
        /// Recursive calls can only be performed with DynamicMethods, other passed in Emits must already have their methods created.
        /// When calling VarArgs methods, arglist should be set to the types of the extra parameters to be passed.
        /// </para>
        /// </summary>
        public Emit Call(Emit emit, Type[] arglist = null)
        {
            if (emit == null)
            {
                throw new ArgumentNullException("emit");
            }
            
            MethodInfo methodInfo = emit.InnerEmit.MtdBuilder ?? (MethodInfo)emit.InnerEmit.DynMethod;
            if (methodInfo == null)
            {
                var dynMethod = new System.Reflection.Emit.DynamicMethod(emit.Name, emit.ReturnType, emit.ParameterTypes, emit.Module, skipVisibility: true);

                emit.InnerEmit.DynMethod = dynMethod;
                methodInfo = dynMethod;
            }

            return Call(methodInfo, arglist);
        }
    }
}
