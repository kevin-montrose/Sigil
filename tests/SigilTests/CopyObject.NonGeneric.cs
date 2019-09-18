using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class CopyObject
    {
        [Fact]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(DateTime), new [] { typeof(DateTime), typeof(DateTime) });
            e1.LoadArgumentAddress(1);
            e1.LoadArgumentAddress(0);
            e1.CopyObject<DateTime>();
            e1.LoadArgument(1);
            e1.Return();

            var d1 = e1.CreateDelegate<Func<DateTime, DateTime, DateTime>>();

            var now = DateTime.UtcNow;

            Assert.Equal(now, d1(now, DateTime.MinValue));
        }
    }
}
