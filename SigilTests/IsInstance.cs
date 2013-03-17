using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestClass, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class IsInstance
    {
        [TestMethod]
        public void NotElided()
        {
            var e1 = Emit<Func<string, string>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.IsInstance<string>();
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate(out instrs, OptimizationOptions.None);

            Assert.AreEqual("hello", d1("hello"));

            Assert.IsTrue(instrs.Contains("isinst"));
        }

        [TestMethod]
        public void Elided()
        {
            var e1 = Emit<Func<string, string>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.IsInstance<string>();
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate(out instrs);

            Assert.AreEqual("hello", d1("hello"));

            Assert.IsFalse(instrs.Contains("isinst"));
        }

        [TestMethod]
        public void Simple()
        {
            var e1 = Emit<Func<object, string>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.IsInstance<string>();
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate(out instrs);

            Assert.AreEqual(null, d1(123));
            Assert.AreEqual("hello", d1("hello"));

            Assert.IsTrue(instrs.Contains("isinst"));
        }
    }
}
