using Sigil.NonGeneric;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using Xunit;

namespace SigilTests
{
    public partial class Branches
    {
        [Fact]
        public void ScanNonGeneric()
        {
            var terms = new[] { "hello", "world", "fizz", "buzz" };
            var strEq = typeof(string).GetMethod("Equals", new[] { typeof(object) });

            var e1 = Emit.NewDynamicMethod(typeof(int), new[] { typeof(string) });

            var done = e1.DefineLabel("done");

            e1.LoadArgument(0);                     // string

            for (var i = 0; i < terms.Length; i++)
            {
                var next = e1.DefineLabel("next_" + i);

                e1.Duplicate();                     // string string
                e1.LoadConstant(terms[i]);          // string string string
                e1.Call(strEq);                     // int string
                e1.BranchIfFalse(next);             // string

                e1.Pop();                           // --empty--
                e1.LoadConstant(i);                 // int
                e1.Branch(done);                    // int

                e1.MarkLabel(next);                 // string
            }

            e1.Pop();                               // --empty--
            e1.LoadConstant(-1);                    // int

            e1.MarkLabel(done);                     // int
            e1.Return();                            // --empty--

            var d1 = e1.CreateDelegate<Func<string, int>>();

            Assert.Equal(-1, d1("whatever"));
            Assert.Equal(0, d1("hello"));
            Assert.Equal(1, d1("world"));
            Assert.Equal(2, d1("fizz"));
            Assert.Equal(3, d1("buzz"));
        }

        [Fact]
        public void ConditionalBranchOverNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes);
            var a = e1.DeclareLocal<int>("a");

            var l1 = e1.DefineLabel("l1");

            e1.LoadConstant("123");
            e1.LoadConstant(456);

            e1.LoadConstant(1);
            e1.BranchIfTrue(l1);

            e1.Pop();
            e1.LoadConstant(789);

            e1.MarkLabel(l1);
            e1.StoreLocal(a);
            e1.Call(typeof(int).GetMethod("Parse", new[] { typeof(string) }));
            e1.LoadLocal(a);
            e1.Add();

            e1.Return();

            var d1 = e1.CreateDelegate<Func<int>>();

