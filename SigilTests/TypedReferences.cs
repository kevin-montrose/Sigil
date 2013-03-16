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
    [TestClass]
    public class TypedReferences
    {
        public static int _Simple(TypedReference a)
        {
            return __refvalue( a, int?) ?? 314159;
        }

        [TestMethod]
        public void Simple()
        {
            var e1 = Emit<Func<int?, int>>.NewDynamicMethod();

            e1.LoadArgumentAddress(0);
            e1.MakeReferenceAny<int?>();

            e1.Call(typeof(TypedReferences).GetMethod("_Simple", BindingFlags.Static | BindingFlags.Public));
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate(out instrs);

            var a = d1(123);
            var b = d1(null);

            Assert.AreEqual(123, a);
            Assert.AreEqual(314159, b);
        }
    }
}
