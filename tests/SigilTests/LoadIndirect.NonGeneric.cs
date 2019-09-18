using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class LoadIndirect
    {
        [Fact]
        public void UnalignedNonGeneric()
        {
            {
                var e1 = Emit.NewDynamicMethod(typeof(byte), new [] { typeof(byte) });
                e1.LoadArgumentAddress(0);
                e1.LoadIndirect<byte>(unaligned: 2);
                e1.Return();

                var d1 = e1.CreateDelegate<Func<byte, byte>>();
                Assert.Equal(111, d1(111));
            }
        }

        [Fact]
        public unsafe void AllNonGeneric()
        {
            {
                var e1 = Emit.NewDynamicMethod(typeof(byte), new [] { typeof(byte) });
                e1.LoadArgumentAddress(0);
                e1.LoadIndirect<byte>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<byte, byte>>();
                Assert.Equal(111, d1(111));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(sbyte), new [] { typeof(sbyte) });
                e1.LoadArgumentAddress(0);
                e1.LoadIndirect<sbyte>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<sbyte, sbyte>>();
                Assert.Equal(-111, d1(-111));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(short), new [] { typeof(short) });
                e1.LoadArgumentAddress(0);
                e1.LoadIndirect<short>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<short, short>>();
                Assert.Equal(-111, d1(-111));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(ushort), new [] { typeof(ushort) });
                e1.LoadArgumentAddress(0);
                e1.LoadIndirect<ushort>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<ushort, ushort>>();
                Assert.Equal((ushort)short.MaxValue, d1((ushort)short.MaxValue));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(int) });
                e1.LoadArgumentAddress(0);
                e1.LoadIndirect<int>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<int, int>>();
                Assert.Equal(-111, d1(-111));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(uint), new [] { typeof(uint) });
                e1.LoadArgumentAddress(0);
                e1.LoadIndirect<uint>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<uint, uint>>();
                Assert.Equal((uint)int.MaxValue, d1((uint)int.MaxValue));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(long), new [] { typeof(long) });
                e1.LoadArgumentAddress(0);
                e1.LoadIndirect<long>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<long, long>>();
                Assert.Equal(-111, d1(-111));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(ulong), new [] { typeof(ulong) });
                e1.LoadArgumentAddress(0);
                e1.LoadIndirect<ulong>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<ulong, ulong>>();
                Assert.Equal((ulong)long.MaxValue, d1((ulong)long.MaxValue));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(float), new [] { typeof(float) });
                e1.LoadArgumentAddress(0);
                e1.LoadIndirect<float>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<float, float>>();
                Assert.Equal(12.34f, d1(12.34f));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(double), new [] { typeof(double) });
                e1.LoadArgumentAddress(0);
                e1.LoadIndirect<double>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<double, double>>();
                Assert.Equal(12.34, d1(12.34));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(object), new [] { typeof(object) });
                e1.LoadArgumentAddress(0);
                e1.LoadIndirect<object>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<object, object>>();
                Assert.Equal("hello", d1("hello"));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(IntPtr), new [] { typeof(IntPtr) });
                e1.LoadArgument(0);
                e1.LoadIndirect(typeof(int*));
                e1.Return();

                var x = (int*)3;
                var y = &x;

                var d1 = e1.CreateDelegate<Func<IntPtr, IntPtr>>();

                var z = d1((IntPtr)y);

                Assert.Equal((IntPtr)3, z);
            }
        }
    }
}
