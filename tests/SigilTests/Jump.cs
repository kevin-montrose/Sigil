using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Jump
    {
        private static int SimpleSet;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1013:Public method should be marked as test", Justification = "It needs to be public")]
        public static void SimpleSetM(int x)
        {
            SimpleSet = x;
        }

        [Fact]
        public void Simple()
        {
            var simpleSet = typeof(Jump).GetMethod("SimpleSetM");

            var e1 = Emit<Action<int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadConstant(int.MaxValue);
            e1.Xor();
            e1.StoreArgument(0);
            e1.Jump(simpleSet);
            e1.Return();

            var d1 = e1.CreateDelegate();

            d1(1234);
            Assert.Equal(1234 ^ int.MaxValue, SimpleSet);
            d1(777);
            Assert.Equal(777 ^ int.MaxValue, SimpleSet);
            d1(0x12345678);
            Assert.Equal(0x12345678 ^ int.MaxValue, SimpleSet);
        }
    }
}
