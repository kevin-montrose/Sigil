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
    public class Add
    {
        [TestMethod]
        public void IntInt()
        {
            var il = Emit<Func<int>>.NewDynamicMethod("IntInt");
            il.LoadConstant(1);
            il.LoadConstant(2);
            il.Add();
            il.Return();

            var del = il.CreateDelegate();
            Assert.AreEqual(3, del());
        }
    }
}
