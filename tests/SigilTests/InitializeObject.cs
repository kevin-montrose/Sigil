using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class InitializeObject
    {
        [Fact]
        public void Simple()
        {
            var e1 = Emit<Func<DateTime, DateTime>>.NewDynamicMethod();
            e1.LoadArgumentAddress(0);
            e1.InitializeObject<DateTime>();
            e1.LoadArgument(0);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(new DateTime(), d1(DateTime.Now));
        }
    }
}
