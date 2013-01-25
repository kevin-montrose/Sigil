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
    public class Jump
    {
        private static int SimpleSet;
        public static void SimpleSetM(int x)
        {
            SimpleSet = x;
        }

        [TestMethod]
        public void Simple()
        {
            var simpleSet = typeof(Jump).GetMethod("SimpleSetM");

            var e1 = Emit<Action<int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadConstant(int.MaxValue);
            e1.Xor();
            e1.StoreArgument(0);
            e1.Jump(simpleSet);
            e1.Return();

            var d1 = e1.CreateDelegate();

            d1(1234);
            Assert.AreEqual(1234 ^ int.MaxValue, SimpleSet);
            d1(777);
            Assert.AreEqual(777 ^ int.MaxValue, SimpleSet);
            d1(0x12345678);
            Assert.AreEqual(0x12345678 ^ int.MaxValue, SimpleSet);
        }
    }
}
