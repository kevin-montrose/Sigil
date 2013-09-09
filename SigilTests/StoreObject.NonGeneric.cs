using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    public partial class StoreObject
    {
        [TestMethod]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(DateTime), new [] { typeof(DateTime) });
            var l = e1.DeclareLocal<DateTime>();
            e1.LoadLocalAddress(l);
            e1.LoadArgument(0);
            e1.StoreObject<DateTime>();
            e1.LoadLocal(l);
            e1.Return();

            var d1 = e1.CreateDelegate<Func<DateTime, DateTime>>();

            var now = DateTime.UtcNow;

            Assert.AreEqual(now, d1(now));
        }
    }
}
