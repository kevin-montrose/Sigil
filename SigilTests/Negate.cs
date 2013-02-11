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
    public class Negate
    {
        [TestMethod]
        public void Simple()
        {
            var e1 = Emit<Func<int, int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.Negate();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(-123, d1(123));
        }

        [TestMethod]
        public void Long()
        {
            var e1 = Emit<Func<long, long>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.Negate();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(-123L, d1(123));
        }
    }
}
