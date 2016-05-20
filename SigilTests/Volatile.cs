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
    [TestClass]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Volatile
    {
        class SimpleClass
        {
#pragma warning disable CS0649
            public volatile int A;
#pragma warning restore CS0649
        }

        [TestMethod]
        public void Simple()
        {
            var e1 = Emit<Func<SimpleClass, int>>.NewDynamicMethod("e1");
            e1.LoadArgument(0);
            e1.LoadField(typeof(SimpleClass).GetField("A"));
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate(out instrs);

            Assert.AreEqual(1, d1(new SimpleClass { A = 1 }));
            Assert.IsTrue(instrs.Contains("volatile."));
        }

        class NoneClass
        {
            public int A;
        }

        [TestMethod]
        public void None()
        {
            var e1 = Emit<Func<NoneClass, int>>.NewDynamicMethod("e1");
            e1.LoadArgument(0);
            e1.LoadField(typeof(NoneClass).GetField("A"));
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate(out instrs);

            Assert.AreEqual(1, d1(new NoneClass { A = 1 }));
            Assert.IsFalse(instrs.Contains("volatile."));
        }

        [TestMethod]
        public void Builder()
        {
            var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
            var mod = asm.DefineDynamicModule("Bar");
            var t = mod.DefineType("T");

            var sField = t.DefineField("A", typeof(int), new[] { typeof(System.Runtime.CompilerServices.IsVolatile) }, Type.EmptyTypes, FieldAttributes.Public | FieldAttributes.Static);

            var e1 = Emit<Func<int>>.BuildStaticMethod(t, "Static", MethodAttributes.Public);

            e1.LoadField(sField, isVolatile: true);
            e1.Return();

            string instrs;
            e1.CreateMethod(out instrs);

            var type = t.CreateType();
            var mtd = type.GetMethod("Static");

            var f = type.GetField("A");
            f.SetValue(null, 123);

            Assert.AreEqual(123, (int)mtd.Invoke(null, new object[0]));
            Assert.IsTrue(instrs.Contains("volatile."));
        }
    }
}
