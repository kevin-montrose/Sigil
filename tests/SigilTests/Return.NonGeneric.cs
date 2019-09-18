using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Return
    {
        [Fact]
        public void NonTerminalNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(void), Type.EmptyTypes);
            var l1 = e1.DefineLabel("l1");
            var l2 = e1.DefineLabel("l2");

            e1.Branch(l1);

            e1.MarkLabel(l2);
            e1.Return();

            e1.MarkLabel(l1);
            e1.Branch(l2);

            var d1 = (Action)e1.CreateDelegate(typeof(Action));
            d1();
        }

        [Fact]
        public void EmptyNonGeneric()
        {
            var il = Emit.NewDynamicMethod(typeof(void), Type.EmptyTypes,"Empty");
            il.Return();

            var del = (Action)il.CreateDelegate(typeof(Action));

            del();
        }

        [Fact]
        public void ConstantNonGeneric()
        {
            var il = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes,"Constant");
            il.LoadConstant(123);
            il.Return();

            var del = (Func<int>)il.CreateDelegate(typeof(Func<int>));

            Assert.Equal(123, del());
        }
    }
}
