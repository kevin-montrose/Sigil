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
    public class LoadElement
    {
        [TestMethod]
        public void Simple()
        {
            var e1 = Emit<Func<int[], int, int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.LoadElement();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(1, d1(new[] { 3, 1, 2 }, 1));
            Assert.AreEqual(111, d1(new[] { 111, 2, 3 }, 0));

            var e2 = Emit<Func<string[], string>>.NewDynamicMethod("E2");
            e2.LoadArgument(0);
            e2.LoadConstant(1);
            e2.LoadElement();
            e2.Return();

            var d2 = e2.CreateDelegate();

            Assert.AreEqual("hello", d2(new[] { "world", "hello" }));
        }
    }
}
