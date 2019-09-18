using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class LoadElementAddress
    {
        [Fact]
        public void Simple()
        {
            var e1 = Emit<Func<int[], int, int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.LoadElementAddress<int>();
            e1.LoadIndirect<int>();
            e1.Return();

            var d1 = e1.CreateDelegate(out string instrs);

            Assert.Equal(2, d1(new[] { 1, 2, 3 }, 1));
            Assert.Contains("readonly.", instrs);
        }

        [Fact]
        public void NotReadonly()
        {
            var e1 = Emit<Action<int[], int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.LoadElementAddress<int>();
            e1.Duplicate();
            e1.LoadIndirect<int>();
            e1.LoadConstant(1);
            e1.Add();
            e1.StoreIndirect<int>();
            e1.Return();

            var d1 = e1.CreateDelegate(out string instrs);

            var x = new[] { 1, 2, 3 };
            d1(x, 1);

            Assert.Equal(1, x[0]);
            Assert.Equal(3, x[1]);
            Assert.Equal(3, x[2]);

            Assert.DoesNotContain("readonly.", instrs);
        }

        [Fact]
        public void ReadOnlyCall()
        {
            var toString = typeof(object).GetMethod("ToString");

            var e1 = Emit<Func<int, object[], string>>.NewDynamicMethod("E1");
            e1.LoadArgument(1);
            e1.LoadArgument(0);
            e1.LoadElementAddress<object>();
            e1.LoadIndirect<object>();
            e1.CallVirtual(toString);
            e1.Return();

            var d1 = e1.CreateDelegate(out string instrs);

            Assert.Equal("123", d1(0, new object[] { 123 }));
            Assert.Contains("readonly.", instrs);
        }

        [Fact]
        public void NotReadOnlyCall()
        {
            var toString = typeof(object).GetMethod("ToString");

            var e1 = Emit<Func<int, object[], string>>.NewDynamicMethod("E1");
            e1.LoadArgument(1);
            e1.LoadArgument(0);
            e1.LoadElementAddress<object>();
            e1.Duplicate();
            e1.Pop();
            e1.LoadIndirect<object>();
            e1.CallVirtual(toString);
            e1.Return();

            var d1 = e1.CreateDelegate(out string instrs);

            Assert.Equal("123", d1(0, new object[] { 123 }));
            Assert.DoesNotContain("readonly.", instrs);
        }
    }
}
