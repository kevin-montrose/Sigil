using Sigil.NonGeneric;
using System;
using System.Linq;
using Xunit;

namespace SigilTests
{
    public partial class InitializeBlock
    {
        [Fact]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(void), new [] { typeof(byte[]) });

            e1.LoadArgument(0);
            e1.LoadConstant(0);
            e1.LoadElementAddress<byte>();
            e1.LoadConstant(101);
            e1.LoadArgument(0);
            e1.LoadLength<byte>();
            e1.InitializeBlock();
            e1.Return();

            var d1 = e1.CreateDelegate<Action<byte[]>>();

            var b = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            d1(b);

            Assert.True(b.All(x => x == 101));
        }

        [Fact]
        public void VolatileNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(void), new [] { typeof(byte[]) });

            e1.LoadArgument(0);
            e1.LoadConstant(0);
            e1.LoadElementAddress<byte>();
            e1.LoadConstant(101);
            e1.LoadArgument(0);
            e1.LoadLength<byte>();
            e1.InitializeBlock(isVolatile: true);
            e1.Return();

            var d1 = e1.CreateDelegate<Action<byte[]>>();

            var b = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            d1(b);

            Assert.True(b.All(x => x == 101));
        }
    }
}
