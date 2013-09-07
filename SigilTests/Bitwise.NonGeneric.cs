using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    public partial class Bitwise
    {
        [TestMethod]
        public void AndNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(byte), typeof(byte) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.And();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<byte, byte, int>>();

            byte a = 123, b = 200;

            Assert.AreEqual(a & b, d1(a, b));
        }

        [TestMethod]
        public void OrNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(byte), typeof(byte) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Or();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<byte, byte, int>>();

            byte a = 123, b = 200;

            Assert.AreEqual(a | b, d1(a, b));
        }

        [TestMethod]
        public void XorNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(byte), typeof(byte) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Xor();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<byte, byte, int>>();

            byte a = 123, b = 200;

            Assert.AreEqual(a ^ b, d1(a, b));
        }

        [TestMethod]
        public void NotNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(byte) }, "E1");
            e1.LoadArgument(0);
            e1.Not();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<byte, int>>();

            byte a = 123;

            Assert.AreEqual(~a, d1(a));
        }
    }
}
