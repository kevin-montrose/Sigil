using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class StoreObject
    {
        [Fact]
        public void Simple()
        {
            var e1 = Emit<Func<DateTime, DateTime>>.NewDynamicMethod();
            var l = e1.DeclareLocal<DateTime>();
            e1.LoadLocalAddress(l);
            e1.LoadArgument(0);
            e1.StoreObject<DateTime>();
            e1.LoadLocal(l);
            e1.Return();

            var d1 = e1.CreateDelegate();

            var now = DateTime.UtcNow;

            Assert.Equal(now, d1(now));
        }
    }
}
