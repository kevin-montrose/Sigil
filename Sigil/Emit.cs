using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        private ILGenerator IL;
        private Type ReturnType;
        private Type[] ParameterTypes;
        private DynamicMethod DynMethod;

        private StackState Stack;

        private Emit(DynamicMethod dynMethod)
        {
            DynMethod = dynMethod;

            IL = DynMethod.GetILGenerator();

            ReturnType = DynMethod.ReturnType;
            ParameterTypes = DynMethod.GetParameters().Select(p => p.ParameterType).ToArray();

            Stack = new StackState();
        }

        public DelegateType CreateDelegate()
        {
            return (DelegateType)(object)DynMethod.CreateDelegate(typeof(DelegateType));
        }

        public static Emit<DelegateType> NewDynamicMethod(string name)
        {
            var delType = typeof(DelegateType);

            var baseTypes = new HashSet<Type>();
            baseTypes.Add(delType);
            var bType = delType.BaseType;
            while (bType != null)
            {
                baseTypes.Add(bType);
                bType = bType.BaseType;
            }

            if (!baseTypes.Contains(typeof(Delegate)))
            {
                throw new ArgumentException("DelegateType must be a delegate, found " + delType.FullName);
            }

            var invoke = delType.GetMethod("Invoke");
            var returnType = invoke.ReturnType;
            var parameterTypes = invoke.GetParameters().Select(s => s.ParameterType).ToArray();

            var dynMethod = new DynamicMethod(name, returnType, parameterTypes);

            return new Emit<DelegateType>(dynMethod);
        }
    }
}
