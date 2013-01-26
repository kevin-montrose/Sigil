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
    public class Add
    {
        [TestMethod]
        public void IntInt()
        {
            var il = Emit<Func<int>>.NewDynamicMethod("IntInt");
            il.LoadConstant(1);
            il.LoadConstant(2);
            il.Add();
            il.Return();

            var del = il.CreateDelegate();
            Assert.AreEqual(3, del());
        }

        [TestMethod]
        public void IntNativeInt()
        {
            var e1 = Emit<Func<int>>.NewDynamicMethod("E1");
            e1.LoadConstant(1);
            e1.ConvertToNativeInt();
            e1.LoadConstant(3);
            e1.Add();
            e1.ConvertToInt32();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(4, d1());
        }

        [TestMethod]
        public void NativeIntInt()
        {
            var e1 = Emit<Func<int>>.NewDynamicMethod("E1");
            e1.LoadConstant(1);
            e1.LoadConstant(3);
            e1.ConvertToNativeInt();
            e1.Add();
            e1.ConvertToInt32();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(4, d1());
        }

        [TestMethod]
        public void NativeIntNativeInt()
        {
            var e1 = Emit<Func<int>>.NewDynamicMethod("E1");
            e1.LoadConstant(1);
            e1.ConvertToNativeInt();
            e1.LoadConstant(3);
            e1.ConvertToNativeInt();
            e1.Add();
            e1.ConvertToInt32();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(4, d1());
        }

        [TestMethod]
        public void IntPointer()
        {
            var e1 = Emit<Func<int, int>>.NewDynamicMethod("E1");
            e1.LoadArgumentAddress(0);
            e1.LoadConstant(2);
            e1.Add();
            e1.ConvertToInt32();
            e1.Return();

            var d1 = e1.CreateDelegate();

            try
            {
                var x = d1(3);

                Assert.IsTrue(x != 0);
            }
            catch
            {
                Assert.Fail("ShouldBeLegal");
            }
        }

        [TestMethod]
        public void PointerInt()
        {
            var e1 = Emit<Func<int, int>>.NewDynamicMethod("E1");
            e1.LoadConstant(2);
            e1.LoadArgumentAddress(0);
            e1.Add();
            e1.ConvertToInt32();
            e1.Return();

            var d1 = e1.CreateDelegate();

            try
            {
                var x = d1(3);

                Assert.IsTrue(x != 0);
            }
            catch
            {
                Assert.Fail("ShouldBeLegal");
            }
        }

        [TestMethod]
        public void PointerNativeInt()
        {
            var e1 = Emit<Func<int, int>>.NewDynamicMethod("E1");
            e1.LoadConstant(2);
            e1.ConvertToNativeInt();
            e1.LoadArgumentAddress(0);
            e1.Add();
            e1.ConvertToInt32();
            e1.Return();

            var d1 = e1.CreateDelegate();

            try
            {
                var x = d1(3);

                Assert.IsTrue(x != 0);
            }
            catch
            {
                Assert.Fail("ShouldBeLegal");
            }
        }

        [TestMethod]
        public void LongLong()
        {
            var e1 = Emit<Func<long, long, long>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Add();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(2 * ((long)uint.MaxValue), d1(uint.MaxValue, uint.MaxValue));
        }

        [TestMethod]
        public void FloatFloat()
        {
            var e1 = Emit<Func<float, float, float>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Add();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(3.14f + 1.59f, d1(3.14f, 1.59f));
        }

        [TestMethod]
        public void DoubleDouble()
        {
            var e1 = Emit<Func<double, double, double>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Add();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(3.14 + 1.59, d1(3.14, 1.59));
        }

        [TestMethod]
        public void Overflow()
        {
            var e1 = Emit<Func<double, double, double>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.AddOverflow();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(3.14 + 1.59, d1(3.14, 1.59));
        }

        [TestMethod]
        public void UnsignedOverflow()
        {
            var e1 = Emit<Func<double, double, double>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.UnsignedAddOverflow();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(3.14 + 1.59, d1(3.14, 1.59));
        }
    }
}
