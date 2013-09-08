using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    public partial class LoadFieldAddress
    {
        [TestMethod]
        public void InstanceNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(TestObj) });
            e1.LoadArgument(0);
            e1.LoadFieldAddress(typeof(TestObj).GetField("Instance"));
            e1.LoadIndirect<int>();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<TestObj, int>>();

            Assert.AreEqual(10, d1(new TestObj { Instance = 10 }));
        }

        [TestMethod]
        public void StaticNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes);
            e1.LoadFieldAddress(typeof(TestObj).GetField("Static"));
            e1.LoadIndirect<int>();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int>>();

            TestObj.Static = 20;
            Assert.AreEqual(20, d1());
        }
    }
}
