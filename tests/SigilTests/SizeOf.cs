using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class SizeOf
    {
        [Fact]
        public void Simple()
        {
            var e1 = Emit<Func<int>>.NewDynamicMethod("E1");
            e1.SizeOf<int>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(sizeof(int), d1());
        }
    }
}
