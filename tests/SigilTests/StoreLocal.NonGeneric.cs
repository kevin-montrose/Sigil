using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class StoreLocal
    {
        [Fact]
        public void IntNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes, "E1");

            var bar = e1.DeclareLocal<int>("bar");
            e1.LoadConstant(12);
            e1.LoadConstant(34);
            e1.Add();
            e1.StoreLocal(bar);
            e1.LoadLocal(bar);
            e1.Return();

            var del = e1.CreateDelegate<Func<int>>();

            Assert.Equal(46, del());
        }
    }
}
