using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class StoreElement
    {
        [Fact]
        public unsafe void AllNonGeneric()
        {
            {
                var e1 = Emit.NewDynamicMethod(typeof(void), new [] { typeof(sbyte[]), typeof(int), typeof(sbyte) });
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement<sbyte>();
                e1.Return();

                var d1 = e1.CreateDelegate<Action<sbyte[], int, sbyte>>();

                var x = new sbyte[] { 5 };
                d1(x, 0, 10);

                Assert.Equal((sbyte)10, x[0]);
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(void), new [] { typeof(byte[]), typeof(int), typeof(byte) });
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement<byte>();
                e1.Return();

                var d1 = e1.CreateDelegate<Action<byte[], int, byte>>();

                var x = new byte[] { 5 };
                d1(x, 0, 10);

                Assert.Equal((byte)10, x[0]);
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(void), new [] { typeof(short[]), typeof(int), typeof(short) });
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement<short>();
                e1.Return();

                var d1 = e1.CreateDelegate<Action<short[], int, short>>();

                var x = new short[] { 5 };
                d1(x, 0, 10);

                Assert.Equal((short)10, x[0]);
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(void), new [] { typeof(ushort[]), typeof(int), typeof(ushort) });
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement<ushort>();
                e1.Return();

                var d1 = e1.CreateDelegate<Action<ushort[], int, ushort>>();

                var x = new ushort[] { 5 };
                d1(x, 0, 10);

                Assert.Equal((ushort)10, x[0]);
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(void), new [] { typeof(int[]), typeof(int), typeof(int) });
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement<int>();
                e1.Return();

                var d1 = e1.CreateDelegate<Action<int[], int, int>>();

                var x = new int[] { 5 };
                d1(x, 0, 10);

                Assert.Equal((int)10, x[0]);
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(void), new [] { typeof(uint[]), typeof(int), typeof(uint) });
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement<uint>();
                e1.Return();

                var d1 = e1.CreateDelegate<Action<uint[], int, uint>>();

                var x = new uint[] { 5 };
                d1(x, 0, 10);

                Assert.Equal((uint)10, x[0]);
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(void), new [] { typeof(long[]), typeof(int), typeof(long) });
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement<long>();
                e1.Return();

                var d1 = e1.CreateDelegate<Action<long[], int, long>>();

                var x = new long[] { 5 };
                d1(x, 0, 10);

                Assert.Equal((long)10, x[0]);
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(void), new [] { typeof(ulong[]), typeof(int), typeof(ulong) });
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement<ulong>();
                e1.Return();

                var d1 = e1.CreateDelegate<Action<ulong[], int, ulong>>();

                var x = new ulong[] { 5 };
                d1(x, 0, 10);

                Assert.Equal((ulong)10, x[0]);
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(void), new [] { typeof(float[]), typeof(int), typeof(float) });
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement<float>();
                e1.Return();

                var d1 = e1.CreateDelegate<Action<float[], int, float>>();

                var x = new float[] { 5 };
                d1(x, 0, 10);

                Assert.Equal((float)10, x[0]);
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(void), new[] { typeof(double[]), typeof(int), typeof(double) });
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement<double>();
                e1.Return();

                var d1 = e1.CreateDelegate<Action<double[], int, double>>();

                var x = new double[] { 5 };
                d1(x, 0, 10);

                Assert.Equal((double)10, x[0]);
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(void), new [] { typeof(object[]), typeof(int), typeof(object) });
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement<object>();
                e1.Return();

                var d1 = e1.CreateDelegate<Action<object[], int, object>>();

                var x = new object[] { "hello" };
                d1(x, 0, "world");

                Assert.Equal("world", (string)x[0]);
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(void), new [] {typeof(DateTime[]), typeof(int), typeof(DateTime) });
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement<DateTime>();
                e1.Return();

                var d1 = e1.CreateDelegate<Action<DateTime[], int, DateTime>>();

                var now = DateTime.UtcNow;

                var x = new DateTime[] { DateTime.MinValue };
                d1(x, 0, now);

                Assert.Equal(now, x[0]);
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(void), new [] { typeof(int*[]), typeof(int), typeof(IntPtr) });
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement(typeof(int*));
                e1.Return();

                var d1 = e1.CreateDelegate<Action<int*[], int, IntPtr>>();

                var ptr = new IntPtr(123);

                var x = new int*[] { (int*)new IntPtr(456) };
                d1(x, 0, ptr);

                var y = (int*)ptr == x[0];

                Assert.True(y);
            }
        }
    }
}
