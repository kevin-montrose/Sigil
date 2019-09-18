using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class LoadConstants
    {
        [Fact]
        public void Null()
        {
            var e1 = Emit<Func<string, string>>.NewDynamicMethod("E1");
            e1.LoadNull();
            e1.StoreArgument(0);
            e1.LoadArgument(0);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Null(d1("Foo"));
        }

        [Fact]
        public void AllBools()
        {
            {
                var e1 = Emit<Func<bool>>.NewDynamicMethod();
                e1.LoadConstant(true);
                e1.Return();

                var d1 = e1.CreateDelegate();
                Assert.True(d1());
            }

            {
                var e1 = Emit<Func<bool>>.NewDynamicMethod();
                e1.LoadConstant(false);
                e1.Return();

                var d1 = e1.CreateDelegate();
                Assert.False(d1());
            }
        }

        [Fact]
        public void AllInts()
        {
            for (var i = -1; i <= 256; i++)
            {
                var e1 = Emit<Func<int>>.NewDynamicMethod();
                e1.LoadConstant(i);
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.Equal(i, d1());
            }
        }

        [Fact]
        public void AllUInts()
        {
            for (uint i = 0; i <= 256; i++)
            {
                var e1 = Emit<Func<uint>>.NewDynamicMethod();
                e1.LoadConstant(i);
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.Equal(i, d1());
            }

            {
                var e1 = Emit<Func<uint>>.NewDynamicMethod();
                e1.LoadConstant(uint.MaxValue);
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.Equal(uint.MaxValue, d1());
            }
        }

        [Fact]
        public void Long()
        {
            var e1 = Emit<Func<long>>.NewDynamicMethod();
            e1.LoadConstant(long.MaxValue);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(long.MaxValue, d1());
        }

        [Fact]
        public void ULong()
        {
            var e1 = Emit<Func<ulong>>.NewDynamicMethod();
            e1.LoadConstant(ulong.MaxValue);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(ulong.MaxValue, d1());
        }

        [Fact]
        public void Float()
        {
            var e1 = Emit<Func<float>>.NewDynamicMethod();
            e1.LoadConstant(12.34f);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(12.34f, d1());
        }

        [Fact]
        public void Double()
        {
            var e1 = Emit<Func<double>>.NewDynamicMethod();
            e1.LoadConstant(12.34);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(12.34, d1());
        }

        [Fact]
        public void String()
        {
            var e1 = Emit<Func<string>>.NewDynamicMethod();
            e1.LoadConstant("hello world");
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal("hello world", d1());
        }

        [Fact]
        public void Type()
        {
            var e1 = Emit<Func<Type>>.NewDynamicMethod();
            e1.LoadConstant<string>();
            e1.Call(typeof(Type).GetMethod("GetTypeFromHandle"));
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(typeof(string), d1());
        }

        [Fact]
        public void Method()
        {
            var e1 = Emit<Func<RuntimeMethodHandle>>.NewDynamicMethod();
            e1.LoadConstant(typeof(RuntimeMethodHandle).GetMethod("GetFunctionPointer"));
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.NotEqual(default, d1());
        }

        private class FieldClass
        {
            public int Foo;

            public FieldClass() { Foo = 123; }
        }

        [Fact]
        public void Field()
        {
            var e1 = Emit<Func<RuntimeFieldHandle>>.NewDynamicMethod();
            e1.LoadConstant(typeof(FieldClass).GetField("Foo"));
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.NotEqual(default, d1());
        }
    }
}
