using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class IsInstance
    {
        [Fact]
        public void NotElided()
        {
            var e1 = Emit<Func<string, string>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.IsInstance<string>();
            e1.Return();

            var d1 = e1.CreateDelegate(out string instrs, OptimizationOptions.None);

            Assert.Equal("hello", d1("hello"));
            Assert.Contains("isinst", instrs);
        }

        [Fact]
        public void Elided()
        {
            var e1 = Emit<Func<string, string>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.IsInstance<string>();
            e1.Return();

            var d1 = e1.CreateDelegate(out string instrs);

            Assert.Equal("hello", d1("hello"));
            Assert.DoesNotContain("isinst", instrs);
        }

        [Fact]
        public void Simple()
        {
            var e1 = Emit<Func<object, string>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.IsInstance<string>();
            e1.Return();

            var d1 = e1.CreateDelegate(out string instrs);

            Assert.Null(d1(123));
            Assert.Equal("hello", d1("hello"));

            Assert.Contains("isinst", instrs);
        }
    }
}
