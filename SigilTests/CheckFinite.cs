using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestClass]
    public class CheckFinite
    {
        [TestMethod]
        public void Simple()
        {
            var e1 = Emit<Action<double>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.CheckFinite();
            e1.Pop();
            e1.Return();

            var d1 = e1.CreateDelegate();

            d1(123);

            try
            {
                d1(double.PositiveInfinity);
                Assert.Fail("Should have thrown");
            }
            catch (ArithmeticException e)
            {
                Assert.IsNotNull(e);
            }
        }
    }
}
