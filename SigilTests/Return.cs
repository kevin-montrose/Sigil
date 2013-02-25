using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestClass, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Return
    {
        [TestMethod]
        public void NonTerminal()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            var l1 = e1.DefineLabel("l1");
            var l2 = e1.DefineLabel("l2");

            e1.Branch(l1);

            e1.MarkLabel(l2);
            e1.Return();

            e1.MarkLabel(l1);
            e1.Branch(l2);

            var d1 = e1.CreateDelegate();
            d1();
        }

        [TestMethod]
        public void Empty()
        {
            var il = Emit<Action>.NewDynamicMethod("Empty");
            il.Return();

            var del = il.CreateDelegate();

            del();
        }

        [TestMethod]
        public void Constant()
        {
            var il = Emit<Func<int>>.NewDynamicMethod("Constant");
            il.LoadConstant(123);
            il.Return();

            var del = il.CreateDelegate();

            Assert.AreEqual(123, del());
        }
    }
}
