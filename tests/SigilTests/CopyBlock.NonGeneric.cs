using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class CopyBlock
    {
        [Fact]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(void), new [] { typeof(byte[]), typeof(byte[]) });

            e1.LoadArgument(1);
            e1.LoadConstant(0);
            e1.LoadElementAddress<byte>();

            e1.LoadArgument(0);
            e1.LoadConstant(0);
            e1.LoadElementAddress<byte>();

            e1.LoadArgument(0);
            e1.LoadLength<byte>();

            e1.CopyBlock();

            e1.Return();

            var d = e1.CreateDelegate<Action<byte[], byte[]>>();

            var a = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var b = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 00 };

            d(a, b);

            for (byte i = 0; i < a.Length; i++)
            {
                Assert.Equal(i + 1, a[i]);
                Assert.Equal(i + 1, b[i]);
            }
        }

        [Fact]
        public void VolatileNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(void), new [] { typeof(byte[]), typeof(byte[]) });

            e1.LoadArgument(1);
            e1.LoadConstant(0);
            e1.LoadElementAddress<byte>();

            e1.LoadArgument(0);
            e1.LoadConstant(0);
            e1.LoadElementAddress<byte>();

            e1.LoadArgument(0);
            e1.LoadLength<byte>();

            e1.CopyBlock(isVolatile: true);

            e1.Return();

            var d = e1.CreateDelegate<Action<byte[], byte[]>>();

            var a = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var b = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 00 };

            d(a, b);

            for (byte i = 0; i < a.Length; i++)
            {
                Assert.Equal(i + 1, a[i]);
                Assert.Equal(i + 1, b[i]);
            }
        }
    }
}
