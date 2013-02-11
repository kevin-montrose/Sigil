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
    public class LoadIndirect
    {
        [TestMethod]
        public unsafe void All()
        {
            {
                var e1 = Emit<Func<byte, byte>>.NewDynamicMethod();
                e1.LoadArgumentAddress(0);
                e1.LoadIndirect<byte>();
                e1.Return();

                var d1 = e1.CreateDelegate();
                Assert.AreEqual(111, d1(111));
            }

            {
                var e1 = Emit<Func<sbyte, sbyte>>.NewDynamicMethod();
                e1.LoadArgumentAddress(0);
                e1.LoadIndirect<sbyte>();
                e1.Return();

                var d1 = e1.CreateDelegate();
                Assert.AreEqual(-111, d1(-111));
            }

            {
                var e1 = Emit<Func<short, short>>.NewDynamicMethod();
                e1.LoadArgumentAddress(0);
                e1.LoadIndirect<short>();
                e1.Return();

                var d1 = e1.CreateDelegate();
                Assert.AreEqual(-111, d1(-111));
            }

            {
                var e1 = Emit<Func<ushort, ushort>>.NewDynamicMethod();
                e1.LoadArgumentAddress(0);
                e1.LoadIndirect<ushort>();
                e1.Return();

                var d1 = e1.CreateDelegate();
                Assert.AreEqual((ushort)short.MaxValue, d1((ushort)short.MaxValue));
            }

            {
                var e1 = Emit<Func<int, int>>.NewDynamicMethod();
                e1.LoadArgumentAddress(0);
                e1.LoadIndirect<int>();
                e1.Return();

                var d1 = e1.CreateDelegate();
                Assert.AreEqual(-111, d1(-111));
            }

            {
                var e1 = Emit<Func<uint, uint>>.NewDynamicMethod();
                e1.LoadArgumentAddress(0);
                e1.LoadIndirect<uint>();
                e1.Return();

                var d1 = e1.CreateDelegate();
                Assert.AreEqual((uint)int.MaxValue, d1((uint)int.MaxValue));
            }

            {
                var e1 = Emit<Func<long, long>>.NewDynamicMethod();
                e1.LoadArgumentAddress(0);
                e1.LoadIndirect<long>();
                e1.Return();

                var d1 = e1.CreateDelegate();
                Assert.AreEqual(-111, d1(-111));
            }

            {
                var e1 = Emit<Func<ulong, ulong>>.NewDynamicMethod();
                e1.LoadArgumentAddress(0);
                e1.LoadIndirect<ulong>();
                e1.Return();

                var d1 = e1.CreateDelegate();
                Assert.AreEqual((ulong)long.MaxValue, d1((ulong)long.MaxValue));
            }

            {
                var e1 = Emit<Func<float, float>>.NewDynamicMethod();
                e1.LoadArgumentAddress(0);
                e1.LoadIndirect<float>();
                e1.Return();

                var d1 = e1.CreateDelegate();
                Assert.AreEqual(12.34f, d1(12.34f));
            }

            {
                var e1 = Emit<Func<double, double>>.NewDynamicMethod();
                e1.LoadArgumentAddress(0);
                e1.LoadIndirect<double>();
                e1.Return();

                var d1 = e1.CreateDelegate();
                Assert.AreEqual(12.34, d1(12.34));
            }

            {
                var e1 = Emit<Func<object, object>>.NewDynamicMethod();
                e1.LoadArgumentAddress(0);
                e1.LoadIndirect<object>();
                e1.Return();

                var d1 = e1.CreateDelegate();
                Assert.AreEqual("hello", d1("hello"));
            }

            {
                var e1 = Emit<Func<IntPtr, IntPtr>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadIndirect(typeof(int*));
                e1.Return();

                var x = (int*)3;
                var y = &x;

                var d1 = e1.CreateDelegate();

                var z = d1((IntPtr)y);

                Assert.AreEqual((IntPtr)3, z);
            }
        }
    }
}
