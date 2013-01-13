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

        [TestMethod]
        public void EmitSingleReturn()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");
            e1.Return();

            var del = e1.CreateDelegate();

            try
            {
                e1.LoadConstant(100);
                Assert.Fail("Shouldn't be able to modify emit after a delegate has been created");
            }
            catch (SigilException e)
            {
                Assert.AreEqual(e.Message, "Cannot modify Emit after a delegate has been generated from it");
            }

            del();
        }
    }
}
