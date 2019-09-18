using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class ReThrow
    {
        [Fact]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(void), Type.EmptyTypes);
            var m = typeof(ReThrow).GetMethod("AlwaysThrows");

            var t = e1.BeginExceptionBlock();
            e1.Call(m);
            var c = e1.BeginCatchAllBlock(t);
            e1.Pop();
            e1.ReThrow();
            e1.EndCatchBlock(c);
            e1.EndExceptionBlock(t);

            e1.Return();

            var d1 = e1.CreateDelegate<Action>();

            var ex = Assert.Throws<Exception>(() => d1());
            Assert.Equal("Hello World", ex.Message);
        }
    }
}
