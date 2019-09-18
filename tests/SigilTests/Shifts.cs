using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Shifts
    {
        [Fact]
        public void Left()
        {
            var e1 = Emit<Func<int, int, int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.ShiftLeft();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(1 << 3, d1(1, 3));
            Assert.Equal(5 << 2, d1(5, 2));
        }

        [Fact]
        public void Right()
        {
            var e1 = Emit<Func<int, int, int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.ShiftRight();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(1234 >> 2, d1(1234, 2));
            Assert.Equal(8675309 >> 5, d1(8675309, 5));
        }

        [Fact]
        public void RightUnsigned()
        {
            var e1 = Emit<Func<uint, uint, uint>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.UnsignedShiftRight();
            e1.Return();

            var d1 = e1.CreateDelegate();

            uint x = 1234;
            x = x >> 2;

            uint y = 8675309;
            y = y >> 5;

            Assert.Equal(x, d1(1234, 2));
            Assert.Equal(y, d1(8675309, 5));
        }
    }
}
