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
    public class Leave
    {
        [TestMethod]
        public void Simple()
        {
            var e1 = Emit<Func<int, int>>.NewDynamicMethod();
            var t = e1.BeginExceptionBlock();
            e1.Leave(t.Label);
            e1.LoadArgument(0);
            e1.LoadConstant(1);
            e1.Add();
            e1.StoreArgument(0);
            var c = e1.BeginCatchAllBlock(t);
            e1.Pop();
            e1.EndCatchBlock(c);
            e1.EndExceptionBlock(t);
            e1.LoadArgument(0);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(1, d1(1));
        }
    }
}
