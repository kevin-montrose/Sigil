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
    public class StoreField
    {
        class StoreFieldClass
        {
            public static int Static;
            public int Instance;

            public StoreFieldClass()
            {
                Static = Instance = 314159;
            }
        }

        [TestMethod]
        public void Simple()
        {
            {
                var e1 = Emit<Action<StoreFieldClass>>.NewDynamicMethod();
                var f = typeof(StoreFieldClass).GetField("Static");
                e1.LoadConstant(12);
                e1.StoreField(f);
                e1.Return();

                var d1 = e1.CreateDelegate();

                StoreFieldClass.Static = 2;
                d1(null);

                Assert.AreEqual(12, StoreFieldClass.Static);
            }

            {
                var e1 = Emit<Action<StoreFieldClass>>.NewDynamicMethod();
                var f = typeof(StoreFieldClass).GetField("Instance");
                e1.LoadArgument(0);
                e1.LoadConstant(12);
                e1.StoreField(f);
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = new StoreFieldClass();
                d1(x);

                Assert.AreEqual(12, x.Instance);
            }
        }
    }
}