            Assert.Equal(123 + 456, d1());
        }

        [Fact]
        public void ManyConditionalNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(void), Type.EmptyTypes);

            for (var i = 0; i < 100; i++)
            {
                var l1 = e1.DefineLabel();
                var l2 = e1.DefineLabel();

                e1.LoadConstant(0);
                e1.LoadConstant(1);
                e1.BranchIfGreater(l1);

                e1.LoadConstant(2);
                e1.LoadConstant(3);
                e1.BranchIfLess(l2);

                e1.MarkLabel(l1);
                e1.MarkLabel(l2);
            }

            e1.Return();

            var d1 = e1.CreateDelegate<Action>();
            d1();
        }

        [Fact]
        public void InMethodNonGeneric()
        {
            var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
            var mod = asm.DefineDynamicModule("Bar");
            var t = mod.DefineType("T");

            var e1 = Emit.BuildStaticMethod(typeof(string), new[] { typeof(int) }, t, "Static", MethodAttributes.Public);
            var gt = e1.DefineLabel("GreaterThan");
            var skip = e1.DefineLabel("Skip");

            e1.Branch(skip);
            e1.MarkLabel(skip);

            e1.LoadArgument(0);
            e1.LoadConstant(0);
            e1.BranchIfGreater(gt);

            e1.LoadConstant("less than or equal");
            e1.Return();

            e1.MarkLabel(gt);
            e1.LoadConstant("greater than");
            e1.Return();

            e1.CreateMethod();

            var type = t.CreateType();

            var d = type.GetMethod("Static");

            Func<int, string> d1 = x => (string)d.Invoke(null, new object[] { x });

            Assert.Equal("less than or equal", d1(0));
            Assert.Equal("less than or equal", d1(-100));
            Assert.Equal("greater than", d1(50));
        }

        [Fact]
        public void BranchingOverExceptionsNonGeneric()
        {
            {
                var hasNormalBranch = new Regex("^br ", RegexOptions.Multiline);

                for (var i = 0; i < 127 - 10; i++)
                {
                    var e1 = Emit.NewDynamicMethod(typeof(void), Type.EmptyTypes, "E1");
                    var end = e1.DefineLabel("end");

                    e1.Branch(end);

                    var t = e1.BeginExceptionBlock();
                    var c = e1.BeginCatchBlock<Exception>(t);
                    e1.Pop();
                    e1.EndCatchBlock(c);
                    e1.EndExceptionBlock(t);

                    for (var j = 0; j < i; j++)
                    {
                        e1.Nop();
                    }

                    e1.MarkLabel(end);
                    e1.Return();

                    var d1 = e1.CreateDelegate<Action>(out string instrs);

                    d1();

                    Assert.DoesNotMatch(hasNormalBranch, instrs);
                }
            }

            {
                var hasNormalBranch = new Regex("^br ", RegexOptions.Multiline);

                for (var i = 0; i < 127 - 16; i++)
                {
                    var e1 = Emit.NewDynamicMethod(typeof(void), Type.EmptyTypes, "E1");
                    var end = e1.DefineLabel("end");

                    e1.Branch(end);

                    var t = e1.BeginExceptionBlock();
                    var c = e1.BeginCatchBlock<Exception>(t);
                    e1.Pop();
                    e1.EndCatchBlock(c);
                    var f = e1.BeginFinallyBlock(t);
                    e1.EndFinallyBlock(f);
                    e1.EndExceptionBlock(t);

                    for (var j = 0; j < i; j++)
                    {
                        e1.Nop();
                    }

                    e1.MarkLabel(end);
                    e1.Return();

                    var d1 = e1.CreateDelegate<Action>(out string instrs);

                    d1();

                    Assert.DoesNotMatch(hasNormalBranch, instrs);
                }
            }
        }

        [Fact]
        public void LeaveDataOnStackBetweenBranchesNonGeneric()
        {
            var il = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes, "LeaveDataOnStackBetweenBranches");

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
            int i = il.CreateDelegate<Func<int>>()();
            Assert.Equal(4, i);
        }

        [Fact]
        public void ShortFormNonGeneric()
        {
            {
                var hasNormalBranch = new Regex("^br ", RegexOptions.Multiline);

                for (var i = 0; i < 127; i++)
                {
                    var e1 = Emit.NewDynamicMethod(typeof(void), Type.EmptyTypes, "E1");
                    var dead = e1.DefineLabel();
                    var end = e1.DefineLabel("end");

                    e1.Branch(end);

                    e1.MarkLabel(dead);

                    for (var j = 0; j < i; j++)
                    {
                        e1.Nop();
                    }

                    e1.MarkLabel(end);
                    e1.Return();

                    var d1 = e1.CreateDelegate<Action>(out string instrs);

                    d1();

                    Assert.DoesNotMatch(hasNormalBranch, instrs);
                }
            }
        }

        [Fact]
        public void ShortFormNoOptimizationsNonGeneric()
        {
            {
                var hasNormalBranch = new Regex("^br ", RegexOptions.Multiline);

                for (var i = 0; i < 127; i++)
                {
                    var e1 = Emit.NewDynamicMethod(typeof(void), Type.EmptyTypes, "E1");
                    var dead = e1.DefineLabel();
                    var end = e1.DefineLabel("end");

                    e1.Branch(end);

                    e1.MarkLabel(dead);

                    for (var j = 0; j < i; j++)
                    {
                        e1.Nop();
                    }

                    e1.MarkLabel(end);
                    e1.Return();

                    var d1 = e1.CreateDelegate<Action>(out string instrs, Sigil.OptimizationOptions.None);

                    d1();

                    Assert.Matches(hasNormalBranch, instrs);
                }
            }
        }

        [Fact]
        public void BinaryInputNonGeneric()
        {
            var emit = typeof(Emit);
            var branches =
                new[]
                {
                    emit.GetMethod("BranchIfEqual", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("BranchIfGreater", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("BranchIfGreaterOrEqual", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("BranchIfLess", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("BranchIfLessOrEqual", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("UnsignedBranchIfNotEqual", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("UnsignedBranchIfGreater", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("UnsignedBranchIfGreaterOrEqual", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("UnsignedBranchIfLess", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("UnsignedBranchIfLessOrEqual", new [] { typeof(Sigil.Label) })
                };

            foreach (var branch in branches)
            {
                var e1 = Emit.NewDynamicMethod(typeof(void), Type.EmptyTypes, "E1");
                var l = e1.DefineLabel();
                e1.LoadConstant(0);
                e1.LoadConstant(1);
                branch.Invoke(e1, new object[] { l });
                e1.MarkLabel(l);
                e1.Return();

                var d1 = e1.CreateDelegate<Action>();
                d1();
            }
        }

        [Fact]
        public void UnaryInputNonGeneric()
        {
            {
                var e1 = Emit.NewDynamicMethod(typeof(void), Type.EmptyTypes, "E1");
                var l = e1.DefineLabel();
                e1.LoadConstant(0);
                e1.BranchIfFalse(l);
                e1.MarkLabel(l);
                e1.Return();

                var d1 = e1.CreateDelegate<Action>();

                d1();
            }

            {
                var e2 = Emit.NewDynamicMethod(typeof(void), Type.EmptyTypes, "E1");
                var l = e2.DefineLabel();
                e2.LoadConstant(0);
                e2.BranchIfTrue(l);
                e2.MarkLabel(l);
                e2.Return();

                var d2 = e2.CreateDelegate<Action>();

                d2();
            }
        }

        [Fact]
        public void MultiLabelNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes, "E1");
            var d1 = e1.DefineLabel();
            var d2 = e1.DefineLabel();

            e1.LoadConstant(1);
            var one = e1.DefineLabel("one");
            e1.Branch(one);

            e1.MarkLabel(d1);

            e1.LoadConstant(2);
            e1.Add();

            e1.MarkLabel(one);
            var two = e1.DefineLabel("two");
            e1.Branch(two);

            e1.MarkLabel(d2);

            e1.LoadConstant(3);
            e1.Add();

            e1.MarkLabel(two);
            e1.Return();

            var del = e1.CreateDelegate<Func<int>>();

            Assert.Equal(1, del());
        }

        [Fact]
        public void BrSNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes, "E1");
            var dead = e1.DefineLabel();

            var after = e1.DefineLabel("after");
            e1.LoadConstant(456);
            e1.Branch(after);

            e1.MarkLabel(dead);

            e1.LoadConstant(111);
            e1.Add();

            e1.MarkLabel(after);
            e1.Return();

            var del = e1.CreateDelegate<Func<int>>();

            Assert.Equal(456, del());
        }

        [Fact]
        public void BrNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes, "E1");

            var after = e1.DefineLabel("after");
            var dead = e1.DefineLabel();
            e1.LoadConstant(111);
            e1.Branch(after);

            e1.MarkLabel(dead);
            e1.LoadConstant(123);   // Have to load something, or stacks are bogus
            for (var i = 0; i < 1000; i++)
            {
                e1.Nop();
            }

            e1.MarkLabel(after);
            e1.Return();

            var del = e1.CreateDelegate<Func<int>>();

            Assert.Equal(111, del());
        }

        [Fact]
        public void BeqSNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes, "E1");

            var after = e1.DefineLabel("after");
            e1.LoadConstant(314);
            e1.LoadConstant(456);
            e1.LoadConstant(456);
            e1.BranchIfEqual(after);

            e1.LoadConstant(111);
            e1.Add();

            e1.MarkLabel(after);
            e1.Return();

            var del = e1.CreateDelegate<Func<int>>();

            Assert.Equal(314, del());
        }

        [Fact]
        public void BeqNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes, "E1");

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

            var del = e1.CreateDelegate<Func<int>>();

            Assert.Equal(314, del());
        }
    }
}
