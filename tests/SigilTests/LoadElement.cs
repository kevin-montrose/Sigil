using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class LoadElement
    {
        [Fact]
        public void Simple()
        {
            var e1 = Emit<Func<int[], int, int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.LoadElement<int>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(1, d1(new[] { 3, 1, 2 }, 1));
            Assert.Equal(111, d1(new[] { 111, 2, 3 }, 0));

            var e2 = Emit<Func<string[], string>>.NewDynamicMethod("E2");
            e2.LoadArgument(0);
            e2.LoadConstant(1);
            e2.LoadElement<string>();
            e2.Return();

            var d2 = e2.CreateDelegate();

            Assert.Equal("hello", d2(new[] { "world", "hello" }));
        }

        [Fact]
        public void Byte()
        {
            var e1 = Emit<Func<byte[], int, byte>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.LoadElement<byte>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(123, d1(new byte[] { 123 }, 0));
        }

        [Fact]
        public void SByte()
        {
            var e1 = Emit<Func<sbyte[], int, sbyte>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.LoadElement<sbyte>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(-100, d1(new sbyte[] { -100 }, 0));
        }

        [Fact]
        public unsafe void Pointer()
        {
            var e1 = Emit<Func<int*[], int, IntPtr>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.LoadElement(typeof(int*));
            e1.Return();

            var x = new int*[] { (int*)123 };

            var d1 = e1.CreateDelegate();

            var y = (int*)d1(x, 0);

            Assert.True(x[0] == y);
        }

        [Fact]
        public void Short()
        {
            var e1 = Emit<Func<short[], int, short>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.LoadElement<short>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal((short)-100, d1(new short[] { -100 }, 0));
        }

        [Fact]
        public void UShort()
        {
            var e1 = Emit<Func<ushort[], int, ushort>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.LoadElement<ushort>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal((ushort)100, d1(new ushort[] { 100 }, 0));
        }

        [Fact]
        public void UInt()
        {
            var e1 = Emit<Func<uint[], int, uint>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.LoadElement<uint>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal((uint)100, d1(new uint[] { 100 }, 0));
        }

        [Fact]
        public void Long()
        {
            var e1 = Emit<Func<long[], int, long>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.LoadElement<long>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal((long)100, d1(new long[] { 100 }, 0));
        }

        [Fact]
        public void ULong()
        {
            var e1 = Emit<Func<ulong[], int, ulong>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.LoadElement<ulong>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal((ulong)100, d1(new ulong[] { 100 }, 0));
        }

        [Fact]
        public void Float()
        {
            var e1 = Emit<Func<float[], int, float>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.LoadElement<float>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal((float)100, d1(new float[] { 100 }, 0));
        }

        [Fact]
        public void Double()
        {
            var e1 = Emit<Func<double[], int, double>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.LoadElement<double>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal((double)100, d1(new double[] { 100 }, 0));
        }

        [Fact]
        public void Struct()
        {
            var e1 = Emit<Func<DateTime[], int, DateTime>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.LoadElement<DateTime>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            var now = DateTime.UtcNow;

            Assert.Equal(now, d1(new[] { now }, 0));
        }
    }
}
