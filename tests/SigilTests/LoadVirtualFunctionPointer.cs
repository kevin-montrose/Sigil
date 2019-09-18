using System;
using System.Reflection;
using System.Reflection.Emit;
using Sigil;
using Xunit;

namespace SigilTests
{
    public partial class LoadVirtualFunctionPointer
    {
        [Fact]
        public void CanValidateUnbaked()
        {
            var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("MethodBuilders"), AssemblyBuilderAccess.Run);
            var mod = asm.DefineDynamicModule("Mod");
            var tb = mod.DefineType("Type");

            var cb = Emit<Action>.BuildConstructor(tb, MethodAttributes.Public);
            cb.Return();
            var cons = cb.CreateConstructor();

            var mb = Emit<Action>.BuildInstanceMethod(tb, "UnbakedFunction", MethodAttributes.Public);
            mb.Return();
            var meth = mb.CreateMethod();

            var createProxy = Emit<Action>.BuildStaticMethod(tb, "Create", MethodAttributes.Public | MethodAttributes.Static);
            createProxy.NewObject(cons, new Type[0]);
            createProxy.LoadVirtualFunctionPointer(meth, new Type[0]);
            createProxy.Pop();//dump the useless value
            createProxy.Return();

            createProxy.CreateMethod();//will throw on failure
        }
    }
}
