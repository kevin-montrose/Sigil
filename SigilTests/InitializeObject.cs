using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestClass, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class InitializeObject
    {
        [TestMethod]
        public void Simple()
        {
            var e1 = Emit<Func<DateTime, DateTime>>.NewDynamicMethod();
            e1.LoadArgumentAddress(0);
            e1.InitializeObject<DateTime>();
            e1.LoadArgument(0);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(new DateTime(), d1(DateTime.Now));
        }
    }
}
