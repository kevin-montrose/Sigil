using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestClass]
    public class Calls
    {
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
