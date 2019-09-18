using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Negate
    {
        [Fact]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(int) }, "E1");
            e1.LoadArgument(0);
            e1.Negate();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int, int>>();

            Assert.Equal(-123, d1(123));
        }

        [Fact]
        public void LongNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(long), new [] { typeof(long) }, "E1");
            e1.LoadArgument(0);
            e1.Negate();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<long, long>>();

            Assert.Equal(-123L, d1(123));
        }
    }
}
