using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class ReThrow
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1013:Public method should be marked as test", Justification = "It needs to be public")]
        public static void AlwaysThrows()
        {
            throw new Exception("Hello World");
        }

        [Fact]
        public void Simple()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            var m = typeof(ReThrow).GetMethod("AlwaysThrows");

            var t = e1.BeginExceptionBlock();
            e1.Call(m);
            var c = e1.BeginCatchAllBlock(t);
            e1.ReThrow();
            e1.EndCatchBlock(c);
            e1.EndExceptionBlock(t);

            e1.Return();

            var d1 = e1.CreateDelegate();

            var ex = Assert.Throws<Exception>(() => d1());
            Assert.Equal("Hello World", ex.Message);
        }
    }
}
