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
    public class Switch
    {
        [TestMethod]
        public void Simple()
        {
            var e1 = Emit<Func<int, bool>>.NewDynamicMethod();
            var l = e1.DefineLabel();

            e1.LoadArgument(0);
            e1.Switch(l);
            e1.LoadConstant(0);
            e1.Return();

            e1.MarkLabel(l);
            e1.LoadConstant(1);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.IsTrue(d1(0));
            Assert.IsFalse(d1(1));
        }
    }
}
