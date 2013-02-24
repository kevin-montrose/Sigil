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
    public class CastClass
    {
        [TestMethod]
        public void DisableEliding()
        {
            var e1 = Emit<Func<string, string>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.CastClass<object>();
            e1.CallVirtual(typeof(object).GetMethod("ToString"));
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate(out instrs, OptimizationOptions.All & ~OptimizationOptions.EnableTrivialCastEliding);

            Assert.AreEqual("foo", d1("foo"));
            Assert.IsTrue(instrs.Contains("castclass"));
        }

        [TestMethod]
        public void Elide()
        {
            var e1 = Emit<Func<string, string>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.CastClass<object>();
            e1.CallVirtual(typeof(object).GetMethod("ToString"));
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate(out instrs);

            Assert.AreEqual("foo", d1("foo"));
            Assert.IsFalse(instrs.Contains("castclass"));
        }

        [TestMethod]
        public void ElideBranched()
        {
            var e1 = Emit<Func<string, string>>.NewDynamicMethod();
            var l1 = e1.DefineLabel();
            var l2 = e1.DefineLabel();
            var l3 = e1.DefineLabel();

            e1.LoadArgument(0);
            e1.Branch(l1);
            
            e1.MarkLabel(l2);
            e1.CastClass<object>();
            e1.CallVirtual(typeof(object).GetMethod("ToString"));
            e1.Branch(l3);

            e1.MarkLabel(l1);
            e1.Branch(l2);

            e1.MarkLabel(l3);
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate(out instrs);

            Assert.AreEqual("foo", d1("foo"));
            Assert.IsFalse(instrs.Contains("castclass"));
        }

        [TestMethod]
        public void ElideManyBranched()
        {
            var e1 = Emit<Func<string, string>>.NewDynamicMethod();

            e1.LoadArgument(0);

            for (var i = 0; i < 10; i++)
            {
                var l1 = e1.DefineLabel();
                var l2 = e1.DefineLabel();
                var l3 = e1.DefineLabel();
                e1.Branch(l1);
                
                e1.MarkLabel(l2);
                e1.Duplicate();
                e1.Pop();
                e1.Branch(l3);

                e1.MarkLabel(l1);
                e1.Branch(l2);

                e1.MarkLabel(l3);
            }

            e1.CastClass<string>();
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate(out instrs);

            Assert.AreEqual("foo", d1("foo"));
            Assert.IsFalse(instrs.Contains("castclass"));
        }

        [TestMethod]
        public void ManyBranched()
        {
            var e1 = Emit<Func<object, string>>.NewDynamicMethod();

            e1.LoadArgument(0);

            for (var i = 0; i < 10; i++)
            {
                var l1 = e1.DefineLabel();
                var l2 = e1.DefineLabel();
                var l3 = e1.DefineLabel();
                e1.Branch(l1);

                e1.MarkLabel(l2);
                e1.Duplicate();
                e1.Pop();
                e1.Branch(l3);

                e1.MarkLabel(l1);
                e1.Branch(l2);

                e1.MarkLabel(l3);
            }

            e1.CastClass<string>();
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate(out instrs);

            Assert.AreEqual("foo", d1("foo"));
            Assert.IsTrue(instrs.Contains("castclass"));
        }

        [TestMethod]
        public void Simple()
        {
            var e1 = Emit<Func<object, string>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.CastClass<string>();
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate(out instrs);

            Assert.AreEqual(null, d1(null));
            Assert.AreEqual("hello", d1("hello"));
            Assert.IsTrue(instrs.Contains("castclass"));
        }
    }
}
