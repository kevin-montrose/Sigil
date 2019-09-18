using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class StoreElement
    {
        [Fact]
        public unsafe void All()
        {
            {
                var e1 = Emit<Action<sbyte[], int, sbyte>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement<sbyte>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = new sbyte[] { 5 };
                d1(x, 0, 10);

                Assert.Equal((sbyte)10, x[0]);
            }

            {
                var e1 = Emit<Action<byte[], int, byte>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement<byte>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = new byte[] { 5 };
                d1(x, 0, 10);

                Assert.Equal((byte)10, x[0]);
            }

            {
                var e1 = Emit<Action<short[], int, short>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement<short>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = new short[] { 5 };
                d1(x, 0, 10);

                Assert.Equal((short)10, x[0]);
            }

            {
                var e1 = Emit<Action<ushort[], int, ushort>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement<ushort>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = new ushort[] { 5 };
                d1(x, 0, 10);

                Assert.Equal((ushort)10, x[0]);
            }

            {
                var e1 = Emit<Action<int[], int, int>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement<int>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = new int[] { 5 };
                d1(x, 0, 10);

                Assert.Equal((int)10, x[0]);
            }

            {
                var e1 = Emit<Action<uint[], int, uint>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement<uint>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = new uint[] { 5 };
                d1(x, 0, 10);

                Assert.Equal((uint)10, x[0]);
            }

            {
                var e1 = Emit<Action<long[], int, long>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement<long>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = new long[] { 5 };
                d1(x, 0, 10);

                Assert.Equal((long)10, x[0]);
            }

            {
                var e1 = Emit<Action<ulong[], int, ulong>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement<ulong>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = new ulong[] { 5 };
                d1(x, 0, 10);

                Assert.Equal((ulong)10, x[0]);
            }

            {
                var e1 = Emit<Action<float[], int, float>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement<float>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = new float[] { 5 };
                d1(x, 0, 10);

                Assert.Equal((float)10, x[0]);
            }

            {
                var e1 = Emit<Action<double[], int, double>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement<double>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = new double[] { 5 };
                d1(x, 0, 10);

                Assert.Equal((double)10, x[0]);
            }

            {
                var e1 = Emit<Action<object[], int, object>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement<object>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = new object[] { "hello" };
                d1(x, 0, "world");

                Assert.Equal("world", (string)x[0]);
            }

            {
                var e1 = Emit<Action<DateTime[], int, DateTime>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement<DateTime>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var now = DateTime.UtcNow;

                var x = new DateTime[] { DateTime.MinValue };
                d1(x, 0, now);

                Assert.Equal(now, x[0]);
            }

            {
                var e1 = Emit<Action<int*[], int, IntPtr>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement(typeof(int*));
                e1.Return();

                var d1 = e1.CreateDelegate();

                var ptr = new IntPtr(123);

                var x = new int*[] { (int*)new IntPtr(456) };
                d1(x, 0, ptr);

                var y = (int*)ptr == x[0];

                Assert.True(y);
            }
        }
    }
}
