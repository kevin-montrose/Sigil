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
    public class LoadlLocalAddress
    {
        [TestMethod]
        public void Simple()
        {
            var e1 = Emit<Func<int>>.NewDynamicMethod("E1");
            var a = e1.DeclareLocal<int>("a");
            e1.LoadConstant(123);
            e1.StoreLocal(a);
            e1.LoadLocalAddress(a);
            e1.LoadIndirect<int>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(123, d1());
        }
    }
}
