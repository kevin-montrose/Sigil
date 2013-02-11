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
    public class Unbox
    {
        [TestMethod]
        public void JustUnbox()
        {
            var e1 = Emit<Func<object, int>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.Unbox<int>();
            e1.LoadIndirect<int>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(1234567, d1(1234567));
        }

        [TestMethod]
        public void UnboxAny()
        {
            var e1 = Emit<Func<object, int>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.UnboxAny<int>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(1234567, d1(1234567));
        }
    }
}
