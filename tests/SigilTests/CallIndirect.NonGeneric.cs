using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class CallIndirect
    {
        [Fact]
        public void SimpleNonGeneric()
        {
            var foo = typeof(CallIndirect).GetMethod("Foo");

            var e1 = Emit.NewDynamicMethod(typeof(string), Type.EmptyTypes, "E1");
            e1.LoadConstant(3);
            e1.LoadFunctionPointer(foo);
            e1.CallIndirect<string, int>(foo.CallingConvention);
            e1.Return();

            var d1 = e1.CreateDelegate<Func<string>>();

            Assert.Equal("BarBarBar", d1());
        }

        [Fact]
        public void VirtualNonGeneric()
        {
            var toString = typeof(object).GetMethod("ToString");

            var e1 = Emit.NewDynamicMethod(typeof(string), Type.EmptyTypes, "E1");
            e1.NewObject<VirtualClass>();
            e1.Duplicate();
            e1.LoadVirtualFunctionPointer(toString);
            e1.CallIndirect<string>(toString.CallingConvention);
            e1.Return();

            var d1 = e1.CreateDelegate<Func<string>>();

            Assert.Equal("I'm Virtual!", d1());
        }
    }
}
