using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    public partial class Negate
    {
        [TestMethod]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(int) }, "E1");
            e1.LoadArgument(0);
            e1.Negate();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int, int>>();

            Assert.AreEqual(-123, d1(123));
        }

        [TestMethod]
        public void LongNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(long), new [] { typeof(long) }, "E1");
            e1.LoadArgument(0);
            e1.Negate();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<long, long>>();

            Assert.AreEqual(-123L, d1(123));
        }
    }
}
