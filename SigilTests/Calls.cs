using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestClass]
    public class Calls
    {
        [TestMethod]
        public void PartialTypeMapping2()
        {
            {
                var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                var mod = asm.DefineDynamicModule("Bar");
                var t = mod.DefineType("T");
                var dictOfT = typeof(IDictionary<,>).MakeGenericType(t, t);

                var e1 = Emit<Func<object, object, bool>>.BuildMethod(t, "E1", MethodAttributes.Public, CallingConventions.Standard | CallingConventions.HasThis);
                e1.LoadArgument(1);
                e1.CastClass(dictOfT);
                e1.LoadArgument(2);
                e1.CallVirtual(typeof(object).GetMethod("Equals", new Type[] { typeof(object) }));
                e1.Return();

                e1.CreateMethod();

                var type = t.CreateType();

                var inst = type.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
                var e1Mtd = type.GetMethod("E1");

                Func<object, object, bool> d1 = (a, b) => (bool)e1Mtd.Invoke(inst, new object[] { a, b });

                dictOfT = typeof(Dictionary<,>).MakeGenericType(type, type);
                var cons = dictOfT.GetConstructor(Type.EmptyTypes);

                var dictInst = cons.Invoke(new object[0]);
                var listAdd = dictOfT.GetMethod("Add", new[] { type, type });
                listAdd.Invoke(dictInst, new object[] { inst, inst });

                var x = dictInst;
                var y = "hello world";

                Assert.IsTrue(d1(x, x));
                Assert.IsFalse(d1(x, new List<int> { 1, 2, 3 }));
            }
        }

        [TestMethod]
        public void PartialTypeMapping1()
        {
            {
                var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                var mod = asm.DefineDynamicModule("Bar");
                var t = mod.DefineType("T");
                var listOfT = typeof(List<>).MakeGenericType(t);

                var e1 = Emit<Func<object, object, bool>>.BuildMethod(t, "E1", MethodAttributes.Public, CallingConventions.Standard | CallingConventions.HasThis);
                e1.LoadArgument(1);
                e1.CastClass(listOfT);
                e1.LoadArgument(2);
                e1.CallVirtual(typeof(object).GetMethod("Equals", new Type[] { typeof(object) }));
                e1.Return();

                e1.CreateMethod();

                var type = t.CreateType();

                var inst = type.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
                var e1Mtd = type.GetMethod("E1");

                Func<object, object, bool> d1 = (a, b) => (bool)e1Mtd.Invoke(inst, new object[] { a, b });

                listOfT = typeof(List<>).MakeGenericType(type);
                var cons = listOfT.GetConstructor(Type.EmptyTypes);

                var listInst = cons.Invoke(new object[0]);
                var listAdd = listOfT.GetMethod("Add", new[] { type });
                listAdd.Invoke(listInst, new object[] { inst });

                var x = listInst;
                var y = "hello world";

                Assert.IsTrue(d1(x, x));
                Assert.IsFalse(d1(x, new List<int> { 1, 2, 3 }));
            }
        }

        public enum EnumParamsEnum
        {
            A,
            B
        }

        private static EnumParamsEnum? _EnumParamsMethod;
        public static void EnumParamsMethod(EnumParamsEnum foo)
        {
            _EnumParamsMethod = foo;
        }

        [TestMethod]
        public void EnumParams()
        {
            var e1 = Emit<Action<int>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.Call(typeof(Calls).GetMethod("EnumParamsMethod"));
            e1.Return();

            var d1 = e1.CreateDelegate();

            _EnumParamsMethod = null;
            d1((int)EnumParamsEnum.A);
            Assert.AreEqual(EnumParamsEnum.A, _EnumParamsMethod.Value);

            _EnumParamsMethod = null;
            d1((int)EnumParamsEnum.B);
            Assert.AreEqual(EnumParamsEnum.B, _EnumParamsMethod.Value);
        }

        private static bool DoesNothingWasCalled;
        public static void DoesNothing()
        {
            DoesNothingWasCalled = true;
        }

        [TestMethod]
        public void VoidStatic()
        {
            var mi = typeof(Calls).GetMethod("DoesNothing");

            var e1 = Emit<Action>.NewDynamicMethod("E1");
            e1.Call(mi);
            e1.Return();

            var del = e1.CreateDelegate();

            Assert.IsFalse(DoesNothingWasCalled);
            del();
            Assert.IsTrue(DoesNothingWasCalled);
        }

        class VoidInstanceClass
        {
            public int Go() { return 314159; }
        }

        [TestMethod]
        public void VoidInstance()
        {
            var e1 = Emit<Func<int>>.NewDynamicMethod("E1");
            e1.NewObject<VoidInstanceClass>();
            e1.Call(typeof(VoidInstanceClass).GetMethod("Go"));
            e1.Return();

            var del = e1.CreateDelegate();

            Assert.AreEqual(314159, del());
        }

        class StringInstanceClass
        {
            public string Go(int hello) { return hello.ToString(); }
        }

        [TestMethod]
        public void StringInstance()
        {
            var e1 = Emit<Func<string>>.NewDynamicMethod("E1");
            e1.NewObject<StringInstanceClass>();
            e1.LoadConstant(8675309);
            e1.Call(typeof(StringInstanceClass).GetMethod("Go"));
            e1.Return();

            var del = e1.CreateDelegate();

            Assert.AreEqual("8675309", del());
        }
    }
}
