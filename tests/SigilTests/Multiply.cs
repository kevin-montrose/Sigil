using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Multiply
    {
        [Fact]
        public void Simple()
        {
            var e1 = Emit<Func<double, double, double>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Multiply();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(3.14 * 1.59, d1(3.14, 1.59));
        }

        [Fact]
        public void Overflow()
        {
            var e1 = Emit<Func<int, int, int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.MultiplyOverflow();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(56 * 100, d1(56, 100));
        }

        [Fact]
        public void UnsignedOverflow()
        {
            var e1 = Emit<Func<int, int, int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.UnsignedMultiplyOverflow();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(56 * 100, d1(56, 100));
        }
    }
}
