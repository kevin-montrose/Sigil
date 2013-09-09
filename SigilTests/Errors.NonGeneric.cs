using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    public partial class Errors
    {
        [TestMethod]
        public void NotDelegateNonGeneric()
        {
            try
            {
                var emit = Emit.NewDynamicMethod(typeof(void), Type.EmptyTypes);
                var del = emit.CreateDelegate(typeof(string));

                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("delegateType must be a delegate, found System.String", e.Message);
            }
        }

        [TestMethod]
        public void WrongReturnTypeNonGeneric()
        {
            try
            {
                var emit = Emit.NewDynamicMethod(typeof(string), Type.EmptyTypes);

                emit.LoadConstant("hello world");
                emit.Return();

                var del = emit.CreateDelegate(typeof(Func<int>));

                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("Expected delegateType to return System.String, found System.Int32", e.Message);
            }
        }

        [TestMethod]
        public void WrongParameterTypesNonGeneric()
        {
            {
                try
                {
                    var emit = Emit.NewDynamicMethod(typeof(void), new[] { typeof(int), typeof(string) });

                    emit.Return();

                    var del = emit.CreateDelegate(typeof(Action<int, int>));

                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("Expected delegateType's parameter at index 1 to be a System.String, found System.Int32", e.Message);
                }
            }

            {
                try
                {
                    var emit = Emit.NewDynamicMethod(typeof(void), new[] { typeof(int) });

                    emit.Return();

                    var del = emit.CreateDelegate(typeof(Action<int, int>));

                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("Expected delegateType to take 1 parameters, found 2", e.Message);
                }
            }
        }
    }
}
