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
    public class CastClass
    {
        [TestMethod]
        public void Simple()
        {
            var e1 = Emit<Func<object, string>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.CastClass<string>();
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate(out instrs);

            Assert.AreEqual(null, d1(null));
            Assert.AreEqual("hello", d1("hello"));
        }

        [TestMethod]
        public void Trivial()
        {
            var e1 = Emit<Action<object>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.CastClass<object>();
            e1.Pop();
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate(out instrs);

            d1(null);

            Assert.IsFalse(instrs.Contains("castclass"));
        }

        [TestMethod]
        public void NullTrivial()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            e1.LoadNull();
            e1.CastClass<object>();
            e1.Pop();
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate(out instrs);

            d1();

            Assert.IsFalse(instrs.Contains("castclass"));
        }
    }
}
