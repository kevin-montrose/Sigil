using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Remainder
    {
        [Fact]
        public void Simple()
        {
            var e1 = Emit<Func<int, int, int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Remainder();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(8675309 % 314, d1(8675309, 314));
        }

        [Fact]
        public void Unsigned()
        {
            var e1 = Emit<Func<uint, uint, uint>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.UnsignedRemainder();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(uint.MaxValue % ((uint)1234), d1(uint.MaxValue, (uint)1234));
        }
    }
}
