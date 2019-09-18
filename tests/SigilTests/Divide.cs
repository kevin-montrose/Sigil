using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Divide
    {
        [Fact]
        public void Simple()
        {
            var e1 = Emit<Func<double, double, double>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Divide();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(3.14 / 1.59, d1(3.14, 1.59));
        }

        [Fact]
        public void Unsigned()
        {
            var e1 = Emit<Func<uint, uint, uint>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.UnsignedDivide();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(uint.MaxValue / ((uint)1234), d1(uint.MaxValue, (uint)1234));
        }
    }
}
