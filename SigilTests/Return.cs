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
    public class Return
    {
        [TestMethod]
        public void Empty()
        {
            var il = Emit<Action>.NewDynamicMethod("Empty");
            il.Return();

            var del = il.CreateDelegate();

            del();
        }

        [TestMethod]
        public void Constant()
        {
            var il = Emit<Func<int>>.NewDynamicMethod("Constant");
            il.LoadConstant(123);
            il.Return();

            var del = il.CreateDelegate();

            Assert.AreEqual(123, del());
        }
    }
}
