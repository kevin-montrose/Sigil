using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    public partial class NewArray
    {
        [TestMethod]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int[]), Type.EmptyTypes);
            e1.LoadConstant(128);
            e1.NewArray<int>();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int[]>>();

            var x = d1();

            Assert.AreEqual(128, x.Length);
        }
    }
}
