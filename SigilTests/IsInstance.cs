using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestClass, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class IsInstance
    {
        [TestMethod]
        public void Simple()
        {
            var e1 = Emit<Func<object, string>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.IsInstance<string>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(null, d1(123));
            Assert.AreEqual("hello", d1("hello"));
        }
    }
}
