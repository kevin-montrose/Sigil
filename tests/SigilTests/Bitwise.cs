using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Bitwise
    {
        [Fact]
        public void And()
        {
            var e1 = Emit<Func<byte, byte, int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.And();
            e1.Return();

            var d1 = e1.CreateDelegate();

            byte a = 123, b = 200;

            Assert.Equal(a & b, d1(a, b));
        }

        [Fact]
        public void Or()
        {
            var e1 = Emit<Func<byte, byte, int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Or();
            e1.Return();

            var d1 = e1.CreateDelegate();

            byte a = 123, b = 200;

            Assert.Equal(a | b, d1(a, b));
        }

        [Fact]
        public void Xor()
        {
            var e1 = Emit<Func<byte, byte, int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Xor();
            e1.Return();

            var d1 = e1.CreateDelegate();

            byte a = 123, b = 200;

            Assert.Equal(a ^ b, d1(a, b));
        }

        [Fact]
        public void Not()
        {
            var e1 = Emit<Func<byte, int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.Not();
            e1.Return();

            var d1 = e1.CreateDelegate();

            byte a = 123;

            Assert.Equal(~a, d1(a));
        }
    }
}
