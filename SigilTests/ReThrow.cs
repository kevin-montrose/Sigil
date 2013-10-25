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
    public partial class ReThrow
    {
        public static void AlwaysThrows()
        {
            throw new Exception("Hello World");
        }

        [TestMethod]
        public void Simple()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            var m = typeof(ReThrow).GetMethod("AlwaysThrows");

            var t = e1.BeginExceptionBlock();
            e1.Call(m);
            var c = e1.BeginCatchAllBlock(t);
            e1.ReThrow();
            e1.EndCatchBlock(c);
            e1.EndExceptionBlock(t);

            e1.Return();

            var d1 = e1.CreateDelegate();

            try
            {
                d1();
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Hello World", e.Message);
            }
        }
    }
}
