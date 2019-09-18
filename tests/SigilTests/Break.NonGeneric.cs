using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Break
    {
        [Fact]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(void), Type.EmptyTypes, "E1");
            e1.LoadConstant(123);
            e1.Break();
            e1.Pop();
            e1.Return();

            var d1 = e1.CreateDelegate<Action>();

            d1();
        }
    }
}
