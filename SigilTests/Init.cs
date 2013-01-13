using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;

namespace SigilTests
{
    [TestClass]
    public class Init
    {
        delegate string FooDelegate(object p);

        [TestMethod]
        public void EmitDelegateParsing()
        {
            var e1 = Emit<Action<int, string>>.NewDynamicMethod("E1");
            var e2 = Emit<Func<int>>.NewDynamicMethod("E2");
            var e3 = Emit<FooDelegate>.NewDynamicMethod("E3");

            Assert.IsNotNull(e1);
            Assert.IsNotNull(e2);
            Assert.IsNotNull(e3);

            try
            {
                var e4 = Emit<string>.NewDynamicMethod("E4");
                Assert.Fail("Shouldn't be able to emit non-delegate");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("DelegateType must be a delegate, found System.String", e.Message);
            }
        }
    }
}
