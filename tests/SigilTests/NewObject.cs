using System.Globalization;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using Xunit;

namespace SigilTests
{
    public partial class NewObject
    {
        private class ThreeClass
        {
            public string Value;

            public ThreeClass(string a, int b, List<double> c)
            {
                Value = a + " @" + b + " ==> " + string.Join(", ", c.Select(d => d.ToString(CultureInfo.InvariantCulture)));
            }
        }

        [Fact]
        public void MultiParam()
        {
            var e1 = Emit<Func<string, int, List<double>, string>>.NewDynamicMethod();
            var val = typeof(ThreeClass).GetField("Value");

            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.LoadArgument(2);
            e1.NewObject<ThreeClass, string, int, List<double>>();
            e1.LoadField(val);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal("hello @10 ==> 1, 2.5, 5.1", d1("hello", 10, new List<double> { 1.0, 2.5, 5.1 }));
        }

        private class Foo
        {
            private int _i;

            private Foo(int i)
            {
                _i = i;
            }

            public override string ToString()
            {
                return _i.ToString();
            }
        }

        [Fact]
        public void PrivateConstructor()
        {
            var e1 = Emit<Func<int, string>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.NewObject<Foo, int>();
            e1.CallVirtual(typeof(object).GetMethod("ToString"));
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal("314159", d1(314159));
        }

        private class RT
        {
            public int A;

            public RT(int a)
            {
                A = a;
            }
        }

        [Fact]
        public void ReferenceType()
        {
            var e1 = Emit<Func<RT>>.NewDynamicMethod("E1");
            e1.LoadConstant(314159);
            e1.NewObject<RT, int>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(314159, d1().A);
        }

        private struct VT
        {
            public double B;

            public VT(double b)
            {
                B = b;
            }
        }

        [Fact]
        public void ValueType()
        {
            var e1 = Emit<Func<VT>>.NewDynamicMethod("E1");
            e1.LoadConstant(3.1415926);
            e1.NewObject<VT, double>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(3.1415926, d1().B);
        }

        [Fact]
        public void ConstructorBuilders()
        {
            var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("ConstructorBuilders"), AssemblyBuilderAccess.Run);
            var mod = asm.DefineDynamicModule("Mod");
            var tb = mod.DefineType("Type");

            var cb = Emit<Action>.BuildConstructor(tb, MethodAttributes.Public);
            cb.Return();
            var cons = cb.CreateConstructor();

            var createProxy = Emit<Func<object>>.BuildStaticMethod(tb, "Create", MethodAttributes.Public | MethodAttributes.Static, true, false);
            createProxy.NewObject(cons, new Type[0]);
            createProxy.Return();

            createProxy.CreateMethod();

            var t = tb.CreateType();

            var mtd = t.GetMethod("Create");
            var o = mtd.Invoke(null, new object[0]);

            Assert.NotNull(o);
            Assert.Equal("Type", o.GetType().Name);
        }
    }
}
