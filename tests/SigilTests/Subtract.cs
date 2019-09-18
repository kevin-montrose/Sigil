using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Subtract
    {
        [Fact]
        public void Simple()
        {
            var e1 = Emit<Func<double, double, double>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Subtract();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(3.14 - 1.59, d1(3.14, 1.59));
        }

        [Fact]
        public void Overflow()
        {
            var e1 = Emit<Func<int, int, int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.SubtractOverflow();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(4 - 5, d1(4, 5));
        }

        [Fact]
        public void UnsignedOverflow()
        {
            var e1 = Emit<Func<int, int, int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.UnsignedSubtractOverflow();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(1234 - 5, d1(1234, 5));
        }
    }
}
