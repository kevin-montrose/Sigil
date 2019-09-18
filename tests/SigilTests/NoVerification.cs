using Sigil;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using Xunit;

namespace SigilTests
{
    public class NoVerification
    {
        [Fact]
        public void Simple()
        {
            var emit = Emit<Func<int>>.NewDynamicMethod(doVerify: false);

            emit.LoadConstant(1);
            emit.LoadConstant(2);
            emit.Add();
            emit.Return();

            var del = emit.CreateDelegate();

            Assert.Equal(3, del());
        }

        [Fact]
        public void Scan()
        {
            var terms = new[] { "hello", "world", "fizz", "buzz" };
            var strEq = typeof(string).GetMethod("Equals", new[] { typeof(object) });

            var e1 = Emit<Func<string, int>>.NewDynamicMethod(doVerify: false);

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

            var d1 = e1.CreateDelegate();

            Assert.Equal(-1, d1("whatever"));
            Assert.Equal(0, d1("hello"));
            Assert.Equal(1, d1("world"));
            Assert.Equal(2, d1("fizz"));
            Assert.Equal(3, d1("buzz"));
        }

        [Fact]
        public void ConditionalBranchOver()
        {
            var e1 = Emit<Func<int>>.NewDynamicMethod(doVerify: false);
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

            var d1 = e1.CreateDelegate();

            Assert.Equal(123 + 456, d1());
        }

        [Fact]
        public void ManyConditional()
        {
            var e1 = Emit<Action>.NewDynamicMethod(doVerify: false);

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

            var d1 = e1.CreateDelegate();
            d1();
        }

        [Fact]
        public void InMethod()
        {
            var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
            var mod = asm.DefineDynamicModule("Bar");
            var t = mod.DefineType("T");

            var e1 = Emit<Func<int, string>>.BuildStaticMethod(t, "Static", MethodAttributes.Public, doVerify: false);
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
        public void BranchingOverExceptions()
        {
            {
                var hasNormalBranch = new Regex("^br ", RegexOptions.Multiline);

                for (var i = 0; i < 127 - 10; i++)
                {
                    var e1 = Emit<Action>.NewDynamicMethod("E1", doVerify: false);
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

                    var d1 = e1.CreateDelegate(out string instrs);

                    d1();

                    Assert.DoesNotMatch(hasNormalBranch, instrs);
                }
            }

            {
                var hasNormalBranch = new Regex("^br ", RegexOptions.Multiline);

                for (var i = 0; i < 127 - 16; i++)
                {
                    var e1 = Emit<Action>.NewDynamicMethod("E1", doVerify: false);
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

                    var d1 = e1.CreateDelegate(out string instrs);

                    d1();

                    Assert.DoesNotMatch(hasNormalBranch, instrs);
                }
            }
        }

        [Fact]
        public void LeaveDataOnStackBetweenBranches()
        {
            var il = Emit<Func<int>>.NewDynamicMethod("LeaveDataOnStackBetweenBranches", doVerify: false);

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
            Assert.Equal(4, i);
        }

        [Fact]
        public void CallIndirectSimple()
        {
            var foo = typeof(CallIndirect).GetMethod("Foo");

            var e1 = Emit<Func<string>>.NewDynamicMethod("E1", doVerify: false);
            e1.LoadConstant(3);
            e1.LoadFunctionPointer(foo);
            e1.CallIndirect<string, int>(foo.CallingConvention);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal("BarBarBar", d1());
        }

        public class VirtualClass
        {
            public override string ToString()
            {
                return "I'm Virtual!";
            }
        }

        [Fact]
        public void Virtual()
        {
            var toString = typeof(object).GetMethod("ToString");

            var e1 = Emit<Func<string>>.NewDynamicMethod("E1", doVerify: false);
            e1.NewObject<VirtualClass>();
            e1.Duplicate();
            e1.LoadVirtualFunctionPointer(toString);
            e1.CallIndirect<string>(toString.CallingConvention);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal("I'm Virtual!", d1());
        }
    }
}
