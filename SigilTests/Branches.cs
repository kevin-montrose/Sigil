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
    public class Branches
    {
        [TestMethod]
        public void MultiLabel()
        {
            var e1 = Emit<Func<int>>.NewDynamicMethod("E1");

            e1.LoadConstant(1);
            var one = e1.DefineLabel("one");
            e1.Branch(one);
            
            e1.LoadConstant(2);
            e1.Add();

            e1.MarkLabel(one);
            var two = e1.DefineLabel("two");
            e1.Branch(two);

            e1.LoadConstant(3);
            e1.Add();

            e1.MarkLabel(two);
            e1.Return();

            var del = e1.CreateDelegate();

            Assert.AreEqual(1, del());
        }

        [TestMethod]
        public void BrS()
        {
            var e1 = Emit<Func<int>>.NewDynamicMethod("E1");

            var after = e1.DefineLabel("after");
            e1.LoadConstant(456);
            e1.Branch(after);
            
            e1.LoadConstant(111);
            e1.Add();

            e1.MarkLabel(after);
            e1.Return();

            var del = e1.CreateDelegate();

            Assert.AreEqual(456, del());
        }

        [TestMethod]
        public void Br()
        {
            var e1 = Emit<Func<int>>.NewDynamicMethod("E1");

            var after = e1.DefineLabel("after");
            e1.LoadConstant(111);
            e1.Branch(after);

            for (var i = 0; i < 1000; i++)
            {
                e1.Nop();
            }

            e1.MarkLabel(after);
            e1.Return();

            var del = e1.CreateDelegate();

            Assert.AreEqual(111, del());
        }

        [TestMethod]
        public void BeqS()
        {
            var e1 = Emit<Func<int>>.NewDynamicMethod("E1");

            var after = e1.DefineLabel("after");
            e1.LoadConstant(314);
            e1.LoadConstant(456);
            e1.LoadConstant(456);
            e1.BranchIfEqual(after);

            e1.LoadConstant(111);
            e1.Add();

            e1.MarkLabel(after);
            e1.Return();

            var del = e1.CreateDelegate();

            Assert.AreEqual(314, del());
        }

        [TestMethod]
        public void Beq()
        {
            var e1 = Emit<Func<int>>.NewDynamicMethod("E1");

            var after = e1.DefineLabel("after");
            e1.LoadConstant(314);
            e1.LoadConstant(456);
            e1.LoadConstant(456);
            e1.BranchIfEqual(after);

            for (var i = 0; i < 1234; i++)
            {
                e1.Nop();
            }

            e1.LoadConstant(111);
            e1.Add();

            e1.MarkLabel(after);
            e1.Return();

            var del = e1.CreateDelegate();

            Assert.AreEqual(314, del());
        }
    }
}
