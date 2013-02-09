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
    public class NewObject
    {
        class Foo
        {
            int _i;

            private Foo(int i)
            {
                _i = i;
            }

            public override string ToString()
            {
                return _i.ToString();
            }
        }

        [TestMethod]
        public void PrivateConstructor()
        {
            var e1 = Emit<Func<int, string>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.NewObject<Foo, int>();
            e1.CallVirtual(typeof(object).GetMethod("ToString"));
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual("314159", d1(314159));
        }
    }
}
