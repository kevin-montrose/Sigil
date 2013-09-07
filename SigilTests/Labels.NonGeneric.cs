using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    public partial class Labels
    {
        [TestMethod]
        public void LookupNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(bool), new [] { typeof(bool) });
            var f = e1.DefineLabel("false");

            Assert.IsTrue(f == e1.Labels["false"]);

            e1.LoadArgument(0);
            e1.BranchIfFalse(f);
            e1.LoadConstant(true);
            e1.Return();

            e1.MarkLabel(e1.Labels["false"]);
            e1.LoadConstant(false);
            e1.Return();

            var d1 = e1.CreateDelegate<Func<bool, bool>>();

            Assert.IsTrue(d1(true));
            Assert.IsFalse(d1(false));
        }
    }
}
