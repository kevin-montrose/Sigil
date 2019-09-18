using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Switch
    {
        [Fact]
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

                var ex = Assert.Throws<Sigil.SigilVerificationException>(() => e1.CreateDelegate<Action>());

                var f = ex.GetDebugInfo();
                Assert.Equal("All execution paths must end with Return", ex.Message);
                Assert.Equal("Bad Path\r\n========\r\n__start\r\nl1\r\n\r\nBad Path\r\n========\r\n__start\r\nl2\r\n\r\nBad Path\r\n========\r\n__start\r\nl3\r\n\r\nInstructions\r\n============\r\nldc.i4.0\r\nswitch l1, l2, l3, l4\r\n\r\nl4:\r\nret\r\n\r\nl1:\r\n\r\nl2:\r\n\r\nl3:\r\n", f);
            }
        }

        [Fact]
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

            Assert.True(d1(0));
            Assert.False(d1(1));
        }
    }
}
