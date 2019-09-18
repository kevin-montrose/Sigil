using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Labels
    {
        [Fact]
        public void LookupNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(bool), new [] { typeof(bool) });
            var f = e1.DefineLabel("false");

            Assert.True(f == e1.Labels["false"]);

            e1.LoadArgument(0);
            e1.BranchIfFalse(f);
            e1.LoadConstant(true);
            e1.Return();

            e1.MarkLabel(e1.Labels["false"]);
            e1.LoadConstant(false);
            e1.Return();

            var d1 = e1.CreateDelegate<Func<bool, bool>>();

            Assert.True(d1(true));
            Assert.False(d1(false));
        }
    }
}
