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
    public class Boxing
    {
        [TestMethod]
        public void Boolean()
        {
            var e1 = Emit<Func<bool, object>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.Box<bool>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual((object)true, d1(true));
            Assert.AreEqual((object)false, d1(false));
        }

        [TestMethod]
        public void Simple()
        {
            var e1 = Emit<Func<object>>.NewDynamicMethod("E1");
            e1.LoadConstant(123);
            e1.Box<byte>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual("123", d1().ToString());

            var e2 = Emit<Func<object>>.NewDynamicMethod("E2");
            e2.LoadConstant(566);
            e2.Box<byte>();
            e2.Return();

            var d2 = e2.CreateDelegate();

            Assert.AreEqual("54", d2().ToString());
        }
    }
}
