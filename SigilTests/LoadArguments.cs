using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sigil;

namespace SigilTests
{
    [TestClass]
    public class LoadArguments
    {
        [TestMethod]
        public void Simple()
        {
            var e1 = Emit<Func<int, string>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.Box<int>();
            e1.CallVirtual(typeof(object).GetMethod("ToString"));
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual("31415", d1(31415));
        }
    }
}
