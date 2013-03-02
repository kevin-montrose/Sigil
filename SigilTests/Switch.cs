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
        public void ReturnChecking()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var l1 = e1.DefineLabel("l1");
                var l2 = e1.DefineLabel("l2");

                e1.LoadConstant(0);
                e1.Switch(l1, l2);

                e1.Return();

                e1.MarkLabel(l1);
                e1.Return();

                e1.MarkLabel(l2);
                e1.Branch(l1);

                var d1 = e1.CreateDelegate();
                d1();
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var l1 = e1.DefineLabel("l1");
                var l2 = e1.DefineLabel("l2");
                var l3 = e1.DefineLabel("l3");
                var l4 = e1.DefineLabel("l4");

                e1.LoadConstant(0);
                e1.Switch(l1, l2, l3, l4);

                e1.MarkLabel(l4);
                e1.Return();

                e1.MarkLabel(l1);
                e1.MarkLabel(l2);
                e1.MarkLabel(l3);

                try
                {
                    e1.CreateDelegate();

                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    var f = e.GetDebugInfo();
                    Assert.AreEqual("All execution paths must end with Return", e.Message);
                    Assert.AreEqual("Bad Path\r\n========\r\n__start\r\nl1\r\n\r\nBad Path\r\n========\r\n__start\r\nl2\r\n\r\nBad Path\r\n========\r\n__start\r\nl3\r\n\r\n", f);
                }
            }
        }

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
