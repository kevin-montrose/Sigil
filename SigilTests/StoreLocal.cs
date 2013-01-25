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
    public class StoreLocal
    {
        [TestMethod]
        public void Int()
        {
            var e1 = Emit<Func<int>>.NewDynamicMethod("E1");

            var bar = e1.DeclareLocal<int>("bar");
            e1.LoadConstant(12);
            e1.LoadConstant(34);
            e1.Add();
            e1.StoreLocal(bar);
            e1.LoadLocal(bar);
            e1.Return();

            var del = e1.CreateDelegate();

            Assert.AreEqual(46, del());
        }
    }
}
