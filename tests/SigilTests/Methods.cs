using Sigil;
using System;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace SigilTests
{
    public partial class Methods
    {
        [Fact]
        public void Static()
        {
            var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
            var mod = asm.DefineDynamicModule("Bar");
            var t = mod.DefineType("T");

            var e1 = Emit<Func<int, string>>.BuildStaticMethod(t, "Static", MethodAttributes.Public);
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
        public void Instance()
        {
            var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
            var mod = asm.DefineDynamicModule("Bar");
            var t = mod.DefineType("T");

            var e1 = Emit<Func<int, string>>.BuildInstanceMethod(t, "Instance", MethodAttributes.Public);
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

        [Fact]
        public void Recursive()
        {
            var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
            var mod = asm.DefineDynamicModule("Bar");
            var t = mod.DefineType("T");

            var e1 = Emit<Func<int, int>>.BuildStaticMethod(t, "Recursive", MethodAttributes.Public);
            var cont = e1.DefineLabel("continue");

            e1.LoadArgument(0);
            e1.LoadConstant(0);
            e1.UnsignedBranchIfNotEqual(cont);

            e1.LoadConstant(1);
            e1.Return();

            e1.MarkLabel(cont);
            e1.LoadArgument(0);
            e1.Duplicate();
            e1.LoadConstant(-1);
            e1.Add();
            e1.Call(e1);
            e1.Multiply();

            e1.Return();

            e1.CreateMethod(out string instrs);

            Assert.Equal("ldarg.0\r\nldc.i4.0\r\nbne.un.s continue\r\nldc.i4.1\r\nret\r\n\r\ncontinue:\r\nldarg.0\r\ndup\r\nldc.i4.m1\r\nadd\r\ncall Recursive\r\nmul\r\nret\r\n", instrs);

            var type = t.CreateType();
            var recur = type.GetMethod("Recursive", BindingFlags.Public | BindingFlags.Static);

            var zero = (int)recur.Invoke(null, new object[] { 0 });
            var one = (int)recur.Invoke(null, new object[] { 1 });
            var two = (int)recur.Invoke(null, new object[] { 2 });
            var three = (int)recur.Invoke(null, new object[] { 3 });
            var ten = (int)recur.Invoke(null, new object[] { 10 });

            Assert.Equal(1, zero);
            Assert.Equal(1 * 1, one);
            Assert.Equal(2 * 1 * 1, two);
            Assert.Equal(3 * 2 * 1 * 1, three);
            Assert.Equal(10 * 9 * 8 * 7 * 6 * 5 * 4 * 3 * 2 * 1 * 1, ten);
        }
    }
}
