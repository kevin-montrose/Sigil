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
    public class Branches
    {
        [TestMethod]
        public void Br()
        {
            var e1 = Emit<Func<int>>.NewDynamicMethod("E1");

            var after = e1.CreateLabel("after");
            e1.LoadConstant(456);
            e1.Branch(after);
            
            e1.LoadConstant(111);
            e1.Add();

            e1.MarkLabel(after);
            e1.Return();

            var del = e1.CreateDelegate();

            Assert.AreEqual(456, del());
        }
    }
}
