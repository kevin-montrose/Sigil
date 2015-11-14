#if !COREFXTODO
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestClass, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class TypedReferences
    {
        public static int _MakeRef(TypedReference a)
        {
            return __refvalue( a, int?) ?? 314159;
        }

        [TestMethod]
        public void MakeRef()
        {
            var e1 = Emit<Func<int?, int>>.NewDynamicMethod();

            e1.LoadArgumentAddress(0);
            e1.MakeReferenceAny<int?>();

            e1.Call(typeof(TypedReferences).GetMethod("_MakeRef", BindingFlags.Static | BindingFlags.Public));
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate(out instrs);

            var a = d1(123);
            var b = d1(null);

            Assert.AreEqual(123, a);
            Assert.AreEqual(314159, b);
        }

        [TestMethod]
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

            string instrs;
            var d1 = e1.CreateDelegate(out instrs);

            var x = d1();

            Assert.AreEqual(123, x);
        }

        [TestMethod]
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

            string instrs;
            var d1 = e1.CreateDelegate(out instrs);

            var x = d1();

            Assert.AreEqual(typeof(int?), x);
        }
    }
}
#endif