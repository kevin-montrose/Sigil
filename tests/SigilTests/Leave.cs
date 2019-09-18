using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Leave
    {
        [Fact]
        public void Simple()
        {
            var e1 = Emit<Func<int, int>>.NewDynamicMethod();
            var t = e1.BeginExceptionBlock();
            var l = e1.DefineLabel("dead_code");
            e1.Leave(t.Label);
            e1.MarkLabel(l);
            e1.LoadArgument(0);
            e1.LoadConstant(1);
            e1.Add();
            e1.StoreArgument(0);
            e1.Branch(l);
            var c = e1.BeginCatchAllBlock(t);
            e1.Pop();
            e1.EndCatchBlock(c);
            e1.EndExceptionBlock(t);
            e1.LoadArgument(0);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(1, d1(1));
        }
    }
}
