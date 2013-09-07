using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    public partial class Boxing
    {
        [TestMethod]
        public void NullableIntNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(object), new [] { typeof(int?) }, "E1");
            e1.LoadArgument(0);
            e1.Box(typeof(int?));
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int?, object>>();

            Assert.AreEqual((object)((int?)123), d1(123));
            Assert.AreEqual((object)((int?)null), d1(null));
        }

        [TestMethod]
        public void BooleanNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(object), new [] { typeof(bool) }, "E1");
            e1.LoadArgument(0);
            e1.Box<bool>();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<bool, object>>();

            Assert.AreEqual((object)true, d1(true));
            Assert.AreEqual((object)false, d1(false));
        }

        [TestMethod]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(object), Type.EmptyTypes, "E1");
            e1.LoadConstant(123);
            e1.Box<byte>();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<object>>();

            Assert.AreEqual("123", d1().ToString());

            var e2 = Emit.NewDynamicMethod(typeof(object), Type.EmptyTypes, "E2");
            e2.LoadConstant(566);
            e2.Box<byte>();
            e2.Return();

            var d2 = e2.CreateDelegate<Func<object>>();

            Assert.AreEqual("54", d2().ToString());
        }
    }
}
