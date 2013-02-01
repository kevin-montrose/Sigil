using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestClass]
    public class Converts
    {
        [TestMethod]
        public void Convert()
        {
            {
                var e1 = Emit<Func<int, byte>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.Convert<byte>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual((byte)111, d1(111));
            }

            {
                var e1 = Emit<Func<int, sbyte>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.Convert<sbyte>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual((sbyte)-11, d1(-11));
            }

            {
                var e1 = Emit<Func<int, short>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.Convert<short>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual((short)2111, d1(2111));
            }

            {
                var e1 = Emit<Func<int, ushort>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.Convert<ushort>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual(ushort.MaxValue, d1(ushort.MaxValue));
            }

            {
                var e1 = Emit<Func<byte, int>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.Convert<int>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual(123, d1(123));
            }

            {
                var e1 = Emit<Func<long, uint>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.Convert<uint>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = (uint)int.MaxValue;
                x++;

                Assert.AreEqual(x, d1(x));
            }

            {
                var e1 = Emit<Func<float, long>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.Convert<long>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual(12345678, d1(12345678f));
            }

            {
                var e1 = Emit<Func<float, ulong>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.Convert<ulong>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual((ulong)12345678, d1(12345678f));
            }

            {
                var e1 = Emit<Func<int, float>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.Convert<float>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual(123f, d1(123));
            }

            {
                var e1 = Emit<Func<int, double>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.Convert<double>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual(123.0, d1(123));
            }

            {
                var e1 = Emit<Func<int, IntPtr>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.Convert<IntPtr>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var intPtr = new IntPtr(123);

                Assert.AreEqual(intPtr, d1(123));
            }

            {
                var e1 = Emit<Func<int, UIntPtr>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.Convert<UIntPtr>();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var uintPtr = new UIntPtr(123);

                Assert.AreEqual(uintPtr, d1(123));
            }
        }
    }
}
