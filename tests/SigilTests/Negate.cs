using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Negate
    {
        [Fact]
        public void Simple()
        {
            var e1 = Emit<Func<int, int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.Negate();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(-123, d1(123));
        }

        [Fact]
        public void Long()
        {
            var e1 = Emit<Func<long, long>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.Negate();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(-123L, d1(123));
        }
    }
}
