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
   public class StoreIndirect
    {
        [TestMethod]
        public unsafe void All()
        {
            {
                var e1 = Emit<Func<sbyte>>.NewDynamicMethod("E1");
                var a = e1.DeclareLocal<sbyte>("a");
                e1.LoadLocalAddress(a);
                e1.LoadConstant((sbyte)1);
                e1.StoreIndirect<sbyte>();
                e1.LoadLocal(a);
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual((sbyte)1, d1());
            }

            {
                var e1 = Emit<Func<byte>>.NewDynamicMethod("E1");
                var a = e1.DeclareLocal<byte>("a");
                e1.LoadLocalAddress(a);
                e1.LoadConstant((byte)1);
                e1.StoreIndirect<byte>();
                e1.LoadLocal(a);
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual((byte)1, d1());
            }

            {
                var e1 = Emit<Func<short>>.NewDynamicMethod("E1");
                var a = e1.DeclareLocal<short>("a");
                e1.LoadLocalAddress(a);
                e1.LoadConstant((short)1);
                e1.StoreIndirect<short>();
                e1.LoadLocal(a);
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual((short)1, d1());
            }

            {
                var e1 = Emit<Func<ushort>>.NewDynamicMethod("E1");
                var a = e1.DeclareLocal<ushort>("a");
                e1.LoadLocalAddress(a);
                e1.LoadConstant((ushort)1);
                e1.StoreIndirect<ushort>();
                e1.LoadLocal(a);
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual((ushort)1, d1());
            }

            {
                var e1 = Emit<Func<int>>.NewDynamicMethod("E1");
                var a = e1.DeclareLocal<int>("a");
                e1.LoadLocalAddress(a);
                e1.LoadConstant((int)1);
                e1.StoreIndirect<int>();
                e1.LoadLocal(a);
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual((int)1, d1());
            }

            {
                var e1 = Emit<Func<uint>>.NewDynamicMethod("E1");
                var a = e1.DeclareLocal<uint>("a");
                e1.LoadLocalAddress(a);
                e1.LoadConstant((uint)1);
                e1.StoreIndirect<uint>();
                e1.LoadLocal(a);
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual((uint)1, d1());
            }

            {
                var e1 = Emit<Func<long>>.NewDynamicMethod("E1");
                var a = e1.DeclareLocal<long>("a");
                e1.LoadLocalAddress(a);
                e1.LoadConstant((long)1);
                e1.StoreIndirect<long>();
                e1.LoadLocal(a);
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual((long)1, d1());
            }

            {
                var e1 = Emit<Func<ulong>>.NewDynamicMethod("E1");
                var a = e1.DeclareLocal<ulong>("a");
                e1.LoadLocalAddress(a);
                e1.LoadConstant((ulong)1);
                e1.StoreIndirect<ulong>();
                e1.LoadLocal(a);
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual((ulong)1, d1());
            }

            {
                var e1 = Emit<Func<float>>.NewDynamicMethod("E1");
                var a = e1.DeclareLocal<float>("a");
                e1.LoadLocalAddress(a);
                e1.LoadConstant((float)1);
                e1.StoreIndirect<float>();
                e1.LoadLocal(a);
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual((float)1, d1());
            }

            {
                var e1 = Emit<Func<double>>.NewDynamicMethod("E1");
                var a = e1.DeclareLocal<double>("a");
                e1.LoadLocalAddress(a);
                e1.LoadConstant(3.1415926);
                e1.StoreIndirect<double>();
                e1.LoadLocal(a);
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual(3.1415926, d1());
            }

            {
                var e1 = Emit<Func<object>>.NewDynamicMethod("E1");
                var a = e1.DeclareLocal<object>("a");
                e1.LoadLocalAddress(a);
                e1.LoadConstant("hello world");
                e1.StoreIndirect<object>();
                e1.LoadLocal(a);
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual("hello world", d1());
            }

            {
                var e1 = Emit<Func<IntPtr>>.NewDynamicMethod("E1");
                var a = e1.DeclareLocal(typeof(int*), "a");
                e1.LoadLocalAddress(a);
                e1.LoadConstant(123);
                e1.Convert<IntPtr>();
                e1.StoreIndirect(typeof(int*));
                e1.LoadLocal(a);
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = (int*)d1();

                Assert.IsTrue(x == (int*)123);
            }
        }
    }
}
