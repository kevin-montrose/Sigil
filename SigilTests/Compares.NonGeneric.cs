using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    public partial class Compares
    {
        [TestMethod]
        public void EqualsNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(bool), new [] { typeof(int), typeof(int) });
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.CompareEqual();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int, int, bool>>();

            Assert.IsTrue(d1(1, 1));
            Assert.IsFalse(d1(1, 2));
        }

        [TestMethod]
        public void GreaterThanNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(bool), new [] { typeof(int), typeof(int) });
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.CompareGreaterThan();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int, int, bool>>();

            Assert.IsTrue(d1(5, 1));
            Assert.IsFalse(d1(1, 1));
        }

        [TestMethod]
        public void LessThanNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(bool), new [] { typeof(int), typeof(int) });
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.CompareLessThan();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int, int, bool>>();

            Assert.IsTrue(d1(6, 10));
            Assert.IsFalse(d1(1, 1));
        }

        [TestMethod]
        public void UnsignedCompareGreaterThanNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(bool), new [] { typeof(uint), typeof(uint) });
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.UnsignedCompareGreaterThan();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<uint, uint, bool>>();

            Assert.IsTrue(d1(uint.MaxValue, (uint)int.MaxValue));
            Assert.IsFalse(d1(1, 1));
        }

        [TestMethod]
        public void UnsignedCompareLessThanNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(bool), new [] { typeof(uint), typeof(uint) });
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.UnsignedCompareLessThan();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<uint, uint, bool>>();

            Assert.IsTrue(d1((uint)int.MaxValue, uint.MaxValue));
            Assert.IsFalse(d1(1, 1));
        }
    }
}
