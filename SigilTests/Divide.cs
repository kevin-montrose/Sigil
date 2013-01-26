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
    public class Divide
    {
        [TestMethod]
        public void Simple()
        {
            var e1 = Emit<Func<double, double, double>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Divide();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(3.14 / 1.59, d1(3.14, 1.59));
        }

        [TestMethod]
        public void Unsigned()
        {
            var e1 = Emit<Func<uint, uint, uint>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.UnsignedDivide();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(uint.MaxValue / ((uint)1234), d1(uint.MaxValue, (uint)1234));
        }
    }
}
