using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    public partial class Throw
    {
        [TestMethod]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(void), Type.EmptyTypes);
            e1.LoadConstant("Hello!");
            e1.NewObject<Exception, string>();
            e1.Throw();
            e1.Return();

            var d1 = e1.CreateDelegate<Action>();

            try
            {
                d1();
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Hello!", e.Message);
            }
        }
    }
}
