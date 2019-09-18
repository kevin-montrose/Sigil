using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using Xunit;

namespace SigilTests
{
    public partial class CastClass
    {
        [Fact]
        public void VeryLongMethodNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(List<String>), new [] { typeof(List<string>) });

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

            var d1 = e1.CreateDelegate<Func<List<string>, List<string>>>();

            var x = new List<string> { "hello", "world" };

            Assert.Equal(x, d1(x));
        }

        [Fact]
        public void DisableElidingNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(string), new [] { typeof(string) });
            e1.LoadArgument(0);
            e1.CastClass<object>();
            e1.CallVirtual(typeof(object).GetMethod("ToString"));
            e1.Return();

            var d1 = e1.CreateDelegate<Func<string, string>>(out string instrs, Sigil.OptimizationOptions.All & ~Sigil.OptimizationOptions.EnableTrivialCastEliding);

            Assert.Equal("foo", d1("foo"));
            Assert.Contains("castclass", instrs);
        }

        [Fact]
        public void ElideNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(string), new [] { typeof(string) });
            e1.LoadArgument(0);
            e1.CastClass<object>();
            e1.CallVirtual(typeof(object).GetMethod("ToString"));
            e1.Return();

            var d1 = e1.CreateDelegate<Func<string, string>>(out string instrs);

            Assert.Equal("foo", d1("foo"));
            Assert.DoesNotContain("castclass", instrs);
        }

        [Fact]
        public void ElideBranchedNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(string), new [] { typeof(string) });
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

            var d1 = e1.CreateDelegate<Func<string, string>>(out string instrs);

            Assert.Equal("foo", d1("foo"));
            Assert.DoesNotContain("castclass", instrs);
        }

        [Fact]
        public void ElideManyBranchedNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(string), new [] { typeof(string) });

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

            var d1 = e1.CreateDelegate<Func<string, string>>(out string instrs);

            Assert.Equal("foo", d1("foo"));
            Assert.DoesNotContain("castclass", instrs);
        }

        [Fact]
        public void ManyBranchedNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(string), new [] { typeof(object) });

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

            var d1 = e1.CreateDelegate<Func<object, string>>(out string instrs);

            Assert.Equal("foo", d1("foo"));
            Assert.Contains("castclass", instrs);
        }

        [Fact]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(string), new [] { typeof(object) });
            e1.LoadArgument(0);
            e1.CastClass<string>();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<object, string>>(out string instrs);

            Assert.Null(d1(null));
            Assert.Equal("hello", d1("hello"));
            Assert.Contains("castclass", instrs);
        }
    }
}
