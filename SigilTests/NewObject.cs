using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestClass, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class NewObject
    {
        class ThreeClass
        {
            public string Value;

            public ThreeClass(string a, int b, List<double> c)
            {
                Value = a + " @" + b + " ==> " + string.Join(", ", c.Select(d => d.ToString(CultureInfo.InvariantCulture)));
            }
        }

        [TestMethod]
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

            Assert.AreEqual("hello @10 ==> 1, 2.5, 5.1", d1("hello", 10, new List<double> { 1.0, 2.5, 5.1 }));
        }

        class Foo
        {
            int _i;

            private Foo(int i)
            {
                _i = i;
            }

            public override string ToString()
            {
                return _i.ToString();
            }
        }

        [TestMethod]
        public void PrivateConstructor()
        {
            var e1 = Emit<Func<int, string>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.NewObject<Foo, int>();
            e1.CallVirtual(typeof(object).GetMethod("ToString"));
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual("314159", d1(314159));
        }

        class RT
        {
            public int A;

            public RT(int a)
            {
                A = a;
            }
        }

        [TestMethod]
        public void ReferenceType()
        {
            var e1 = Emit<Func<RT>>.NewDynamicMethod("E1");
            e1.LoadConstant(314159);
            e1.NewObject<RT, int>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(314159, d1().A);
        }

        struct VT
        {
            public double B;

            public VT(double b)
            {
                B = b;
            }
        }


        [TestMethod]
        public void ValueType()
        {
            var e1 = Emit<Func<VT>>.NewDynamicMethod("E1");
            e1.LoadConstant(3.1415926);
            e1.NewObject<VT, double>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(3.1415926, d1().B);
        }
    }
}
