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
    public class SizeOf
    {
        [TestMethod]
        public void Simple()
        {
            var e1 = Emit<Func<int>>.NewDynamicMethod("E1").AsShorthand();
            e1.Sizeof<int>();
            e1.Ret();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(sizeof(int), d1());
        }
    }
}
