using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    public partial class Remainder
    {
        [TestMethod]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(int), typeof(int) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Remainder();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int, int, int>>();

            Assert.AreEqual(8675309 % 314, d1(8675309, 314));
        }

        [TestMethod]
        public void UnsignedNonGneeric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(uint), new [] { typeof(uint), typeof(uint) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.UnsignedRemainder();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<uint, uint, uint>>();

            Assert.AreEqual(uint.MaxValue % ((uint)1234), d1(uint.MaxValue, (uint)1234));
        }
    }
}
