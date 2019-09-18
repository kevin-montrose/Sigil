using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Boxing
    {
        [Fact]
        public void NullableIntNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(object), new [] { typeof(int?) }, "E1");
            e1.LoadArgument(0);
            e1.Box(typeof(int?));
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int?, object>>();

            Assert.Equal((object)((int?)123), d1(123));
            Assert.Equal((object)((int?)null), d1(null));
        }

        [Fact]
        public void BooleanNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(object), new [] { typeof(bool) }, "E1");
            e1.LoadArgument(0);
            e1.Box<bool>();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<bool, object>>();

            Assert.Equal((object)true, d1(true));
            Assert.Equal((object)false, d1(false));
        }

        [Fact]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(object), Type.EmptyTypes, "E1");
            e1.LoadConstant(123);
            e1.Box<byte>();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<object>>();

            Assert.Equal("123", d1().ToString());

            var e2 = Emit.NewDynamicMethod(typeof(object), Type.EmptyTypes, "E2");
            e2.LoadConstant(566);
            e2.Box<byte>();
            e2.Return();

            var d2 = e2.CreateDelegate<Func<object>>();

            Assert.Equal("54", d2().ToString());
        }
    }
}
