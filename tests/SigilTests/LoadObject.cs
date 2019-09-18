using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class LoadObject
    {
        [Fact]
        public void Simple()
        {
            var e1 = Emit<Func<DateTime, DateTime>>.NewDynamicMethod();
            e1.LoadArgumentAddress(0);
            e1.LoadObject<DateTime>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            var now = DateTime.UtcNow;

            Assert.Equal(now, d1(now));
        }
    }
}
