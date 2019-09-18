using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Unbox
    {
        [Fact]
        public void JustUnbox()
        {
            var e1 = Emit<Func<object, int>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.Unbox<int>();
            e1.LoadIndirect<int>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(1234567, d1(1234567));
        }

        [Fact]
        public void UnboxAny()
        {
            var e1 = Emit<Func<object, int>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.UnboxAny<int>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(1234567, d1(1234567));
        }
    }
}
