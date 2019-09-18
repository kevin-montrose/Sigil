using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class IsInstance
    {
        [Fact]
        public void NotElidedNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(string), new [] { typeof(string) });
            e1.LoadArgument(0);
            e1.IsInstance<string>();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<string, string>>(out string instrs, Sigil.OptimizationOptions.None);

            Assert.Equal("hello", d1("hello"));
            Assert.Contains("isinst", instrs);
        }

        [Fact]
        public void ElidedNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(string), new [] { typeof(string) });
            e1.LoadArgument(0);
            e1.IsInstance<string>();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<string, string>>(out string instrs);

            Assert.Equal("hello", d1("hello"));
            Assert.DoesNotContain("isinst", instrs);
        }

        [Fact]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(string), new [] { typeof(object) });
            e1.LoadArgument(0);
            e1.IsInstance<string>();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<object, string>>(out string instrs);

            Assert.Null(d1(123));
            Assert.Equal("hello", d1("hello"));
            Assert.Contains("isinst", instrs);
        }
    }
}
