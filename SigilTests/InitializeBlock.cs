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
    public class InitializeBlock
    {
        [TestMethod]
        public void Simple()
        {
            var e1 = Emit<Action<byte[]>>.NewDynamicMethod();

            e1.LoadArgument(0);
            e1.LoadConstant(0);
            e1.LoadElementAddress<byte>();
            e1.LoadConstant(101);
            e1.LoadArgument(0);
            e1.LoadLength();
            e1.InitializeBlock();
            e1.Return();

            var d1 = e1.CreateDelegate();

            var b = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            d1(b);

            Assert.IsTrue(b.All(x => x == 101));
        }

        [TestMethod]
        public void Volatile()
        {
            var e1 = Emit<Action<byte[]>>.NewDynamicMethod();

            e1.LoadArgument(0);
            e1.LoadConstant(0);
            e1.LoadElementAddress<byte>();
            e1.LoadConstant(101);
            e1.LoadArgument(0);
            e1.LoadLength();
            e1.InitializeBlock(isVolatile: true);
            e1.Return();

            var d1 = e1.CreateDelegate();

            var b = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            d1(b);

            Assert.IsTrue(b.All(x => x == 101));
        }
    }
}
