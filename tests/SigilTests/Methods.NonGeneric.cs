using Sigil.NonGeneric;
using System;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace SigilTests
{
    public partial class Methods
    {
        [Fact]
        public void StaticNonGeneric()
        {
            var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
            var mod = asm.DefineDynamicModule("Bar");
            var t = mod.DefineType("T");

            var e1 = Emit.BuildStaticMethod(typeof(string), new [] { typeof(int) }, t, "Static", MethodAttributes.Public);
            e1.LoadArgument(0);
            e1.Box<int>();
            e1.CallVirtual(typeof(object).GetMethod("ToString", Type.EmptyTypes));
            e1.Return();

            e1.CreateMethod();

            var type = t.CreateType();
            var del = type.GetMethod("Static");

            var res = (string)del.Invoke(null, new object[] { 123 });
            Assert.Equal("123", res);
        }

        [Fact]
        public void InstanceNonGeneric()
        {
            var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
            var mod = asm.DefineDynamicModule("Bar");
            var t = mod.DefineType("T");

            var e1 = Emit.BuildInstanceMethod(typeof(string), new [] { typeof(int) }, t, "Instance", MethodAttributes.Public);
            e1.LoadArgument(1);
            e1.LoadConstant(1);
            e1.Add();
            e1.Box<int>();
            e1.CallVirtual(typeof(object).GetMethod("ToString", Type.EmptyTypes));
            e1.Return();

            e1.CreateMethod();

            var type = t.CreateType();
            var inst = type.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
            var del = type.GetMethod("Instance");

            var res = (string)del.Invoke(inst, new object[] { 123 });
            Assert.Equal("124", res);
        }
    }
}
