using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Break
    {
        [Fact]
        public void Simple()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");
            e1.LoadConstant(123);
            e1.Break();
            e1.Pop();
            e1.Return();

            var d1 = e1.CreateDelegate();

            d1();
        }
    }
}
