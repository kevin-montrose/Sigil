using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class LoadFieldAddress
    {
        private class TestObj
        {
            public static int Static;
            public int Instance;
        }

        [Fact]
        public void Instance()
        {
            var e1 = Emit<Func<TestObj, int>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.LoadFieldAddress(typeof(TestObj).GetField("Instance"));
            e1.LoadIndirect<int>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(10, d1(new TestObj { Instance = 10 }));
        }

        [Fact]
        public void Static()
        {
            var e1 = Emit<Func<int>>.NewDynamicMethod();
            e1.LoadFieldAddress(typeof(TestObj).GetField("Static"));
            e1.LoadIndirect<int>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            TestObj.Static = 20;
            Assert.Equal(20, d1());
        }
    }
}
