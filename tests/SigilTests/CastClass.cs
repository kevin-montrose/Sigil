using Sigil;
using System;
using System.Collections.Generic;
using Xunit;

namespace SigilTests
{
    public partial class CastClass
    {
        [Fact]
        public void VeryLongMethod()
        {
            var e1 = Emit<Func<List<string>, List<string>>>.NewDynamicMethod();

            e1.LoadArgument(0);

            for (var i = 0; i < 10000; i++)
            {
                e1.CastClass<IEnumerable<string>>();
                e1.CastClass<System.Collections.IEnumerable>();
                e1.CastClass<object>();
                e1.CastClass<System.Collections.IEnumerable>();
                e1.CastClass<IEnumerable<string>>();
                e1.CastClass<List<string>>();
            }

            e1.Return();

            var d1 = e1.CreateDelegate();

            var x = new List<string> { "hello", "world" };

            Assert.Equal(x, d1(x));
        }

        [Fact]
        public void DisableEliding()
        {
            var e1 = Emit<Func<string, string>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.CastClass<object>();
            e1.CallVirtual(typeof(object).GetMethod("ToString"));
            e1.Return();

            var d1 = e1.CreateDelegate(out string instrs, OptimizationOptions.All & ~OptimizationOptions.EnableTrivialCastEliding);

            Assert.Equal("foo", d1("foo"));
            Assert.Contains("castclass", instrs);
        }

        [Fact]
        public void Elide()
        {
            var e1 = Emit<Func<string, string>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.CastClass<object>();
            e1.CallVirtual(typeof(object).GetMethod("ToString"));
            e1.Return();

            var d1 = e1.CreateDelegate(out string instrs);

            Assert.Equal("foo", d1("foo"));
            Assert.DoesNotContain("castclass", instrs);
        }

        [Fact]
        public void ElideBranched()
        {
            var e1 = Emit<Func<string, string>>.NewDynamicMethod();
            var l1 = e1.DefineLabel();
            var l2 = e1.DefineLabel();
            var l3 = e1.DefineLabel();

            e1.LoadArgument(0);
            e1.Branch(l1);

            e1.MarkLabel(l2);
            e1.CastClass<object>();
            e1.CallVirtual(typeof(object).GetMethod("ToString"));
            e1.Branch(l3);

            e1.MarkLabel(l1);
            e1.Branch(l2);

            e1.MarkLabel(l3);
            e1.Return();

            var d1 = e1.CreateDelegate(out string instrs);

            Assert.Equal("foo", d1("foo"));
            Assert.DoesNotContain("castclass", instrs);
        }

        [Fact]
        public void ElideManyBranched()
        {
            var e1 = Emit<Func<string, string>>.NewDynamicMethod();

            e1.LoadArgument(0);

            for (var i = 0; i < 100; i++)
            {
                var l1 = e1.DefineLabel();
                var l2 = e1.DefineLabel();
                var l3 = e1.DefineLabel();
                e1.Branch(l1);

                e1.MarkLabel(l2);
                e1.Duplicate();
                e1.Pop();
                e1.Branch(l3);

                e1.MarkLabel(l1);
                e1.Branch(l2);

                e1.MarkLabel(l3);
            }

            e1.CastClass<string>();
            e1.Return();

            var d1 = e1.CreateDelegate(out string instrs);

            Assert.Equal("foo", d1("foo"));
            Assert.DoesNotContain("castclass", instrs);
        }

        [Fact]
        public void ManyBranched()
        {
            var e1 = Emit<Func<object, string>>.NewDynamicMethod();

            e1.LoadArgument(0);

            for (var i = 0; i < 200; i++)
            {
                var l1 = e1.DefineLabel();
                var l2 = e1.DefineLabel();
                var l3 = e1.DefineLabel();
                e1.Branch(l1);

                e1.MarkLabel(l2);
                e1.Duplicate();
                e1.Pop();
                e1.Branch(l3);

                e1.MarkLabel(l1);
                e1.Branch(l2);

                e1.MarkLabel(l3);
            }

            e1.CastClass<string>();
            e1.Return();

            var d1 = e1.CreateDelegate(out string instrs);

            Assert.Equal("foo", d1("foo"));
            Assert.Contains("castclass", instrs);
        }

        [Fact]
        public void Simple()
        {
            var e1 = Emit<Func<object, string>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.CastClass<string>();
            e1.Return();

            var d1 = e1.CreateDelegate(out string instrs);

            Assert.Null(d1(null));
            Assert.Equal("hello", d1("hello"));
            Assert.Contains("castclass", instrs);
        }
    }
}
