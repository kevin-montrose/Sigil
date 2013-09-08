using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    public partial class LoadElement
    {
        [TestMethod]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(int[]), typeof(int) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.LoadElement<int>();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int[], int, int>>();

            Assert.AreEqual(1, d1(new[] { 3, 1, 2 }, 1));
            Assert.AreEqual(111, d1(new[] { 111, 2, 3 }, 0));

            var e2 = Emit.NewDynamicMethod(typeof(string), new [] { typeof(string[]) }, "E2");
            e2.LoadArgument(0);
            e2.LoadConstant(1);
            e2.LoadElement<string>();
            e2.Return();

            var d2 = e2.CreateDelegate<Func<string[], string>>();

            Assert.AreEqual("hello", d2(new[] { "world", "hello" }));
        }

        [TestMethod]
        public void ByteNonGeneric()
        {
            {
                var e1 = Emit.NewDynamicMethod(typeof(byte), new [] { typeof(byte[]), typeof(int) });
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadElement<byte>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<byte[], int, byte>>();

                Assert.AreEqual(123, d1(new byte[] { 123 }, 0));
            }
        }

        [TestMethod]
        public void SByteNonGeneric()
        {
            {
                var e1 = Emit.NewDynamicMethod(typeof(sbyte), new [] { typeof(sbyte[]), typeof(int) });
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadElement<sbyte>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<sbyte[], int, sbyte>>();

                Assert.AreEqual(-100, d1(new sbyte[] { -100 }, 0));
            }
        }

        [TestMethod]
        public unsafe void PointerNonGeneric()
        {
            {
                var e1 = Emit.NewDynamicMethod(typeof(IntPtr), new [] { typeof(int*[]), typeof(int) });
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadElement(typeof(int*));
                e1.Return();

                var x = new int*[] { (int*)123 };

                var d1 = e1.CreateDelegate<Func<int*[], int, IntPtr>>();

                var y = (int*)d1(x, 0);

                Assert.IsTrue(x[0] == y);
            }
        }

        [TestMethod]
        public void ShortNonGeneric()
        {
            {
                var e1 = Emit.NewDynamicMethod(typeof(short), new [] { typeof(short[]), typeof(int) });
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadElement<short>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<short[], int, short>>();

                Assert.AreEqual((short)-100, d1(new short[] { -100 }, 0));
            }
        }

        [TestMethod]
        public void UShortNonGeneric()
        {
            {
                var e1 = Emit.NewDynamicMethod(typeof(ushort), new [] { typeof(ushort[]), typeof(int) });
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadElement<ushort>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<ushort[], int, ushort>>();

                Assert.AreEqual((ushort)100, d1(new ushort[] { 100 }, 0));
            }
        }

        [TestMethod]
        public void UIntNonGeneric()
        {
            {
                var e1 = Emit.NewDynamicMethod(typeof(uint), new [] { typeof(uint[]), typeof(int) });
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadElement<uint>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<uint[], int, uint>>();

                Assert.AreEqual((uint)100, d1(new uint[] { 100 }, 0));
            }
        }

        [TestMethod]
        public void LongNonGeneric()
        {
            {
                var e1 = Emit.NewDynamicMethod(typeof(long), new [] { typeof(long[]), typeof(int) });
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadElement<long>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<long[], int, long>>();

                Assert.AreEqual((long)100, d1(new long[] { 100 }, 0));
            }
        }

        [TestMethod]
        public void ULongNonGeneric()
        {
            {
                var e1 = Emit.NewDynamicMethod(typeof(ulong), new [] { typeof(ulong[]), typeof(int) });
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadElement<ulong>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<ulong[], int, ulong>>();

                Assert.AreEqual((ulong)100, d1(new ulong[] { 100 }, 0));
            }
        }

        [TestMethod]
        public void FloatNonGeneric()
        {
            {
                var e1 = Emit.NewDynamicMethod(typeof(float), new [] { typeof(float[]), typeof(int) });
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadElement<float>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<float[], int, float>>();

                Assert.AreEqual((float)100, d1(new float[] { 100 }, 0));
            }
        }

        [TestMethod]
        public void DoubleNonGeneric()
        {
            {
                var e1 = Emit.NewDynamicMethod(typeof(double), new [] { typeof(double[]), typeof(int) });
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadElement<double>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<double[], int, double>>();

                Assert.AreEqual((double)100, d1(new double[] { 100 }, 0));
            }
        }

        [TestMethod]
        public void StructNonGeneric()
        {
            {
                var e1 = Emit.NewDynamicMethod(typeof(DateTime), new [] { typeof(DateTime[]), typeof(int) });
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadElement<DateTime>();
                e1.Return();

                var d1 = e1.CreateDelegate<Func<DateTime[], int, DateTime>>();

                var now = DateTime.UtcNow;

                Assert.AreEqual(now, d1(new[] { now }, 0));
            }
        }
    }
}
