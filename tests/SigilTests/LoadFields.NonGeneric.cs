using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class LoadFields
    {
        [Fact]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(A) }, "E1");
            e1.LoadArgument(0);
            e1.LoadField(typeof(A).GetField("X"));
            e1.Return();

            var d1 = e1.CreateDelegate<Func<A, int>>();

            Assert.Equal(255, d1(new A { X = 255 }));

            var e2 = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes, "E2");
            e2.LoadField(typeof(A).GetField("Y"));
            e2.Return();

            var d2 = e2.CreateDelegate<Func<int>>();

            A.Y = 31415926;

            Assert.Equal(31415926, d2());
        }
    }
}
