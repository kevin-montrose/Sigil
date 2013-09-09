using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    public partial class Switch
    {
        [TestMethod]
        public void ReturnCheckingNonGeneric()
        {
            {
                var e1 = Emit.NewDynamicMethod(typeof(void), Type.EmptyTypes);
                var l1 = e1.DefineLabel("l1");
                var l2 = e1.DefineLabel("l2");

                e1.LoadConstant(0);
                e1.Switch(l1, l2);

                e1.Return();

                e1.MarkLabel(l1);
                e1.Return();

                e1.MarkLabel(l2);
                e1.Branch(l1);

                var d1 = e1.CreateDelegate<Action>();
                d1();
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(void), Type.EmptyTypes);
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
                    e1.CreateDelegate<Action>();

                    Assert.Fail();
                }
                catch (Sigil.SigilVerificationException e)
                {
                    var f = e.GetDebugInfo();
                    Assert.AreEqual("All execution paths must end with Return", e.Message);
                    var b = "Bad Path\r\n========\r\n__start\r\nl1\r\n\r\nBad Path\r\n========\r\n__start\r\nl2\r\n\r\nBad Path\r\n========\r\n__start\r\nl3\r\n\r\nInstructions\r\n============\r\nldc.i4.0\r\nswitch l1, l2, l3, l4\r\n\r\nl4:\r\nret\r\n\r\nl1:\r\n\r\nl2:\r\n\r\nl3:\r\n";
                    Assert.AreEqual(b, f);
                }
            }
        }

        [TestMethod]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(bool), new [] { typeof(int) });
            var l = e1.DefineLabel();

            e1.LoadArgument(0);
            e1.Switch(l);
            e1.LoadConstant(0);
            e1.Return();

            e1.MarkLabel(l);
            e1.LoadConstant(1);
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int, bool>>();

            Assert.IsTrue(d1(0));
            Assert.IsFalse(d1(1));
        }
    }
}
