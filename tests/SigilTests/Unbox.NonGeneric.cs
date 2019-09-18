using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Unbox
    {
        [Fact]
        public void JustUnboxNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(object) });
            e1.LoadArgument(0);
            e1.Unbox<int>();
            e1.LoadIndirect<int>();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<object, int>>();

            Assert.Equal(1234567, d1(1234567));
        }

        [Fact]
        public void UnboxAnyNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(object) });
            e1.LoadArgument(0);
            e1.UnboxAny<int>();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<object, int>>();

            Assert.Equal(1234567, d1(1234567));
        }
    }
}
