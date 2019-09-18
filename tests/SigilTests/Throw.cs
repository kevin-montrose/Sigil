using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Throw
    {
        [Fact]
        public void Simple()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            e1.LoadConstant("Hello!");
            e1.NewObject<Exception, string>();
            e1.Throw();

            var d1 = e1.CreateDelegate();

            var ex = Assert.Throws<Exception>(() => d1());
            Assert.Equal("Hello!", ex.Message);
        }
    }
}
