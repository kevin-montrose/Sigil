using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Compares
    {
        [Fact]
        public void EqualsTest()
        {
            var e1 = Emit<Func<int, int, bool>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.CompareEqual();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.True(d1(1, 1));
            Assert.False(d1(1, 2));
        }

        [Fact]
        public void GreaterThan()
        {
            var e1 = Emit<Func<int, int, bool>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.CompareGreaterThan();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.True(d1(5, 1));
            Assert.False(d1(1, 1));
        }

        [Fact]
        public void LessThan()
        {
            var e1 = Emit<Func<int, int, bool>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.CompareLessThan();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.True(d1(6, 10));
            Assert.False(d1(1, 1));
        }

        [Fact]
        public void UnsignedCompareGreaterThan()
        {
            var e1 = Emit<Func<uint, uint, bool>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.UnsignedCompareGreaterThan();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.True(d1(uint.MaxValue, (uint)int.MaxValue));
            Assert.False(d1(1, 1));
        }

        [Fact]
        public void UnsignedCompareLessThan()
        {
            var e1 = Emit<Func<uint, uint, bool>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.UnsignedCompareLessThan();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.True(d1((uint)int.MaxValue, uint.MaxValue));
            Assert.False(d1(1, 1));
        }
    }
}
