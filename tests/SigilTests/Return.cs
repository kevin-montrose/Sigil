using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Return
    {
        [Fact]
        public void NonTerminal()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            var l1 = e1.DefineLabel("l1");
            var l2 = e1.DefineLabel("l2");

            e1.Branch(l1);

            e1.MarkLabel(l2);
            e1.Return();

            e1.MarkLabel(l1);
            e1.Branch(l2);

            var d1 = e1.CreateDelegate();
            d1();
        }

        [Fact]
        public void Empty()
        {
            var il = Emit<Action>.NewDynamicMethod("Empty");
            il.Return();

            var del = il.CreateDelegate();

            del();
        }

        [Fact]
        public void Constant()
        {
            var il = Emit<Func<int>>.NewDynamicMethod("Constant");
            il.LoadConstant(123);
            il.Return();

            var del = il.CreateDelegate();

            Assert.Equal(123, del());
        }
    }
}
