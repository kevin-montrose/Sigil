using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    public partial class StoreLocal
    {
        [TestMethod]
        public void IntNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes, "E1");

            var bar = e1.DeclareLocal<int>("bar");
            e1.LoadConstant(12);
            e1.LoadConstant(34);
            e1.Add();
            e1.StoreLocal(bar);
            e1.LoadLocal(bar);
            e1.Return();

            var del = e1.CreateDelegate<Func<int>>();

            Assert.AreEqual(46, del());
        }
    }
}
