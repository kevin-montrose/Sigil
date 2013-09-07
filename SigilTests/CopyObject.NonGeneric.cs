using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    public partial class CopyObject
    {
        [TestMethod]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(DateTime), new [] { typeof(DateTime), typeof(DateTime) });
            e1.LoadArgumentAddress(1);
            e1.LoadArgumentAddress(0);
            e1.CopyObject<DateTime>();
            e1.LoadArgument(1);
            e1.Return();

            var d1 = e1.CreateDelegate<Func<DateTime, DateTime, DateTime>>();

            var now = DateTime.UtcNow;

            Assert.AreEqual(now, d1(now, DateTime.MinValue));
        }
    }
}
