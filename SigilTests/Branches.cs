using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestClass, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Branches
    {
        [TestMethod]
        public void LeaveDataOnStackBetweenBranches()
        {
            var il = Emit<Func<int>>.NewDynamicMethod("LeaveDataOnStackBetweenBranches");

            Sigil.Label b0 = il.DefineLabel("b0"), b1 = il.DefineLabel("b1"), b2 = il.DefineLabel("b2");
            il.LoadConstant("abc");
            il.Branch(b0); // jump to b0 with "abc"

            il.MarkLabel(b1); // incoming: 3
            il.LoadConstant(4);
            il.Call(typeof(Math).GetMethod("Max", new[] { typeof(int), typeof(int) }));
            il.Branch(b2); // jump to b2 with 4

            il.MarkLabel(b0); // incoming: "abc"
            il.CallVirtual(typeof(string).GetProperty("Length").GetGetMethod());
            il.Branch(b1); // jump to b1 with 3

            il.MarkLabel(b2); // incoming: 4
            il.Return();
            int i = il.CreateDelegate()();
            Assert.AreEqual(4, i);
        }

        [TestMethod]
        public void LeaveDataOnStackBetweenBranches_OldSchool()
        {
            var dm = new System.Reflection.Emit.DynamicMethod("foo", typeof(int), null);
            var il = dm.GetILGenerator();
            System.Reflection.Emit.Label b0 = il.DefineLabel(), b1 = il.DefineLabel(), b2 = il.DefineLabel();
            il.Emit(System.Reflection.Emit.OpCodes.Ldstr, "abc");
            il.Emit(System.Reflection.Emit.OpCodes.Br, b0); // jump to b0 with "abc"

            il.MarkLabel(b1); // incoming: 3
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_4);
            il.EmitCall(System.Reflection.Emit.OpCodes.Call, typeof(Math).GetMethod("Max", new[] { typeof(int), typeof(int) }), null);
            il.Emit(System.Reflection.Emit.OpCodes.Br, b2); // jump to b2 with 4

            il.MarkLabel(b0); // incoming: "abc"
            il.EmitCall(System.Reflection.Emit.OpCodes.Callvirt, typeof(string).GetProperty("Length").GetGetMethod(), null);
            il.Emit(System.Reflection.Emit.OpCodes.Br, b1); // jump to b1 with 3

            il.MarkLabel(b2); // incoming: 4
            il.Emit(System.Reflection.Emit.OpCodes.Ret);
            int i = ((Func<int>)dm.CreateDelegate(typeof(Func<int>)))();
            Assert.AreEqual(4, i);
        }


        [TestMethod]
        public void ShortForm()
        {
            {
                var hasNormalBranch = new Regex("^br ", RegexOptions.Multiline);

                for (var i = 0; i < 127; i++)
                {
                    var e1 = Emit<Action>.NewDynamicMethod("E1");
                    var end = e1.DefineLabel("end");

                    e1.Branch(end);

                    for (var j = 0; j < i; j++)
                    {
                        e1.Nop();
                    }

                    e1.MarkLabel(end);
                    e1.Return();

                    string instrs;
                    var d1 = e1.CreateDelegate(out instrs);

                    d1();

                    var shouldFail = hasNormalBranch.IsMatch(instrs);

                    if (shouldFail)
                    {
                        Assert.Fail();
                    }
                }
            }
        }

        [TestMethod]
        public void BinaryInput()
        {
            var emit = typeof(Emit<Action>);
            var branches =
                new[]
                {
                    emit.GetMethod("BranchIfEqual"),
                    emit.GetMethod("BranchIfGreater"),
                    emit.GetMethod("BranchIfGreaterOrEqual"),
                    emit.GetMethod("BranchIfLess"),
                    emit.GetMethod("BranchIfLessOrEqual"),
                    emit.GetMethod("UnsignedBranchIfNotEqual"),
                    emit.GetMethod("UnsignedBranchIfGreater"),
                    emit.GetMethod("UnsignedBranchIfGreaterOrEqual"),
                    emit.GetMethod("UnsignedBranchIfLess"),
                    emit.GetMethod("UnsignedBranchIfLessOrEqual")
                };

            foreach (var branch in branches)
            {
                var e1 = Emit<Action>.NewDynamicMethod("E1");
                var l = e1.DefineLabel();
                e1.LoadConstant(0);
                e1.LoadConstant(1);
                branch.Invoke(e1, new object[] { l });
                e1.MarkLabel(l);
                e1.Return();

                var d1 = e1.CreateDelegate();
                d1();
            }
        }

        [TestMethod]
        public void UnaryInput()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod("E1");
                var l = e1.DefineLabel();
                e1.LoadConstant(0);
                e1.BranchIfFalse(l);
                e1.MarkLabel(l);
                e1.Return();

                var d1 = e1.CreateDelegate();

                d1();
            }

            {
                var e2 = Emit<Action>.NewDynamicMethod("E1");
                var l = e2.DefineLabel();
                e2.LoadConstant(0);
                e2.BranchIfTrue(l);
                e2.MarkLabel(l);
                e2.Return();

                var d2 = e2.CreateDelegate();

                d2();
            }
        }

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
