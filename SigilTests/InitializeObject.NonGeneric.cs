using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    public partial class InitializeObject
    {
        [TestMethod]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(DateTime), new [] { typeof(DateTime) });
            e1.LoadArgumentAddress(0);
            e1.InitializeObject<DateTime>();
            e1.LoadArgument(0);
            e1.Return();

            var d1 = e1.CreateDelegate<Func<DateTime, DateTime>>();

            Assert.AreEqual(new DateTime(), d1(DateTime.Now));
        }
    }
}
