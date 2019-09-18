using Sigil.NonGeneric;
using System;
using System.Reflection.Emit;
using System.Reflection;
using Xunit;

namespace SigilTests
{
    public partial class LoadVirtualFunctionPointer
    {
        [Fact]
        public void CanValidateUnbakedNonGeneric()
        {
            var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("MethodBuilders"), AssemblyBuilderAccess.Run);
            var mod = asm.DefineDynamicModule("Mod");
            var tb = mod.DefineType("Type");

            var cb = Emit.BuildConstructor(new Type[0], tb, MethodAttributes.Public);
            cb.Return();
            var cons = cb.CreateConstructor();

            var targetMethodBuilder = Emit.BuildInstanceMethod(typeof(void), new Type[0], tb, "UnbakedFunction", MethodAttributes.Public);
            targetMethodBuilder.Return();
            var targetMethod = targetMethodBuilder.CreateMethod();

            var testMethod = Emit.BuildStaticMethod(typeof(void), new Type[0], tb, "Create", MethodAttributes.Public | MethodAttributes.Static);

            testMethod.NewObject(cons, new Type[0]);

            testMethod.LoadVirtualFunctionPointer(targetMethod, new Type[0]);
            testMethod.Pop();
            testMethod.Return();

            testMethod.CreateMethod();//will throw on failure
        }
    }
}