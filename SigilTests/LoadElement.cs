using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestClass, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class LoadElement
    {
        [TestMethod]
        public void Simple()
        {
            var e1 = Emit<Func<int[], int, int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.LoadElement<int>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(1, d1(new[] { 3, 1, 2 }, 1));
            Assert.AreEqual(111, d1(new[] { 111, 2, 3 }, 0));

            var e2 = Emit<Func<string[], string>>.NewDynamicMethod("E2");
            e2.LoadArgument(0);
            e2.LoadConstant(1);
            e2.LoadElement<string>();
            e2.Return();

            var d2 = e2.CreateDelegate();

            Assert.AreEqual("hello", d2(new[] { "world", "hello" }));
        }

        [TestMethod]
        public void Byte()
        {
            {
                var e1 = Emit<Func<byte[], int, byte>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadElement<byte>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual(123, d1(new byte[] { 123 }, 0));
            }
        }

        [TestMethod]
        public void SByte()
        {
            {
                var e1 = Emit<Func<sbyte[], int, sbyte>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadElement<sbyte>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual(-100, d1(new sbyte[] { -100 }, 0));
            }
        }

        [TestMethod]
        public unsafe void Pointer()
        {
            {


                var e1 = Emit<Func<int*[], int, IntPtr>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadElement(typeof(int*));
                e1.Return();

                var x = new int*[] { (int*)123 };

                var d1 = e1.CreateDelegate();

                var y = (int*)d1(x, 0);

                Assert.IsTrue(x[0] == y);
            }
        }

        [TestMethod]
        public void Short()
        {
            {
                var e1 = Emit<Func<short[], int, short>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadElement<short>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual((short)-100, d1(new short[] { -100 }, 0));
            }
        }

        [TestMethod]
        public void UShort()
        {
            {
                var e1 = Emit<Func<ushort[], int, ushort>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadElement<ushort>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual((ushort)100, d1(new ushort[] { 100 }, 0));
            }
        }

        [TestMethod]
        public void UInt()
        {
            {
                var e1 = Emit<Func<uint[], int, uint>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadElement<uint>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual((uint)100, d1(new uint[] { 100 }, 0));
            }
        }

        [TestMethod]
        public void Long()
        {
            {
                var e1 = Emit<Func<long[], int, long>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadElement<long>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual((long)100, d1(new long[] { 100 }, 0));
            }
        }

        [TestMethod]
        public void ULong()
        {
            {
                var e1 = Emit<Func<ulong[], int, ulong>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadElement<ulong>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual((ulong)100, d1(new ulong[] { 100 }, 0));
            }
        }

        [TestMethod]
        public void Float()
        {
            {
                var e1 = Emit<Func<float[], int, float>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadElement<float>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual((float)100, d1(new float[] { 100 }, 0));
            }
        }

        [TestMethod]
        public void Double()
        {
            {
                var e1 = Emit<Func<double[], int, double>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadElement<double>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual((double)100, d1(new double[] { 100 }, 0));
            }
        }

        [TestMethod]
        public void Struct()
        {
            {
                var e1 = Emit<Func<DateTime[], int, DateTime>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadElement<DateTime>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var now = DateTime.UtcNow;

                Assert.AreEqual(now, d1(new[] { now }, 0));
            }
        }
    }
}
