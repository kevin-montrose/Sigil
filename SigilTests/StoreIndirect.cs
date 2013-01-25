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
   public class StoreIndirect
    {
        [TestMethod]
        public void Simple()
        {
            var e1 = Emit<Func<double>>.NewDynamicMethod("E1");
            var a = e1.DeclareLocal<double>("a");
            e1.LoadLocalAddress(a);
            e1.LoadConstant(3.1415926);
            e1.StoreIndirect<double>();
            e1.LoadLocal(a);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(3.1415926, d1());
        }
    }
}
