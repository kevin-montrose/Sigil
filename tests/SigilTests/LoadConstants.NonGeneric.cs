using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class LoadConstants
    {
        [Fact]
        public void NullNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(string), new [] { typeof(string) }, "E1");
            e1.LoadNull();
            e1.StoreArgument(0);
            e1.LoadArgument(0);
            e1.Return();

            var d1 = e1.CreateDelegate<Func<string, string>>();

            Assert.Null(d1("Foo"));
        }

        [Fact]
        public void AllBoolsNonGeneric()
        {
            {
                var e1 = Emit.NewDynamicMethod(typeof(bool), System.Type.EmptyTypes);
                e1.LoadConstant(true);
                e1.Return();

                var d1 = e1.CreateDelegate<Func<bool>>();
                Assert.True(d1());
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(bool), System.Type.EmptyTypes);
                e1.LoadConstant(false);
                e1.Return();

                var d1 = e1.CreateDelegate<Func<bool>>();
                Assert.False(d1());
            }
        }

        [Fact]
        public void AllIntsNonGeneric()
        {
            for (var i = -1; i <= 256; i++)
            {
                var e1 = Emit.NewDynamicMethod(typeof(int), System.Type.EmptyTypes);
                e1.LoadConstant(i);
                e1.Return();

                var d1 = e1.CreateDelegate<Func<int>>();

                Assert.Equal(i, d1());
            }
        }

        [Fact]
        public void AllUIntsNonGeneric()
        {
            for (uint i = 0; i <= 256; i++)
            {
                var e1 = Emit.NewDynamicMethod(typeof(uint), System.Type.EmptyTypes);
                e1.LoadConstant(i);
                e1.Return();

                var d1 = e1.CreateDelegate<Func<uint>>();

                Assert.Equal(i, d1());
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(uint), System.Type.EmptyTypes);
                e1.LoadConstant(uint.MaxValue);
                e1.Return();

                var d1 = e1.CreateDelegate<Func<uint>>();

                Assert.Equal(uint.MaxValue, d1());
            }
        }

        [Fact]
        public void LongNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(long), System.Type.EmptyTypes);
            e1.LoadConstant(long.MaxValue);
            e1.Return();

            var d1 = e1.CreateDelegate<Func<long>>();

            Assert.Equal(long.MaxValue, d1());
        }

        [Fact]
        public void ULongNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(ulong), System.Type.EmptyTypes);
            e1.LoadConstant(ulong.MaxValue);
            e1.Return();

            var d1 = e1.CreateDelegate<Func<ulong>>();

            Assert.Equal(ulong.MaxValue, d1());
        }

        [Fact]
        public void FloatNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(float), System.Type.EmptyTypes);
            e1.LoadConstant(12.34f);
            e1.Return();

            var d1 = e1.CreateDelegate<Func<float>>();

            Assert.Equal(12.34f, d1());
        }

        [Fact]
        public void DoubleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(double), System.Type.EmptyTypes);
            e1.LoadConstant(12.34);
            e1.Return();

            var d1 = e1.CreateDelegate<Func<double>>();

            Assert.Equal(12.34, d1());
        }

        [Fact]
        public void StringNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(string), System.Type.EmptyTypes);
            e1.LoadConstant("hello world");
            e1.Return();

            var d1 = e1.CreateDelegate<Func<string>>();

            Assert.Equal("hello world", d1());
        }

        [Fact]
        public void TypeNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(Type), System.Type.EmptyTypes);
            e1.LoadConstant<string>();
            e1.Call(typeof(Type).GetMethod("GetTypeFromHandle"));
            e1.Return();

            var d1 = e1.CreateDelegate<Func<Type>>();

            Assert.Equal(typeof(string), d1());
        }

        [Fact]
        public void MethodNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(RuntimeMethodHandle), System.Type.EmptyTypes);
            e1.LoadConstant(typeof(RuntimeMethodHandle).GetMethod("GetFunctionPointer"));
            e1.Return();

            var d1 = e1.CreateDelegate<Func<RuntimeMethodHandle>>();

            Assert.NotEqual(default, d1());
        }

        [Fact]
        public void FieldNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(RuntimeFieldHandle), System.Type.EmptyTypes);
            e1.LoadConstant(typeof(FieldClass).GetField("Foo"));
            e1.Return();

            var d1 = e1.CreateDelegate<Func<RuntimeFieldHandle>>();

            Assert.NotEqual(default, d1());
        }
    }
}
