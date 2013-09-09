using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    public partial class Unbox
    {
        [TestMethod]
        public void JustUnboxNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(object) });
            e1.LoadArgument(0);
            e1.Unbox<int>();
            e1.LoadIndirect<int>();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<object, int>>();

            Assert.AreEqual(1234567, d1(1234567));
        }

        [TestMethod]
        public void UnboxAnyNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(object) });
            e1.LoadArgument(0);
            e1.UnboxAny<int>();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<object, int>>();

            Assert.AreEqual(1234567, d1(1234567));
        }
    }
}
