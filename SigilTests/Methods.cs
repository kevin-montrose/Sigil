using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestClass, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Methods
    {
        [TestMethod]
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
            Assert.AreEqual("123", res);
        }

        [TestMethod]
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
            Assert.AreEqual("124", res);
        }
    }
}
