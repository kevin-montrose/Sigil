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
    public class Constructor
    {
        [TestMethod]
        public void Parameterless()
        {
            var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
            var mod = asm.DefineDynamicModule("Bar");
            var t = mod.DefineType("T");

            var foo = t.DefineField("Foo", typeof(int), FieldAttributes.Public);

            var c = Emit<Action>.BuildConstructor(t, MethodAttributes.Public, CallingConventions.HasThis);
            c.LoadArgument(0);
            c.LoadConstant(123);
            c.StoreField(foo);
            c.Return();

            c.CreateConstructor();

            var type = t.CreateType();

            var fooGet = type.GetField("Foo");

            var inst = type.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);

            Assert.AreEqual(123, (int)fooGet.GetValue(inst));
        }

        [TestMethod]
        public void TwoParameters()
        {
            var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
            var mod = asm.DefineDynamicModule("Bar");
            var t = mod.DefineType("T");

            var foo = t.DefineField("Foo", typeof(double), FieldAttributes.Public);

            var c = Emit<Action<double, double>>.BuildConstructor(t, MethodAttributes.Public, CallingConventions.HasThis);
            c.LoadArgument(0);
            c.LoadArgument(1);
            c.LoadArgument(2);
            c.Divide();
            c.StoreField(foo);
            c.Return();

            c.CreateConstructor();

            var type = t.CreateType();

            var fooGet = type.GetField("Foo");

            var inst = type.GetConstructor(new [] { typeof(double), typeof(double) }).Invoke(new object[] { 15.0, 7.0 });

            Assert.AreEqual(15.0 / 7.0, (double)fooGet.GetValue(inst));
        }
    }
}
