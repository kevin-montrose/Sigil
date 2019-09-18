using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class LoadFieldAddress
    {
        [Fact]
        public void InstanceNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(TestObj) });
            e1.LoadArgument(0);
            e1.LoadFieldAddress(typeof(TestObj).GetField("Instance"));
            e1.LoadIndirect<int>();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<TestObj, int>>();

            Assert.Equal(10, d1(new TestObj { Instance = 10 }));
        }

        [Fact]
        public void StaticNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes);
            e1.LoadFieldAddress(typeof(TestObj).GetField("Static"));
            e1.LoadIndirect<int>();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int>>();

            TestObj.Static = 20;
            Assert.Equal(20, d1());
        }
    }
}
