using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Throw
    {
        [Fact]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(void), Type.EmptyTypes);
            e1.LoadConstant("Hello!");
            e1.NewObject<Exception, string>();
            e1.Throw();

            var d1 = e1.CreateDelegate<Action>();

            var ex = Assert.Throws<Exception>(() => d1());
            Assert.Equal("Hello!", ex.Message);
        }
    }
}
