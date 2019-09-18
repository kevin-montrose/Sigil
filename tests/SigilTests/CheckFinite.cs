using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class CheckFinite
    {
        [Fact]
        public void Simple()
        {
            var e1 = Emit<Action<double>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.CheckFinite();
            e1.Pop();
            e1.Return();

            var d1 = e1.CreateDelegate();

            d1(123);

            Assert.Throws<OverflowException>(() => d1(double.PositiveInfinity));
        }
    }
}
