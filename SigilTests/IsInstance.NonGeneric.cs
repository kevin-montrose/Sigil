using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    public partial class IsInstance
    {
        [TestMethod]
        public void NotElidedNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(string), new [] { typeof(string) });
            e1.LoadArgument(0);
            e1.IsInstance<string>();
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate<Func<string, string>>(out instrs, Sigil.OptimizationOptions.None);

            Assert.AreEqual("hello", d1("hello"));

            Assert.IsTrue(instrs.Contains("isinst"));
        }

        [TestMethod]
        public void ElidedNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(string), new [] { typeof(string) });
            e1.LoadArgument(0);
            e1.IsInstance<string>();
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate<Func<string, string>>(out instrs);

            Assert.AreEqual("hello", d1("hello"));

            Assert.IsFalse(instrs.Contains("isinst"));
        }

        [TestMethod]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(string), new [] { typeof(object) });
            e1.LoadArgument(0);
            e1.IsInstance<string>();
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate<Func<object, string>>(out instrs);

            Assert.AreEqual(null, d1(123));
            Assert.AreEqual("hello", d1("hello"));

            Assert.IsTrue(instrs.Contains("isinst"));
        }
    }
}
