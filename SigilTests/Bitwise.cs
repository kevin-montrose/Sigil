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
    public class Bitwise
    {
        [TestMethod]
        public void And()
        {
            var e1 = Emit<Func<byte, byte, int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.And();
            e1.Return();

            var d1 = e1.CreateDelegate();

            byte a = 123, b = 200;

            Assert.AreEqual(a & b, d1(a, b));
        }

        [TestMethod]
        public void Or()
        {
            var e1 = Emit<Func<byte, byte, int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Or();
            e1.Return();

            var d1 = e1.CreateDelegate();

            byte a = 123, b = 200;

            Assert.AreEqual(a | b, d1(a, b));
        }

        [TestMethod]
        public void Xor()
        {
            var e1 = Emit<Func<byte, byte, int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Xor();
            e1.Return();

            var d1 = e1.CreateDelegate();

            byte a = 123, b = 200;

            Assert.AreEqual(a ^ b, d1(a, b));
        }

        [TestMethod]
        public void Not()
        {
            var e1 = Emit<Func<byte, int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.Not();
            e1.Return();

            var d1 = e1.CreateDelegate();

            byte a = 123;

            Assert.AreEqual(~a, d1(a));
        }
    }
}
