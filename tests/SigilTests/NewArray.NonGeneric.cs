using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class NewArray
    {
        [Fact]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int[]), Type.EmptyTypes);
            e1.LoadConstant(128);
            e1.NewArray<int>();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int[]>>();

            var x = d1();

            Assert.Equal(128, x.Length);
        }
    }
}
