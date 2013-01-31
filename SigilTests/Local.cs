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
    public class Local
    {
        [TestMethod]
        public void Name()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            var local = e1.DeclareLocal<int>("local");

            Assert.AreEqual("System.Int32 local", local.ToString());
        }
    }
}
