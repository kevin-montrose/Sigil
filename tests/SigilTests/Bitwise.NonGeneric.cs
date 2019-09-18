using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Bitwise
    {
        [Fact]
        public void AndNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(byte), typeof(byte) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.And();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<byte, byte, int>>();

            byte a = 123, b = 200;

            Assert.Equal(a & b, d1(a, b));
        }

        [Fact]
        public void OrNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(byte), typeof(byte) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Or();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<byte, byte, int>>();

            byte a = 123, b = 200;

            Assert.Equal(a | b, d1(a, b));
        }

        [Fact]
        public void XorNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(byte), typeof(byte) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Xor();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<byte, byte, int>>();

            byte a = 123, b = 200;

            Assert.Equal(a ^ b, d1(a, b));
        }

        [Fact]
        public void NotNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(byte) }, "E1");
            e1.LoadArgument(0);
            e1.Not();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<byte, int>>();

            byte a = 123;

            Assert.Equal(~a, d1(a));
        }
    }
}
