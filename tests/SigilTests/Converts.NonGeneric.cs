using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Converts
    {
        [Fact]
        public void SimpleNonGeneric()
        {
            {
                var e1 = Emit.NewDynamicMethod(typeof(byte), new [] { typeof(int) });
                e1.LoadArgument(0);
                e1.Convert<byte>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<int, byte>>();

                Assert.Equal((byte)111, d1(111));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(sbyte), new [] { typeof(int) });
                e1.LoadArgument(0);
                e1.Convert<sbyte>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<int, sbyte>>();

                Assert.Equal((sbyte)-11, d1(-11));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(short), new [] { typeof(int) });
                e1.LoadArgument(0);
                e1.Convert<short>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<int, short>>();

                Assert.Equal((short)2111, d1(2111));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(ushort), new [] { typeof(int) });
                e1.LoadArgument(0);
                e1.Convert<ushort>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<int, ushort>>();

                Assert.Equal(ushort.MaxValue, d1(ushort.MaxValue));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(byte) });
                e1.LoadArgument(0);
                e1.Convert<int>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<byte, int>>();

                Assert.Equal(123, d1(123));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(uint), new [] { typeof(long) });
                e1.LoadArgument(0);
                e1.Convert<uint>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<long, uint>>();

                var x = (uint)int.MaxValue;
                x++;

                Assert.Equal(x, d1(x));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(long), new [] { typeof(float) });
                e1.LoadArgument(0);
                e1.Convert<long>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<float, long>>();

                Assert.Equal(12345678, d1(12345678f));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(ulong), new [] { typeof(float) });
                e1.LoadArgument(0);
                e1.Convert<ulong>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<float, ulong>>();

                Assert.Equal((ulong)12345678, d1(12345678f));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(float), new [] { typeof(int) });
                e1.LoadArgument(0);
                e1.Convert<float>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<int, float>>();

                Assert.Equal(123f, d1(123));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(double), new [] { typeof(int) });
                e1.LoadArgument(0);
                e1.Convert<double>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<int, double>>();

                Assert.Equal(123.0, d1(123));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(IntPtr), new [] { typeof(int) });
                e1.LoadArgument(0);
                e1.Convert<IntPtr>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<int, IntPtr>>();

                var intPtr = new IntPtr(123);

                Assert.Equal(intPtr, d1(123));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(UIntPtr), new [] { typeof(int) });
                e1.LoadArgument(0);
                e1.Convert<UIntPtr>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<int, UIntPtr>>();

                var uintPtr = new UIntPtr(123);

                Assert.Equal(uintPtr, d1(123));
            }
        }

        [Fact]
        public void OverflowsNonGeneric()
        {
            {
                var e1 = Emit.NewDynamicMethod(typeof(byte), new [] { typeof(int) });
                e1.LoadArgument(0);
                e1.ConvertOverflow<byte>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<int, byte>>();

                Assert.Equal((byte)111, d1(111));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(sbyte), new [] { typeof(int) });
                e1.LoadArgument(0);
                e1.ConvertOverflow<sbyte>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<int, sbyte>>();

                Assert.Equal((sbyte)-11, d1(-11));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(short), new [] { typeof(int) });
                e1.LoadArgument(0);
                e1.ConvertOverflow<short>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<int, short>>();

                Assert.Equal((short)2111, d1(2111));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(ushort), new [] { typeof(int) });
                e1.LoadArgument(0);
                e1.ConvertOverflow<ushort>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<int, ushort>>();

                Assert.Equal(ushort.MaxValue, d1(ushort.MaxValue));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(byte) });
                e1.LoadArgument(0);
                e1.ConvertOverflow<int>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<byte, int>>();

                Assert.Equal(123, d1(123));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(uint), new [] { typeof(long) });
                e1.LoadArgument(0);
                e1.ConvertOverflow<uint>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<long, uint>>();

                var x = (uint)int.MaxValue;
                x++;

                Assert.Equal(x, d1(x));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(long), new [] { typeof(float) });
                e1.LoadArgument(0);
                e1.ConvertOverflow<long>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<float, long>>();

                Assert.Equal(12345678, d1(12345678f));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(ulong), new [] { typeof(float) });
                e1.LoadArgument(0);
                e1.ConvertOverflow<ulong>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<float, ulong>>();

                Assert.Equal((ulong)12345678, d1(12345678f));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(IntPtr), new [] { typeof(int) });
                e1.LoadArgument(0);
                e1.ConvertOverflow<IntPtr>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<int, IntPtr>>();

                var intPtr = new IntPtr(123);

                Assert.Equal(intPtr, d1(123));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(UIntPtr), new [] { typeof(int) });
                e1.LoadArgument(0);
                e1.ConvertOverflow<UIntPtr>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<int, UIntPtr>>();

                var uintPtr = new UIntPtr(123);

                Assert.Equal(uintPtr, d1(123));
            }
        }

        [Fact]
        public void UnsignedOverflowsNonGeneric()
        {
            {
                var e1 = Emit.NewDynamicMethod(typeof(byte), new [] { typeof(int) });
                e1.LoadArgument(0);
                e1.UnsignedConvertOverflow<byte>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<int, byte>>();

                Assert.Equal((byte)111, d1(111));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(sbyte), new [] { typeof(int) });
                e1.LoadArgument(0);
                e1.UnsignedConvertOverflow<sbyte>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<int, sbyte>>();

                Assert.Throws<OverflowException>(() => d1(-11));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(short), new [] { typeof(int) });
                e1.LoadArgument(0);
                e1.UnsignedConvertOverflow<short>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<int, short>>();

                Assert.Equal((short)2111, d1(2111));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(ushort), new [] { typeof(int) });
                e1.LoadArgument(0);
                e1.UnsignedConvertOverflow<ushort>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<int, ushort>>();

                Assert.Equal(ushort.MaxValue, d1(ushort.MaxValue));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(byte) });
                e1.LoadArgument(0);
                e1.UnsignedConvertOverflow<int>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<byte, int>>();

                Assert.Equal(123, d1(123));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(uint), new [] { typeof(long) });
                e1.LoadArgument(0);
                e1.UnsignedConvertOverflow<uint>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<long, uint>>();

                var x = (uint)int.MaxValue;
                x++;

                Assert.Equal(x, d1(x));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(long), new [] { typeof(float) });
                e1.LoadArgument(0);
                e1.UnsignedConvertOverflow<long>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<float, long>>();

                Assert.Equal(12345678, d1(12345678f));
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(ulong), new [] { typeof(float) });
                e1.LoadArgument(0);
                e1.UnsignedConvertOverflow<ulong>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<float, ulong>>();

                Assert.Equal((ulong)12345678, d1(12345678f));
            }
        }
    }
}
