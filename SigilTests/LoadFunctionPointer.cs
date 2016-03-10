using System;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;

namespace SigilTests
{
    [TestClass]
    public partial class LoadFunctionPointer
    {
        [TestMethod]
        public void CanLoadUnbaked()
        {
            var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("MethodBuilders"), AssemblyBuilderAccess.Run);
            var mod = asm.DefineDynamicModule("Mod");
            var tb = mod.DefineType("Type");

            var mb = Emit<Action>.BuildInstanceMethod(tb, "UnbakedFunction", MethodAttributes.Public);
            mb.Return();
            var meth = mb.CreateMethod();

            var createProxy = Emit<Func<object>>.BuildStaticMethod(tb, "Create", MethodAttributes.Public | MethodAttributes.Static);
            createProxy.LoadFunctionPointer(meth, new Type[0]);
            createProxy.Return();

            createProxy.CreateMethod();//will throw on failure
        }
    }
}
