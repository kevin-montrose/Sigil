using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Compares
    {
        [Fact]
        public void EqualsTestNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(bool), new [] { typeof(int), typeof(int) });
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.CompareEqual();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int, int, bool>>();

            Assert.True(d1(1, 1));
            Assert.False(d1(1, 2));
        }

        [Fact]
        public void GreaterThanNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(bool), new [] { typeof(int), typeof(int) });
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.CompareGreaterThan();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int, int, bool>>();

            Assert.True(d1(5, 1));
            Assert.False(d1(1, 1));
        }

        [Fact]
        public void LessThanNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(bool), new [] { typeof(int), typeof(int) });
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.CompareLessThan();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int, int, bool>>();

            Assert.True(d1(6, 10));
            Assert.False(d1(1, 1));
        }

        [Fact]
        public void UnsignedCompareGreaterThanNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(bool), new [] { typeof(uint), typeof(uint) });
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.UnsignedCompareGreaterThan();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<uint, uint, bool>>();

            Assert.True(d1(uint.MaxValue, (uint)int.MaxValue));
            Assert.False(d1(1, 1));
        }

        [Fact]
        public void UnsignedCompareLessThanNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(bool), new [] { typeof(uint), typeof(uint) });
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.UnsignedCompareLessThan();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<uint, uint, bool>>();

            Assert.True(d1((uint)int.MaxValue, uint.MaxValue));
            Assert.False(d1(1, 1));
        }
    }
}
