using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestClass, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class CopyObject
    {
        [TestMethod]
        public void Simple()
        {
            var e1 = Emit<Func<DateTime, DateTime, DateTime>>.NewDynamicMethod();
            e1.LoadArgumentAddress(1);
            e1.LoadArgumentAddress(0);
            e1.CopyObject<DateTime>();
            e1.LoadArgument(1);
            e1.Return();

            var d1 = e1.CreateDelegate();

            var now = DateTime.UtcNow;

            Assert.AreEqual(now, d1(now, DateTime.MinValue));
        }
    }
}
