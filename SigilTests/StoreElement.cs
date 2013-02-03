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
    public class StoreElement
    {
        [TestMethod]
        public unsafe void All()
        {
            {
                var e1 = Emit<Action<sbyte[], int, sbyte>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = new sbyte[] { 5 };
                d1(x, 0, 10);

                Assert.AreEqual((sbyte)10, x[0]);
            }

            {
                var e1 = Emit<Action<byte[], int, byte>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = new byte[] { 5 };
                d1(x, 0, 10);

                Assert.AreEqual((byte)10, x[0]);
            }

            {
                var e1 = Emit<Action<short[], int, short>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = new short[] { 5 };
                d1(x, 0, 10);

                Assert.AreEqual((short)10, x[0]);
            }

            {
                var e1 = Emit<Action<ushort[], int, ushort>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = new ushort[] { 5 };
                d1(x, 0, 10);

                Assert.AreEqual((ushort)10, x[0]);
            }

            {
                var e1 = Emit<Action<int[], int, int>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = new int[] { 5 };
                d1(x, 0, 10);

                Assert.AreEqual((int)10, x[0]);
            }

            {
                var e1 = Emit<Action<uint[], int, uint>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = new uint[] { 5 };
                d1(x, 0, 10);

                Assert.AreEqual((uint)10, x[0]);
            }

            {
                var e1 = Emit<Action<long[], int, long>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = new long[] { 5 };
                d1(x, 0, 10);

                Assert.AreEqual((long)10, x[0]);
            }

            {
                var e1 = Emit<Action<ulong[], int, ulong>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = new ulong[] { 5 };
                d1(x, 0, 10);

                Assert.AreEqual((ulong)10, x[0]);
            }

            {
                var e1 = Emit<Action<float[], int, float>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = new float[] { 5 };
                d1(x, 0, 10);

                Assert.AreEqual((float)10, x[0]);
            }

            {
                var e1 = Emit<Action<double[], int, double>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = new double[] { 5 };
                d1(x, 0, 10);

                Assert.AreEqual((double)10, x[0]);
            }

            {
                var e1 = Emit<Action<object[], int, object>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = new object[] { "hello" };
                d1(x, 0, "world");

                Assert.AreEqual("world", (string)x[0]);
            }

            {
                var e1 = Emit<Action<DateTime[], int, DateTime>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var now = DateTime.UtcNow;

                var x = new DateTime[] { DateTime.MinValue };
                d1(x, 0, now);

                Assert.AreEqual(now, x[0]);
            }

            {
                var e1 = Emit<Action<int*[], int, IntPtr>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);
                e1.LoadArgument(2);
                e1.StoreElement();
                e1.Return();

                var d1 = e1.CreateDelegate();

                var ptr = new IntPtr(123);

                var x = new int*[] { (int*)new IntPtr(456) };
                d1(x, 0, ptr);

                var y = (int*)ptr == x[0];

                Assert.IsTrue(y);
            }
        }
    }
}
