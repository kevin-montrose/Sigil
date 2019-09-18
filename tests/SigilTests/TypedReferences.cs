using Sigil;
using System;
using System.Reflection;
using Xunit;

namespace SigilTests
{
    public partial class TypedReferences
    {
        public static int _MakeRef(TypedReference a)
        {
            return __refvalue( a, int?) ?? 314159;
        }

        [Fact]
        public void MakeRef()
        {
            var e1 = Emit<Func<int?, int>>.NewDynamicMethod();

            e1.LoadArgumentAddress(0);
            e1.MakeReferenceAny<int?>();

            e1.Call(typeof(TypedReferences).GetMethod("_MakeRef", BindingFlags.Static | BindingFlags.Public));
            e1.Return();

            var d1 = e1.CreateDelegate(out string instrs);

            var a = d1(123);
            var b = d1(null);

            Assert.Equal(123, a);
            Assert.Equal(314159, b);
        }

        [Fact]
        public void RefValue()
        {
            var e1 = Emit<Func<int>>.NewDynamicMethod();
            var a = e1.DeclareLocal<int?>("a");
            e1.LoadConstant(123);
            e1.NewObject<int?, int>();
            e1.StoreLocal(a);
            e1.LoadLocalAddress(a);
            e1.MakeReferenceAny<int?>();
            e1.ReferenceAnyValue<int?>();
            e1.Call(typeof(int?).GetProperty("Value").GetGetMethod());
            e1.Return();

            var d1 = e1.CreateDelegate(out string instrs);

            var x = d1();

            Assert.Equal(123, x);
        }

        [Fact]
        public void RefType()
        {
            var e1 = Emit<Func<Type>>.NewDynamicMethod();
            var a = e1.DeclareLocal<int?>("a");
            e1.LoadConstant(123);
            e1.NewObject<int?, int>();
            e1.StoreLocal(a);
            e1.LoadLocalAddress(a);
            e1.MakeReferenceAny<int?>();
            e1.ReferenceAnyType();
            e1.Call(typeof(Type).GetMethod("GetTypeFromHandle"));
            e1.Return();

            var d1 = e1.CreateDelegate(out string instrs);

            var x = d1();

            Assert.Equal(typeof(int?), x);
        }
    }
}