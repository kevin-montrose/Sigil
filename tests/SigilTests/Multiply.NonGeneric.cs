using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Multiply
    {
        [Fact]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(double), new [] { typeof(double), typeof(double) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Multiply();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<double, double, double>>();

            Assert.Equal(3.14 * 1.59, d1(3.14, 1.59));
        }

        [Fact]
        public void OverflowNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(int), typeof(int) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.MultiplyOverflow();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int, int, int>>();

            Assert.Equal(56 * 100, d1(56, 100));
        }

        [Fact]
        public void UnsignedOverflowNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(int), typeof(int) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.UnsignedMultiplyOverflow();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int, int, int>>();

            Assert.Equal(56 * 100, d1(56, 100));
        }
    }
}
