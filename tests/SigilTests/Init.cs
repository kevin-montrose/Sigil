using System;
using Sigil;
using Xunit;

namespace SigilTests
{
    public class Init
    {
        private delegate string FooDelegate(object p);

        [Fact]
        public void EmitDelegateParsing()
        {
            var e1 = Emit<Action<int, string>>.NewDynamicMethod("E1");
            var e2 = Emit<Func<int>>.NewDynamicMethod("E2");
            var e3 = Emit<FooDelegate>.NewDynamicMethod("E3");

            Assert.NotNull(e1);
            Assert.NotNull(e2);
            Assert.NotNull(e3);

            var ex = Assert.Throws<ArgumentException>(() => Emit<string>.NewDynamicMethod("E4"));
            Assert.Equal("DelegateType must be a delegate, found System.String", ex.Message);
        }

        [Fact]
        public void EmitSingleReturn()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");
            e1.Return();

            var del = e1.CreateDelegate();

            var ex = Assert.Throws<InvalidOperationException>(() => e1.LoadConstant(100));
            Assert.Equal("Cannot modify Emit after a delegate has been generated from it", ex.Message);

            del();
        }
    }
}
