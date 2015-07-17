using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests 
{
    [TestClass, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class TypeInitializer 
    {
        [TestMethod]
        public void Simple() 
        {
            var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
            var mod = asm.DefineDynamicModule("Bar");
            var t = mod.DefineType("T");

            var foo = t.DefineField("Foo", typeof(int), FieldAttributes.Public | FieldAttributes.Static);

            var c = Emit<Action>.BuildTypeInitializer(t);
            c.LoadConstant(123);
            c.StoreField(foo);
            c.Return();

            c.CreateTypeInitializer();

            var type = t.CreateType();

            var fooGet = type.GetField("Foo");

            Assert.AreEqual(123, (int)fooGet.GetValue(null));
        }
    }
}
