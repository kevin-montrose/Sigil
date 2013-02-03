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
    public class StoreObject
    {
        [TestMethod]
        public void Simple()
        {
            var e1 = Emit<Func<DateTime, DateTime>>.NewDynamicMethod();
            var l = e1.DeclareLocal<DateTime>();
            e1.LoadLocalAddress(l);
            e1.LoadArgument(0);
            e1.StoreObject<DateTime>();
            e1.LoadLocal(l);
            e1.Return();

            var d1 = e1.CreateDelegate();

            var now = DateTime.UtcNow;

            Assert.AreEqual(now, d1(now));
        }
    }
}
