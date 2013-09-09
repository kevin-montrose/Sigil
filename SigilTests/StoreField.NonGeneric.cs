using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    public partial class StoreField
    {
        [TestMethod]
        public void SimpleNonGeneric()
        {
            {
                var e1 = Emit.NewDynamicMethod(typeof(void), new [] { typeof(StoreFieldClass) });
                var f = typeof(StoreFieldClass).GetField("Static");
                e1.LoadConstant(12);
                e1.StoreField(f);
                e1.Return();

                var d1 = e1.CreateDelegate<Action<StoreFieldClass>>();

                StoreFieldClass.Static = 2;
                d1(null);

                Assert.AreEqual(12, StoreFieldClass.Static);
            }

            {
                var e1 = Emit.NewDynamicMethod(typeof(void), new [] { typeof(StoreFieldClass) });
                var f = typeof(StoreFieldClass).GetField("Instance");
                e1.LoadArgument(0);
                e1.LoadConstant(12);
                e1.StoreField(f);
                e1.Return();

                var d1 = e1.CreateDelegate<Action<StoreFieldClass>>();

                var x = new StoreFieldClass();
                d1(x);

                Assert.AreEqual(12, x.Instance);
            }
        }
    }
}
