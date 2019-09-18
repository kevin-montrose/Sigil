using Sigil;
using System;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace SigilTests
{
    public partial class TypeInitializer
    {
        [Fact]
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

            Assert.Equal(123, (int)fooGet.GetValue(null));
        }
    }
}
