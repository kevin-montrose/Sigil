using System;
using Sigil;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SigilTests
{
    [TestClass, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class Labels
    {
        [TestMethod]
        public void Lookup()
        {
            var e1 = Emit<Func<bool, bool>>.NewDynamicMethod();
            var f = e1.DefineLabel("false");

            Assert.IsTrue(f == e1.Labels["false"]);

            e1.LoadArgument(0);
            e1.BranchIfFalse(f);
            e1.LoadConstant(true);
            e1.Return();

            e1.MarkLabel(e1.Labels["false"]);
            e1.LoadConstant(false);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.IsTrue(d1(true));
            Assert.IsFalse(d1(false));
        }
    }
}
