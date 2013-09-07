using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    public partial class Divide
    {
        [TestMethod]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(double), new [] { typeof(double), typeof(double) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Divide();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<double, double, double>>();

            Assert.AreEqual(3.14 / 1.59, d1(3.14, 1.59));
        }

        [TestMethod]
        public void UnsignedNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(uint), new [] { typeof(uint), typeof(uint) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.UnsignedDivide();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<uint, uint, uint>>();

            Assert.AreEqual(uint.MaxValue / ((uint)1234), d1(uint.MaxValue, (uint)1234));
        }
    }
}
