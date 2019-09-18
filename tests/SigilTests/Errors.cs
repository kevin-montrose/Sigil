using Sigil;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit;

namespace SigilTests
{
    public partial class Errors
    {
        [Fact]
        public void OverflowsOnlyAcceptsIntegers()
        {
            var ex1 =
                Assert.Throws<SigilVerificationException>(
                    () =>
                    {
                        var e1 = Emit<Func<double, double, double>>.NewDynamicMethod("E1");
                        e1.LoadArgument(0);
                        e1.LoadArgument(1);
                        e1.MultiplyOverflow();
                    }
                );
            Assert.Equal("MultiplyOverflow expected an int, long, or native int; found double", ex1.Message);

            var ex2 =
                Assert.Throws<SigilVerificationException>(
                    () =>
                    {
                        var e1 = Emit<Func<double, double, double>>.NewDynamicMethod("E1");
                        e1.LoadArgument(0);
                        e1.LoadArgument(1);
                        e1.UnsignedMultiplyOverflow();
                    }
                );
            Assert.Equal("UnsignedMultiplyOverflow expected an int, long, or native int; found double", ex2.Message);

            var ex3 =
                Assert.Throws<SigilVerificationException>(
                    () =>
                    {
                        var e1 = Emit<Func<double, double, double>>.NewDynamicMethod("E1");
                        e1.LoadArgument(0);
                        e1.LoadArgument(1);
                        e1.AddOverflow();
                    }
                );
            Assert.Equal("AddOverflow expected an int, long, or native int; found double", ex3.Message);

            var ex4 =
                Assert.Throws<SigilVerificationException>(
                    () =>
                    {
                        var e1 = Emit<Func<double, double, double>>.NewDynamicMethod("E1");
                        e1.LoadArgument(0);
                        e1.LoadArgument(1);
                        e1.UnsignedAddOverflow();
                    }
                );
            Assert.Equal("UnsignedAddOverflow expected an int, long, or native int; found double", ex4.Message);

            var ex5 =
                Assert.Throws<SigilVerificationException>(
                    () =>
                    {
                        var e1 = Emit<Func<double, double, double>>.NewDynamicMethod("E1");
                        e1.LoadArgument(0);
                        e1.LoadArgument(1);
                        e1.SubtractOverflow();
                    }
                );
            Assert.Equal("SubtractOverflow expected an int, long, or native int; found double", ex5.Message);

            var ex6 =
                Assert.Throws<SigilVerificationException>(
                    () =>
                    {
                        var e1 = Emit<Func<double, double, double>>.NewDynamicMethod("E1");
                        e1.LoadArgument(0);
                        e1.LoadArgument(1);
                        e1.UnsignedSubtractOverflow();
                    }
                );
            Assert.Equal("UnsignedSubtractOverflow expected an int, long, or native int; found double", ex6.Message);
        }

        private class _CallNonBaseClassConstructor1
        {
            public string Foo;

            public _CallNonBaseClassConstructor1(string foo)
            {
                Foo = foo;
            }
        }

        private class _CallNonBaseClassConstructor2 : _CallNonBaseClassConstructor1
        {
            public _CallNonBaseClassConstructor2(string bar) : base(bar + bar) { }
        }

        [Fact]
        public void CallNonBaseClassConstructor()
        {
            var assembly = DefineDynamicAssembly();

            var module = assembly.DefineDynamicModule("RuntimeModule");
            var type = module.DefineType("_CallNonBaseClassConstructor3", TypeAttributes.Class, typeof(_CallNonBaseClassConstructor2));

            var ctor = Emit<Action>.BuildConstructor(type, MethodAttributes.Public);

            var baseBaseCtor = typeof(_CallNonBaseClassConstructor1).GetConstructor(new[] { typeof(string) });

            ctor.LoadArgument(0);
            ctor.LoadConstant("abc123");

            var ex = Assert.Throws<SigilVerificationException>(() => ctor.Call(baseBaseCtor));
            Assert.Equal("Only constructors defined in the current class or it's base class may be called", ex.Message);
        }

        internal static System.Reflection.Emit.AssemblyBuilder DefineDynamicAssembly([CallerMemberName]string name=null)
        {
#if NETCOREAPP
            return System.Reflection.Emit.AssemblyBuilder.DefineDynamicAssembly(
                    new AssemblyName(name),
                    System.Reflection.Emit.AssemblyBuilderAccess.RunAndCollect
                );
#else
            return AppDomain.CurrentDomain.DefineDynamicAssembly(
                    new AssemblyName(name),
                    System.Reflection.Emit.AssemblyBuilderAccess.RunAndCollect
                );
#endif

        }

        [Fact]
        public void CallNonBaseClassConstructorNonGeneric()
        {
            var assembly = DefineDynamicAssembly();
            var module = assembly.DefineDynamicModule("RuntimeModule");
            var type = module.DefineType("_CallNonBaseClassConstructor3", TypeAttributes.Class, typeof(_CallNonBaseClassConstructor2));

            var ctor = Sigil.NonGeneric.Emit.BuildConstructor(Type.EmptyTypes, type, MethodAttributes.Public);

            var baseBaseCtor = typeof(_CallNonBaseClassConstructor1).GetConstructor(new[] { typeof(string) });

            ctor.LoadArgument(0);
            ctor.LoadConstant("abc123");

            var ex = Assert.Throws<SigilVerificationException>(() => ctor.Call(baseBaseCtor));
            Assert.Equal("Only constructors defined in the current class or it's base class may be called", ex.Message);
        }

        private class _CallContructorFromNonConstructor
        {
            public _CallContructorFromNonConstructor() { }
        }

        [Fact]
        public void CallContructorFromNonConstructor()
        {
            var cons = typeof(_CallContructorFromNonConstructor).GetConstructor(Type.EmptyTypes);

            var e1 = Emit<Action>.NewDynamicMethod();
            e1.NewObject(cons);

            var ex = Assert.Throws<SigilVerificationException>(() => e1.Call(cons));
            Assert.Equal("Constructors may only be called directly from within a constructor, use NewObject to allocate a new object with a specific constructor.", ex.Message);
        }

        [Fact]
        public void DisassemblingClosure()
        {
            var ret = 4;
            Func<int> d = () => ret;

            var ops = Disassembler<Func<int>>.Disassemble(d);
            Assert.False(ops.CanEmit);

            var ex = Assert.Throws<InvalidOperationException>(() => ops.EmitAll());
            Assert.Equal("Cannot emit this DisassembledOperations object, check CanEmit before calling any Emit methods", ex.Message);
        }

        [Fact]
        public void BadNonTerminalReturn()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            var l1 = e1.DefineLabel("l1");
            var l2 = e1.DefineLabel("l2");

            e1.Branch(l1);

            e1.MarkLabel(l2);
            e1.Return();

            e1.MarkLabel(l1);
            e1.LoadConstant(1);
            e1.BranchIfTrue(l2);

            var ex = Assert.Throws<SigilVerificationException>(() => e1.CreateDelegate());
            Assert.Equal("All execution paths must end with Return", ex.Message);
            Assert.Equal("Bad Path\r\n========\r\n__start\r\nl1\r\n\r\nInstructions\r\n============\r\nbr.s l1\r\n\r\nl2:\r\nret\r\n\r\nl1:\r\nldc.i4.1\r\nbrtrue.s l2\r\n", ex.GetDebugInfo());
        }

        [Fact]
        public void BadBranchManyConditional()
        {
            var e1 = Emit<Action>.NewDynamicMethod();

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

            var ex = Assert.Throws<SigilVerificationException>(() => e1.Pop());
            var f = ex.GetDebugInfo();
            Assert.Equal("Pop expects a value on the stack, but it was empty", ex.Message);
            Assert.Equal("Stack\r\n=====\r\n--empty--\r\n\r\nInstructions\r\n============\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label0\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label1\r\n\r\n_label0:\r\n\r\n_label1:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label2\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label3\r\n\r\n_label2:\r\n\r\n_label3:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label4\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label5\r\n\r\n_label4:\r\n\r\n_label5:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label6\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label7\r\n\r\n_label6:\r\n\r\n_label7:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label8\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label9\r\n\r\n_label8:\r\n\r\n_label9:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label10\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label11\r\n\r\n_label10:\r\n\r\n_label11:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label12\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label13\r\n\r\n_label12:\r\n\r\n_label13:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label14\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label15\r\n\r\n_label14:\r\n\r\n_label15:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label16\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label17\r\n\r\n_label16:\r\n\r\n_label17:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label18\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label19\r\n\r\n_label18:\r\n\r\n_label19:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label20\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label21\r\n\r\n_label20:\r\n\r\n_label21:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label22\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label23\r\n\r\n_label22:\r\n\r\n_label23:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label24\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label25\r\n\r\n_label24:\r\n\r\n_label25:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label26\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label27\r\n\r\n_label26:\r\n\r\n_label27:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label28\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label29\r\n\r\n_label28:\r\n\r\n_label29:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label30\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label31\r\n\r\n_label30:\r\n\r\n_label31:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label32\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label33\r\n\r\n_label32:\r\n\r\n_label33:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label34\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label35\r\n\r\n_label34:\r\n\r\n_label35:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label36\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label37\r\n\r\n_label36:\r\n\r\n_label37:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label38\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label39\r\n\r\n_label38:\r\n\r\n_label39:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label40\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label41\r\n\r\n_label40:\r\n\r\n_label41:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label42\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label43\r\n\r\n_label42:\r\n\r\n_label43:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label44\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label45\r\n\r\n_label44:\r\n\r\n_label45:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label46\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label47\r\n\r\n_label46:\r\n\r\n_label47:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label48\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label49\r\n\r\n_label48:\r\n\r\n_label49:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label50\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label51\r\n\r\n_label50:\r\n\r\n_label51:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label52\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label53\r\n\r\n_label52:\r\n\r\n_label53:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label54\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label55\r\n\r\n_label54:\r\n\r\n_label55:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label56\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label57\r\n\r\n_label56:\r\n\r\n_label57:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label58\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label59\r\n\r\n_label58:\r\n\r\n_label59:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label60\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label61\r\n\r\n_label60:\r\n\r\n_label61:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label62\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label63\r\n\r\n_label62:\r\n\r\n_label63:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label64\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label65\r\n\r\n_label64:\r\n\r\n_label65:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label66\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label67\r\n\r\n_label66:\r\n\r\n_label67:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label68\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label69\r\n\r\n_label68:\r\n\r\n_label69:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label70\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label71\r\n\r\n_label70:\r\n\r\n_label71:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label72\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label73\r\n\r\n_label72:\r\n\r\n_label73:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label74\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label75\r\n\r\n_label74:\r\n\r\n_label75:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label76\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label77\r\n\r\n_label76:\r\n\r\n_label77:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label78\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label79\r\n\r\n_label78:\r\n\r\n_label79:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label80\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label81\r\n\r\n_label80:\r\n\r\n_label81:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label82\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label83\r\n\r\n_label82:\r\n\r\n_label83:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label84\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label85\r\n\r\n_label84:\r\n\r\n_label85:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label86\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label87\r\n\r\n_label86:\r\n\r\n_label87:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label88\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label89\r\n\r\n_label88:\r\n\r\n_label89:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label90\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label91\r\n\r\n_label90:\r\n\r\n_label91:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label92\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label93\r\n\r\n_label92:\r\n\r\n_label93:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label94\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label95\r\n\r\n_label94:\r\n\r\n_label95:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label96\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label97\r\n\r\n_label96:\r\n\r\n_label97:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label98\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label99\r\n\r\n_label98:\r\n\r\n_label99:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label100\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label101\r\n\r\n_label100:\r\n\r\n_label101:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label102\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label103\r\n\r\n_label102:\r\n\r\n_label103:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label104\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label105\r\n\r\n_label104:\r\n\r\n_label105:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label106\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label107\r\n\r\n_label106:\r\n\r\n_label107:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label108\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label109\r\n\r\n_label108:\r\n\r\n_label109:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label110\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label111\r\n\r\n_label110:\r\n\r\n_label111:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label112\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label113\r\n\r\n_label112:\r\n\r\n_label113:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label114\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label115\r\n\r\n_label114:\r\n\r\n_label115:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label116\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label117\r\n\r\n_label116:\r\n\r\n_label117:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label118\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label119\r\n\r\n_label118:\r\n\r\n_label119:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label120\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label121\r\n\r\n_label120:\r\n\r\n_label121:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label122\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label123\r\n\r\n_label122:\r\n\r\n_label123:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label124\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label125\r\n\r\n_label124:\r\n\r\n_label125:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label126\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label127\r\n\r\n_label126:\r\n\r\n_label127:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label128\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label129\r\n\r\n_label128:\r\n\r\n_label129:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label130\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label131\r\n\r\n_label130:\r\n\r\n_label131:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label132\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label133\r\n\r\n_label132:\r\n\r\n_label133:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label134\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label135\r\n\r\n_label134:\r\n\r\n_label135:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label136\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label137\r\n\r\n_label136:\r\n\r\n_label137:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label138\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label139\r\n\r\n_label138:\r\n\r\n_label139:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label140\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label141\r\n\r\n_label140:\r\n\r\n_label141:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label142\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label143\r\n\r\n_label142:\r\n\r\n_label143:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label144\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label145\r\n\r\n_label144:\r\n\r\n_label145:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label146\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label147\r\n\r\n_label146:\r\n\r\n_label147:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label148\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label149\r\n\r\n_label148:\r\n\r\n_label149:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label150\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label151\r\n\r\n_label150:\r\n\r\n_label151:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label152\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label153\r\n\r\n_label152:\r\n\r\n_label153:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label154\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label155\r\n\r\n_label154:\r\n\r\n_label155:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label156\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label157\r\n\r\n_label156:\r\n\r\n_label157:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label158\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label159\r\n\r\n_label158:\r\n\r\n_label159:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label160\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label161\r\n\r\n_label160:\r\n\r\n_label161:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label162\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label163\r\n\r\n_label162:\r\n\r\n_label163:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label164\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label165\r\n\r\n_label164:\r\n\r\n_label165:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label166\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label167\r\n\r\n_label166:\r\n\r\n_label167:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label168\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label169\r\n\r\n_label168:\r\n\r\n_label169:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label170\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label171\r\n\r\n_label170:\r\n\r\n_label171:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label172\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label173\r\n\r\n_label172:\r\n\r\n_label173:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label174\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label175\r\n\r\n_label174:\r\n\r\n_label175:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label176\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label177\r\n\r\n_label176:\r\n\r\n_label177:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label178\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label179\r\n\r\n_label178:\r\n\r\n_label179:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label180\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label181\r\n\r\n_label180:\r\n\r\n_label181:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label182\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label183\r\n\r\n_label182:\r\n\r\n_label183:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label184\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label185\r\n\r\n_label184:\r\n\r\n_label185:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label186\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label187\r\n\r\n_label186:\r\n\r\n_label187:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label188\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label189\r\n\r\n_label188:\r\n\r\n_label189:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label190\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label191\r\n\r\n_label190:\r\n\r\n_label191:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label192\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label193\r\n\r\n_label192:\r\n\r\n_label193:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label194\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label195\r\n\r\n_label194:\r\n\r\n_label195:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label196\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label197\r\n\r\n_label196:\r\n\r\n_label197:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label198\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label199\r\n\r\n_label198:\r\n\r\n_label199:\r\n", f);
        }

        [Fact]
        public void BadOptimizationOptions()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            e1.Return();

            var ex = Assert.Throws<ArgumentException>(() => e1.CreateDelegate((OptimizationOptions)123));
            Assert.Equal("optimizationOptions contained unknown flags, found 123", ex.Message);
        }

        [Fact]
        public void BadManyBranch()
        {
            var e1 = Emit<Func<string, string>>.NewDynamicMethod();

            e1.LoadArgument(0);

            for (var i = 0; i < 100; i++)
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

            e1.Pop();

            var ex = Assert.Throws<SigilVerificationException>(() => e1.Return());
            Assert.Equal("Return expects a value on the stack, but it was empty", ex.Message);
        }

        [Fact]
        public void BadDoubleBranch()
        {
            var e1 = Emit<Func<string, string>>.NewDynamicMethod();
            var l1 = e1.DefineLabel("l1");
            var l2 = e1.DefineLabel("l2");
            var l3 = e1.DefineLabel("l3");

            e1.LoadArgument(0);
            e1.Branch(l1);

            e1.MarkLabel(l2);
            e1.Pop();
            e1.CallVirtual(typeof(object).GetMethod("ToString"));
            e1.Branch(l3);

            e1.MarkLabel(l1);

            var ex = Assert.Throws<SigilVerificationException>(() => e1.Branch(l2));
            Assert.Equal("Branch expects a value on the stack, but it was empty", ex.Message);
            var debug = ex.GetDebugInfo();
            Assert.Equal("Stack\r\n=====\r\n--empty--\r\n\r\nInstructions\r\n============\r\nldarg.0\r\nbr l1\r\n\r\nl2:\r\npop\r\ncallvirt System.String ToString()  // relevant instruction\r\nbr l3\r\n\r\nl1:\r\nbr l2\r\n", debug);
        }

        [Fact]
        public void BadStackVals()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.LoadConstant(1);
                e1.LoadConstant("foo");

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Add());
                var debug = ex.GetDebugInfo();
                Assert.Equal("Stack\r\n=====\r\nSystem.String  // bad value\r\nint\r\n\r\nInstructions\r\n============\r\nldc.i4.1\r\nldstr 'foo'\r\n", debug);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.LoadConstant("foo");
                e1.LoadConstant(1);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Add());
                var debug = ex.GetDebugInfo();
                Assert.Equal("Stack\r\n=====\r\nint\r\nSystem.String  // bad value\r\n\r\nInstructions\r\n============\r\nldstr 'foo'\r\nldc.i4.1\r\n", debug);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var substring = typeof(string).GetMethod("Substring", new[] { typeof(int), typeof(int) });
                e1.LoadConstant("foo");
                e1.LoadConstant(1);
                e1.LoadConstant(2.0);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Call(substring));
                var debug = ex.GetDebugInfo();
                Assert.Equal("Stack\r\n=====\r\ndouble  // bad value\r\nint\r\nSystem.String\r\n\r\nInstructions\r\n============\r\nldstr 'foo'\r\nldc.i4.1\r\nldc.r8 2\r\n", debug);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var substring = typeof(string).GetMethod("Substring", new[] { typeof(int), typeof(int) });
                e1.LoadConstant("foo");
                e1.LoadConstant(1f);
                e1.LoadConstant(2);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Call(substring));
                var debug = ex.GetDebugInfo();
                Assert.Equal("Stack\r\n=====\r\nint\r\nfloat  // bad value\r\nSystem.String\r\n\r\nInstructions\r\n============\r\nldstr 'foo'\r\nldc.r4 1\r\nldc.i4.2\r\n", debug);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var substring = typeof(string).GetMethod("Substring", new[] { typeof(int), typeof(int) });
                e1.NewObject<object>();
                e1.LoadConstant(1);
                e1.LoadConstant(2);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Call(substring));
                var debug = ex.GetDebugInfo();
                Assert.Equal("Stack\r\n=====\r\nint\r\nint\r\nSystem.Object  // bad value\r\n\r\nInstructions\r\n============\r\nnewobj Void .ctor()\r\nldc.i4.1\r\nldc.i4.2\r\n", debug);
            }
        }

        [Fact]
        public void LabelDiffStack()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            var l = e1.DefineLabel();

            e1.LoadConstant(1);
            e1.BranchIfFalse(l);
            e1.LoadConstant(4);

            var ex = Assert.Throws<SigilVerificationException>(() => e1.MarkLabel(l));
            var f = ex.GetDebugInfo();
            Assert.Equal("MarkLabel expects 0 values on the stack", ex.Message);
            Assert.Equal("\r\nInstructions\r\n============\r\nldc.i4.1\r\nbrfalse _label0\r\nldc.i4.4\r\n", f);
        }

        [Fact]
        public void BranchDiffStack()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            var l = e1.DefineLabel();

            e1.LoadConstant(1);
            e1.MarkLabel(l);
            e1.Pop();
            e1.LoadConstant("123");

            var ex = Assert.Throws<SigilVerificationException>(() => e1.Branch(l));
            var f = ex.GetDebugInfo();
            Assert.Equal("Branch expected an int; found System.String", ex.Message);
            Assert.Equal("\r\nInstructions\r\n============\r\nldc.i4.1\r\n\r\n_label0:\r\npop\r\nldstr '123'\r\nbr _label0\r\n", f);
        }

        [Fact]
        public void Unreachable()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            var l = e1.DefineLabel();

            e1.Branch(l);

            var ex = Assert.Throws<SigilVerificationException>(() => e1.Pop());
            Assert.Equal("Unreachable code detected", ex.Message);
        }

        [Fact]
        public void WriteLine()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.WriteLine(null));
                Assert.Equal("line", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.WriteLine("foo", null));
                Assert.Equal("locals", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var e2 = Emit<Action>.NewDynamicMethod();
                var a = e2.DeclareLocal<int>();

                var ex = Assert.Throws<ArgumentException>(() => e1.WriteLine("foo", a));
                Assert.Equal("System.Int32 _local0 is not owned by this Emit, and thus cannot be used", ex.Message);
            }
        }

        [Fact]
        public void DoubleLabelDeclaration()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            e1.DefineLabel("a");

            var ex = Assert.Throws<InvalidOperationException>(() => e1.DefineLabel("a"));
            Assert.Equal("Label with name 'a' already exists", ex.Message);
        }

        [Fact]
        public void DoubleLocalDeclaration()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            var a = e1.DeclareLocal<int>("a");

            var ex = Assert.Throws<InvalidOperationException>(() => e1.DeclareLocal<string>("a"));
            Assert.Equal("Local with name 'a' already exists", ex.Message);

            a.Dispose();

            e1.DeclareLocal<string>("a");
        }

        [Fact]
        public void GenericThisCall()
        {
            {
                var e1 = Emit<Action<Action<string>>>.NewDynamicMethod("E1");
                var invoke = typeof(Action<int>).GetMethod("Invoke");

                e1.LoadArgument(0);
                e1.LoadConstant(1);
                var ex = Assert.Throws<SigilVerificationException>(() => e1.CallVirtual(invoke));
                Assert.Equal($"CallVirtual expected a {typeof(Action<int>).FullName}; found {typeof(Action<string>).FullName}", ex.Message);
            }
        }

        [Fact]
        public void Compare()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.LoadConstant(0);
                e1.LoadConstant(1.0f);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.CompareLessThan());
                Assert.Equal("CompareLessThan expected a float; found int", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.CompareLessThan());
                Assert.Equal("CompareLessThan expected a double, float, int, long, or native int; found System.Object", ex.Message);
            }
        }

        [Fact]
        public void AdditionalValidation()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var t = e1.BeginExceptionBlock();
                e1.Return();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.CreateDelegate());
                Assert.Equal("Unended ExceptionBlock Sigil.ExceptionBlock", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.Pop();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.CreateDelegate());
                Assert.Equal("All execution paths must end with Return", ex.Message);
            }
        }

        [Fact]
        public void UnboxAny()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.UnboxAny(null));
                Assert.Equal("valueType", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentException>(() => e1.UnboxAny(typeof(void)));
                Assert.Equal("Void cannot be boxed, and thus cannot be unboxed", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.UnboxAny<int>());
                Assert.Equal("UnboxAny expects a value on the stack, but it was empty", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.LoadConstant("hello");

                var ex = Assert.Throws<SigilVerificationException>(() => e1.UnboxAny<int>());
                Assert.Equal("UnboxAny expected a System.Object; found System.String", ex.Message);
            }
        }

        [Fact]
        public void Unbox()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.Unbox(null));
                Assert.Equal("valueType", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentException>(() => e1.Unbox(typeof(string)));
                Assert.Equal("Unbox expects a ValueType, found System.String", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentException>(() => e1.Unbox(typeof(void)));
                Assert.Equal("Void cannot be boxed, and thus cannot be unboxed", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Unbox<int>());
                Assert.Equal("Unbox expects a value on the stack, but it was empty", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.LoadConstant("hello");

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Unbox<int>());
                Assert.Equal("Unbox expected a System.Object; found System.String", ex.Message);
            }
        }

        [Fact]
        public void EndFinallyBlock()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.EndFinallyBlock(null));
                Assert.Equal("forFinally", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var e2 = Emit<Action>.NewDynamicMethod();
                var t = e2.BeginExceptionBlock();
                var f = e2.BeginFinallyBlock(t);

                var ex = Assert.Throws<ArgumentException>(() => e1.EndFinallyBlock(f));
                Assert.Equal("Sigil.FinallyBlock is not owned by this Emit, and thus cannot be used", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var t = e1.BeginExceptionBlock();
                var f = e1.BeginFinallyBlock(t);
                e1.EndFinallyBlock(f);

                var ex = Assert.Throws<InvalidOperationException>(() => e1.EndFinallyBlock(f));
                Assert.Equal("EndFinallyBlock expects an unclosed finally block, but Sigil.FinallyBlock is already closed", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var t = e1.BeginExceptionBlock();
                var f = e1.BeginFinallyBlock(t);
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.EndFinallyBlock(f));
                Assert.Equal("EndFinallyBlock expected the stack of be empty", ex.Message);
            }
        }

        [Fact]
        public void EndCatchBlock()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.EndCatchBlock(null));
                Assert.Equal("forCatch", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var e2 = Emit<Action>.NewDynamicMethod();

                var t = e2.BeginExceptionBlock();
                var c = e2.BeginCatchAllBlock(t);

                var ex = Assert.Throws<ArgumentException>(() => e1.EndCatchBlock(c));
                Assert.Equal("Sigil.CatchBlock is not owned by this Emit, and thus cannot be used", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var t = e1.BeginExceptionBlock();
                var c = e1.BeginCatchAllBlock(t);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.EndCatchBlock(c));
                Assert.Equal("EndCatchBlock expected the stack of be empty", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var t = e1.BeginExceptionBlock();
                var c = e1.BeginCatchAllBlock(t);
                e1.Pop();
                e1.EndCatchBlock(c);

                var ex = Assert.Throws<InvalidOperationException>(() => e1.EndCatchBlock(c));
                Assert.Equal("CatchBlock has already been ended", ex.Message);
            }
        }

        [Fact]
        public void EndExceptionBlock()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.EndExceptionBlock(null));
                Assert.Equal("forTry", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var e2 = Emit<Action>.NewDynamicMethod();
                var t = e2.BeginExceptionBlock();

                var ex = Assert.Throws<ArgumentException>(() => e1.EndExceptionBlock(t));
                Assert.Equal("Sigil.ExceptionBlock is not owned by this Emit, and thus cannot be used", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var t = e1.BeginExceptionBlock();
                var c = e1.BeginCatchAllBlock(t);
                e1.Pop();
                e1.EndCatchBlock(c);
                e1.EndExceptionBlock(t);

                var ex = Assert.Throws<InvalidOperationException>(() => e1.EndExceptionBlock(t));
                Assert.Equal("ExceptionBlock has already been ended", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var t = e1.BeginExceptionBlock();
                var c = e1.BeginCatchAllBlock(t);

                var ex = Assert.Throws<InvalidOperationException>(() => e1.EndExceptionBlock(t));
                Assert.Equal("Cannot end ExceptionBlock, CatchBlock Sigil.CatchBlock has not been ended", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var t = e1.BeginExceptionBlock();
                var f = e1.BeginFinallyBlock(t);

                var ex = Assert.Throws<InvalidOperationException>(() => e1.EndExceptionBlock(t));
                Assert.Equal("Cannot end ExceptionBlock, FinallyBlock Sigil.FinallyBlock has not been ended", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var t = e1.BeginExceptionBlock();

                var ex = Assert.Throws<InvalidOperationException>(() => e1.EndExceptionBlock(t));
                Assert.Equal("Cannot end ExceptionBlock without defining at least one of a catch or finally block", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var t1 = e1.BeginExceptionBlock();
                var t2 = e1.BeginExceptionBlock();

                var ex = Assert.Throws<InvalidOperationException>(() => e1.EndExceptionBlock(t1));
                Assert.Equal("Cannot end outer ExceptionBlock Sigil.ExceptionBlock while inner EmitExceptionBlock Sigil.ExceptionBlock is open", ex.Message);
            }
        }

        [Fact]
        public void Throw()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Throw());
                Assert.Equal("Throw expects a value on the stack, but it was empty", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Throw());
                Assert.Equal("Throw expected a System.Exception; found System.Object", ex.Message);
            }
        }

        [Fact]
        public void Switch()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.Switch((Label[])null));
                Assert.Equal("labels", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentException>(() => e1.Switch(new Label[0]));
                Assert.Equal("labels must have at least one element", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var e2 = Emit<Action>.NewDynamicMethod();
                var l = e2.DefineLabel();

                var ex = Assert.Throws<ArgumentException>(() => e1.Switch(l));
                Assert.Equal("_label0 is not owned by this Emit, and thus cannot be used", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var l = e1.DefineLabel();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Switch(l));
                Assert.Equal("Switch expects a value on the stack, but it was empty", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var l = e1.DefineLabel();

                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Switch(l));
                Assert.Equal("Switch expected an int, or native int; found System.Object", ex.Message);
            }
        }

        [Fact]
        public void StoreObject()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.StoreObject(null));
                Assert.Equal("valueType", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentException>(() => e1.StoreObject(typeof(string)));
                Assert.Equal("valueType must be a ValueType", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentException>(() => e1.StoreObject(typeof(int), unaligned: 3));
                Assert.Equal("unaligned must be null, 1, 2, or 4", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.StoreObject(typeof(int)));
                Assert.Equal("StoreObject expects 2 values on the stack", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.StoreObject(typeof(int)));
                Assert.Equal("StoreObject expected an int; found System.Object", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.LoadConstant(0);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.StoreObject(typeof(int)));
                Assert.Equal("StoreObject expected a native int, System.Int32&, or System.Int32*; found System.Object", ex.Message);
            }
        }

        [Fact]
        public void StoreLocal()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.StoreLocal((Sigil.Local)null));
                Assert.Equal("local", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var e2 = Emit<Action>.NewDynamicMethod();
                var l = e2.DeclareLocal<int>();

                var ex = Assert.Throws<ArgumentException>(() => e1.StoreLocal(l));
                Assert.Equal("System.Int32 _local0 is not owned by this Emit, and thus cannot be used", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var l = e1.DeclareLocal<int>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.StoreLocal(l));
                Assert.Equal("StoreLocal expects a value on the stack, but it was empty", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var l = e1.DeclareLocal<int>();
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.StoreLocal(l));
                Assert.Equal("StoreLocal expected an int; found System.Object", ex.Message);
            }
        }

        [Fact]
        public void StoreIndirect()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.StoreIndirect(null));
                Assert.Equal("type", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var ex = Assert.Throws<ArgumentException>(() => e1.StoreIndirect(typeof(int), unaligned: 3));
                Assert.Equal("unaligned must be null, 1, 2, or 4", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.StoreIndirect(typeof(int)));
                Assert.Equal("StoreIndirect expects 2 values on the stack", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.StoreIndirect(typeof(int)));
                Assert.Equal("StoreIndirect expected an int; found System.Object", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.LoadConstant(0);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.StoreIndirect(typeof(int)));
                Assert.Equal("StoreIndirect expected a native int, System.Int32&, System.Int32*, System.UInt32&, or System.UInt32*; found System.Object", ex.Message);
            }

            {
                var e1 = Emit<Action<DateTime>>.NewDynamicMethod();
                e1.LoadConstant(0);
                e1.Convert<IntPtr>();
                e1.LoadArgument(0);

                var ex = Assert.Throws<InvalidOperationException>(() => e1.StoreIndirect(typeof(DateTime)));
                Assert.Equal("StoreIndirect cannot be used with System.DateTime, StoreObject may be more appropriate", ex.Message);
            }
        }

        private class StoreFieldClass
        {
            public static int Static;
            public int Instance;

            public StoreFieldClass()
            {
                Static = Instance = 2;
            }
        }

        [Fact]
        public void StoreField()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.StoreField(null));
                Assert.Equal("field", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(int).GetField("MaxValue");

                var ex = Assert.Throws<ArgumentException>(() => e1.StoreField(f, unaligned: 3));
                Assert.Equal("unaligned must be null, 1, 2, or 4", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(int).GetField("MaxValue");

                var ex = Assert.Throws<ArgumentException>(() => e1.StoreField(f, unaligned: 4));
                Assert.Equal("unaligned cannot be used with static fields", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(StoreFieldClass).GetField("Instance");

                var ex = Assert.Throws<SigilVerificationException>(() => e1.StoreField(f));
                Assert.Equal("StoreField expects 2 values on the stack", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(StoreFieldClass).GetField("Instance");
                e1.NewObject<object>();
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.StoreField(f));
                Assert.Equal("StoreField expected an int; found System.Object", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(StoreFieldClass).GetField("Instance");
                e1.NewObject<object>();
                e1.LoadConstant(0);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.StoreField(f));
                Assert.Equal("StoreField expected a SigilTests.Errors+StoreFieldClass; found System.Object", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(StoreFieldClass).GetField("Static");

                var ex = Assert.Throws<SigilVerificationException>(() => e1.StoreField(f));
                Assert.Equal("StoreField expects a value on the stack, but it was empty", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(StoreFieldClass).GetField("Static");
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.StoreField(f));
                Assert.Equal("StoreField expected an int; found System.Object", ex.Message);
            }
        }

        [Fact]
        public void StoreElement()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.StoreElement<int>());
                Assert.Equal("StoreElement expects 3 values on the stack", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.NewObject<object>();
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.StoreElement<int>());
                Assert.Equal("StoreElement expected an int; found System.Object", ex.Message);
            }

            {
                var e1 = Emit<Action<int[], int, int>>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.NewObject<object>();
                e1.LoadArgument(2);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.StoreElement<int>());
                Assert.Equal("StoreElement expected an int, or native int; found System.Object", ex.Message);
            }

            {
                var e1 = Emit<Action<int[], int, int>>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.LoadArgument(1);
                e1.LoadArgument(2);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.StoreElement<int>());
                Assert.Equal("StoreElement expected a System.Int32[]; found System.Object", ex.Message);
            }
        }

        [Fact]
        public void StoreArgument()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<InvalidOperationException>(() => e1.StoreArgument(0));
                Assert.Equal("Delegate of type System.Action takes no parameters", ex.Message);
            }

            {
                var e1 = Emit<Action<int>>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentException>(() => e1.StoreArgument(4));
                Assert.Equal("index must be between 0 and 0, inclusive", ex.Message);
            }

            {
                var e1 = Emit<Action<int>>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.StoreArgument(0));
                Assert.Equal("StoreArgument expects a value on the stack, but it was empty", ex.Message);
            }

            {
                var e1 = Emit<Action<int>>.NewDynamicMethod();
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.StoreArgument(0));
                Assert.Equal("StoreArgument expected an int; found System.Object", ex.Message);
            }
        }

        [Fact]
        public void SizeOf()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.SizeOf(null));
                Assert.Equal("valueType", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentException>(() => e1.SizeOf(typeof(string)));
                Assert.Equal("valueType must be a ValueType", ex.Message);
            }
        }

        [Fact]
        public void Return()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.LoadConstant(0);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Return());
                Assert.Equal("Return expected the stack of be empty", ex.Message);
            }

            {
                var e1 = Emit<Func<int>>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Return());
                Assert.Equal("Return expects a value on the stack, but it was empty", ex.Message);
            }

            {
                var e1 = Emit<Func<int>>.NewDynamicMethod();
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Return());
                Assert.Equal("Return expected an int; found System.Object", ex.Message);
            }

            {
                var e1 = Emit<Func<int>>.NewDynamicMethod();
                e1.LoadConstant(0);
                e1.LoadConstant(0);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Return());
                Assert.Equal("Return expected the stack of be empty", ex.Message);
            }
        }

        [Fact]
        public void ReThrow()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<InvalidOperationException>(() => e1.ReThrow());
                Assert.Equal("ReThrow is only legal in a catch block", ex.Message);
            }
        }

        [Fact]
        public void Pop()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Pop());
                Assert.Equal("Pop expects a value on the stack, but it was empty", ex.Message);
            }
        }

        private struct NewObjectStruct
        {
            public NewObjectStruct(int i)
            {
            }
        }

        [Fact]
        public void NewObject()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.NewObject((Type)null, null));
                Assert.Equal("type", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.NewObject(typeof(object), null));
                Assert.Equal("parameterTypes", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<InvalidOperationException>(() => e1.NewObject(typeof(object), typeof(int), typeof(string)));
                Assert.Equal("Type System.Object must have a constructor that matches parameters [System.Int32, System.String]", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.NewObject((ConstructorInfo)null));
                Assert.Equal("constructor", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.NewObject<string, char[]>());
                Assert.Equal("NewObject expects a value on the stack, but it was empty", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.NewObject<string, char[]>());
                Assert.Equal("NewObject expected a System.Char[]; found System.Object", ex.Message);
            }
        }

        [Fact]
        public void NewArray()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.NewArray(null));
                Assert.Equal("elementType", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.NewArray<int>());
                Assert.Equal("NewArray expects a value on the stack, but it was empty", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.NewArray<int>());
                Assert.Equal("NewArray expected an int, or native int; found System.Object", ex.Message);
            }
        }

        [Fact]
        public void Locals()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.DeclareLocal(null));
                Assert.Equal("type", ex.ParamName);
            }
        }

        [Fact]
        public void LocalAllocate()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var t = e1.BeginExceptionBlock();
                var c = e1.BeginCatchAllBlock(t);

                var ex = Assert.Throws<InvalidOperationException>(() => e1.LocalAllocate());
                Assert.Equal("LocalAllocate cannot be used in a catch block", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var t = e1.BeginExceptionBlock();
                var c = e1.BeginCatchAllBlock(t);
                e1.Pop();
                e1.EndCatchBlock(c);
                var f = e1.BeginFinallyBlock(t);

                var ex = Assert.Throws<InvalidOperationException>(() => e1.LocalAllocate());
                Assert.Equal("LocalAllocate cannot be used in a finally block", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.LocalAllocate());
                Assert.Equal("LocalAllocate expected the stack to have 1 value", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.LocalAllocate());
                Assert.Equal("LocalAllocate expected an int, or native int; found System.Object", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.LoadConstant(0);
                e1.LoadConstant(0);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.LocalAllocate());
                Assert.Equal("LocalAllocate expected the stack to have 1 value", ex.Message);
            }
        }

        [Fact]
        public void LoadVirtualFunctionPointer()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.LoadVirtualFunctionPointer(null));
                Assert.Equal("method", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(string).GetMethod("Intern");

                var ex = Assert.Throws<ArgumentException>(() => e1.LoadVirtualFunctionPointer(f));
                Assert.Equal("Only non-static methods can be passed to LoadVirtualFunctionPointer, found System.String Intern(System.String)", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(object).GetMethod("ToString");

                var ex = Assert.Throws<SigilVerificationException>(() => e1.LoadVirtualFunctionPointer(f));
                Assert.Equal("LoadVirtualFunctionPointer expects a value on the stack, but it was empty", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(List<int>).GetMethod("Add");

                e1.LoadConstant(0);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.LoadVirtualFunctionPointer(f));
                Assert.Equal($"LoadVirtualFunctionPointer expected a {typeof(List<int>).FullName}; found int", ex.Message);
            }
        }

        [Fact]
        public void LoadObject()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.LoadObject(null));
                Assert.Equal("valueType", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentException>(() => e1.LoadObject(typeof(string)));
                Assert.Equal("valueType must be a ValueType", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentException>(() => e1.LoadObject<DateTime>(unaligned: 3));
                Assert.Equal("unaligned must be null, 1, 2, or 4", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.LoadObject<DateTime>());
                Assert.Equal("LoadObject expects a value on the stack, but it was empty", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.LoadObject<DateTime>());
                Assert.Equal("LoadObject expected a native int, System.DateTime&, or System.DateTime*; found System.Object", ex.Message);
            }
        }

        [Fact]
        public void LoadLocalAddress()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.LoadLocalAddress((Sigil.Local)null));
                Assert.Equal("local", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var e2 = Emit<Action>.NewDynamicMethod();
                var l = e2.DeclareLocal<int>();

                var ex = Assert.Throws<ArgumentException>(() => e1.LoadLocalAddress(l));
                Assert.Equal("System.Int32 _local0 is not owned by this Emit, and thus cannot be used", ex.Message);
            }
        }

        [Fact]
        public void LoadLocal()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.LoadLocal((Sigil.Local)null));
                Assert.Equal("local", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var e2 = Emit<Action>.NewDynamicMethod();
                var l = e2.DeclareLocal<int>();

                var ex = Assert.Throws<ArgumentException>(() => e1.LoadLocal(l));
                Assert.Equal("System.Int32 _local0 is not owned by this Emit, and thus cannot be used", ex.Message);
            }
        }

        [Fact]
        public void LoadLength()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.LoadLength<int>());
                Assert.Equal("LoadLength expects a value on the stack, but it was empty", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.LoadLength<int>());
                Assert.Equal("LoadLength expected a System.Int32[]; found System.Object", ex.Message);
            }
        }

        [Fact]
        public void LoadIndirect()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.LoadIndirect(null));
                Assert.Equal("type", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentException>(() => e1.LoadIndirect<int>(unaligned: 3));
                Assert.Equal("unaligned must be null, 1, 2, or 4", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.LoadIndirect<int>());
                Assert.Equal("LoadIndirect expects a value on the stack, but it was empty", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.LoadIndirect<int>());
                Assert.Equal("LoadIndirect expected a native int, System.Int32&, or System.Int32*; found System.Object", ex.Message);
            }

            {
                var e1 = Emit<Action<DateTime>>.NewDynamicMethod();
                e1.LoadArgumentAddress(0);

                var ex = Assert.Throws<InvalidOperationException>(() => e1.LoadIndirect<DateTime>());
                Assert.Equal("LoadIndirect cannot be used with System.DateTime, LoadObject may be more appropriate", ex.Message);
            }
        }

        [Fact]
        public void LoadFunctionPointer()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.LoadFunctionPointer(null));
                Assert.Equal("method", ex.ParamName);
            }
        }

        [Fact]
        public void LoadFieldAddress()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.LoadFieldAddress(null));
                Assert.Equal("field", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(LoadFieldClass).GetField("A");

                var ex = Assert.Throws<SigilVerificationException>(() => e1.LoadFieldAddress(f));
                Assert.Equal("LoadFieldAddress expects a value on the stack, but it was empty", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(LoadFieldClass).GetField("A");

                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.LoadFieldAddress(f));
                Assert.Equal("LoadFieldAddress expected a SigilTests.Errors+LoadFieldClass; found System.Object", ex.Message);
            }
        }

        private class LoadFieldClass
        {
            public int A;

            public LoadFieldClass()
            {
                A = 123;
            }
        }

        [Fact]
        public void LoadField()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.LoadField(null));
                Assert.Equal("field", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(int).GetField("MaxValue");

                var ex = Assert.Throws<ArgumentException>(() => e1.LoadField(f, unaligned: 1));
                Assert.Equal("unaligned cannot be used with static fields", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(int).GetField("MaxValue");

                var ex = Assert.Throws<ArgumentException>(() => e1.LoadField(f, unaligned: 3));
                Assert.Equal("unaligned must be null, 1, 2, or 4", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(LoadFieldClass).GetField("A");

                var ex = Assert.Throws<SigilVerificationException>(() => e1.LoadField(f));
                Assert.Equal("LoadField expects a value on the stack, but it was empty", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(LoadFieldClass).GetField("A");

                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.LoadField(f));
                Assert.Equal("LoadField expected a SigilTests.Errors+LoadFieldClass; found System.Object", ex.Message);
            }
        }

        [Fact]
        public void LoadElementAddress()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.LoadElementAddress<int>());
                Assert.Equal("LoadElementAddress expects 2 values on the stack", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.LoadElementAddress<int>());
                Assert.Equal("LoadElementAddress expected an int, or native int; found System.Object", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.LoadConstant(0);
                e1.LoadConstant(0);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.LoadElementAddress<int>());
                Assert.Equal("LoadElementAddress expected a System.Int32[]; found int", ex.Message);
            }

            {
                var e1 = Emit<Action<int[,]>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadConstant(0);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.LoadElementAddress<int>());
                Assert.Equal("LoadElementAddress expected a System.Int32[]; found System.Int32[,]", ex.Message);
            }
        }

        [Fact]
        public void LoadElement()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.LoadElement<int>());
                Assert.Equal("LoadElement expects 2 values on the stack", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.LoadElement<int>());
                Assert.Equal("LoadElement expected an int, or native int; found System.Object", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.LoadConstant(0);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.LoadElement<int>());
                Assert.Equal("LoadElement expected a System.Int32[]; found System.Object", ex.Message);
            }

            {
                var e1 = Emit<Action<int[,], int>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.LoadElement<int>());
                Assert.Equal("LoadElement expected a System.Int32[]; found System.Int32[,]", ex.Message);
            }
        }

        [Fact]
        public void LoadConstant()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.LoadConstant((Type)null));
                Assert.Equal("type", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.LoadConstant((MethodInfo)null));
                Assert.Equal("method", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.LoadConstant((FieldInfo)null));
                Assert.Equal("field", ex.ParamName);
            }
        }

        [Fact]
        public void LoadArgumentAddress()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentException>(() => e1.LoadArgumentAddress(0));
                Assert.Equal("Delegate of type System.Action takes no parameters", ex.Message);
            }

            {
                var e1 = Emit<Action<int>>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentException>(() => e1.LoadArgumentAddress(4));
                Assert.Equal("index must be between 0 and 0, inclusive", ex.Message);
            }
        }

        [Fact]
        public void LoadArgument()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentException>(() => e1.LoadArgument(0));
                Assert.Equal("Delegate of type System.Action takes no parameters", ex.Message);
            }

            {
                var e1 = Emit<Action<int>>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentException>(() => e1.LoadArgument(4));
                Assert.Equal("index must be between 0 and 0, inclusive", ex.Message);
            }
        }

        [Fact]
        public void Leave()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.Leave((Sigil.Label)null));
                Assert.Equal("label", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var e2 = Emit<Action>.NewDynamicMethod();
                var l = e2.DefineLabel();

                var ex = Assert.Throws<ArgumentException>(() => e1.Leave(l));
                Assert.Equal("_label0 is not owned by this Emit, and thus cannot be used", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var l = e1.DefineLabel();

                var ex = Assert.Throws<InvalidOperationException>(() => e1.Leave(l));
                Assert.Equal("Leave can only be used within an exception or catch block", ex.Message);
            }
        }

        [Fact]
        public void Labels()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var l = e1.DefineLabel();
                var d = e1.DefineLabel();
                e1.Branch(l);
                e1.MarkLabel(d);
                e1.Return();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.CreateDelegate());
                Assert.Equal("Usage of unmarked label _label0", ex.Message);
            }
        }

        [Fact]
        public void MarkLabel()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.MarkLabel((Label)null));
                Assert.Equal("label", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var e2 = Emit<Action>.NewDynamicMethod();
                var l = e2.DefineLabel();

                var ex = Assert.Throws<ArgumentException>(() => e1.MarkLabel(l));
                Assert.Equal("_label0 is not owned by this Emit, and thus cannot be used", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var l = e1.DefineLabel();
                e1.MarkLabel(l);

                var ex = Assert.Throws<InvalidOperationException>(() => e1.MarkLabel(l));
                Assert.Equal("label [_label0] has already been marked, and cannot be marked a second time", ex.Message);
            }
        }

        [Fact]
        public void Jump()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.Jump(null));
                Assert.Equal("method", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var toString = typeof(object).GetMethod("ToString");

                var ex = Assert.Throws<ArgumentException>(() => e1.Jump(toString));
                Assert.Equal("Jump expected a calling convention of Standard, found Standard, HasThis", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var intern = typeof(string).GetMethod("Intern");

                var ex = Assert.Throws<ArgumentException>(() => e1.Jump(intern));
                Assert.Equal("Jump expected a method with 0 parameters, found 1", ex.Message);
            }

            {
                var e1 = Emit<Action<string>>.NewDynamicMethod();
                var intern = typeof(string).GetMethod("Intern");

                e1.LoadArgument(0);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Jump(intern));
                Assert.Equal("Jump expected the stack of be empty", ex.Message);
            }

            {
                var e1 = Emit<Action<int>>.NewDynamicMethod();
                var intern = typeof(string).GetMethod("Intern");

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Jump(intern));
                Assert.Equal("Jump expected the #0 parameter to be assignable from System.Int32, but found System.String", ex.Message);
            }

            {
                var e1 = Emit<Action<string>>.NewDynamicMethod();
                var intern = typeof(string).GetMethod("Intern");

                e1.BeginExceptionBlock();

                var ex = Assert.Throws<InvalidOperationException>(() => e1.Jump(intern));
                Assert.Equal("Jump cannot transfer control from an exception block", ex.Message);
            }

            {
                var e1 = Emit<Action<string>>.NewDynamicMethod();
                var intern = typeof(string).GetMethod("Intern");

                var t = e1.BeginExceptionBlock();
                e1.BeginCatchAllBlock(t);

                var ex = Assert.Throws<InvalidOperationException>(() => e1.Jump(intern));
                Assert.Equal("Jump cannot transfer control from a catch block", ex.Message);
            }

            {
                var e1 = Emit<Action<string>>.NewDynamicMethod();
                var intern = typeof(string).GetMethod("Intern");

                var t = e1.BeginExceptionBlock();
                var c = e1.BeginCatchAllBlock(t);
                e1.Pop();
                e1.EndCatchBlock(c);
                e1.BeginFinallyBlock(t);

                var ex = Assert.Throws<InvalidOperationException>(() => e1.Jump(intern));
                Assert.Equal("Jump cannot transfer control from a finally block", ex.Message);
            }
        }

        [Fact]
        public void IsInstance()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.IsInstance(null));
                Assert.Equal("type", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.IsInstance<string>());
                Assert.Equal("IsInstance expects a value on the stack, but it was empty", ex.Message);
            }
        }

        [Fact]
        public void InitializeObject()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.InitializeObject(null));
                Assert.Equal("valueType", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.InitializeObject<int>());
                Assert.Equal("InitializeObject expects a value on the stack, but it was empty", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.InitializeObject<DateTime>());
                Assert.Equal("InitializeObject expected a native int, System.DateTime&, or System.DateTime*; found System.Object", ex.Message);
            }
        }

        [Fact]
        public void InitializeBlock()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentException>(() => e1.InitializeBlock(unaligned: 3));
                Assert.Equal("unaligned must be null, 1, 2, or 4", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.InitializeBlock());
                Assert.Equal("InitializeBlock expects 3 values on the stack", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.NewObject<object>();
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.InitializeBlock());
                Assert.Equal("InitializeBlock expected an int; found System.Object", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.NewObject<object>();
                e1.LoadConstant(0);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.InitializeBlock());
                Assert.Equal("InitializeBlock expected an int, or native int; found System.Object", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.LoadConstant(0);
                e1.Convert<IntPtr>();
                e1.LoadConstant(0);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.InitializeBlock());
                Assert.Equal("InitializeBlock expected a native int, System.Byte&, or System.Byte*; found System.Object", ex.Message);
            }
        }

        [Fact]
        public void Duplicate()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Duplicate());
                Assert.Equal("Duplicate expects a value on the stack, but it was empty", ex.Message);
            }
        }

        [Fact]
        public void CopyObject()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.CopyObject(null));
                Assert.Equal("valueType", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentException>(() => e1.CopyObject(typeof(string)));
                Assert.Equal("CopyObject expects a ValueType; found System.String", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.CopyObject<int>());
                Assert.Equal("CopyObject expects 2 values on the stack", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.CopyObject<int>());
                Assert.Equal("CopyObject expected a native int, System.Int32&, or System.Int32*; found System.Object", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.LoadConstant(0);
                e1.Convert<IntPtr>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.CopyObject<int>());
                Assert.Equal("CopyObject expected a native int, System.Int32&, or System.Int32*; found System.Object", ex.Message);
            }
        }

        [Fact]
        public void CopyBlock()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentException>(() => e1.CopyBlock(unaligned: 3));
                Assert.StartsWith("unaligned must be null, 1, 2, or 4", ex.Message);
                Assert.Equal("unaligned", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.CopyBlock());
                Assert.Equal("CopyBlock expects 3 values on the stack", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.NewObject<object>();
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.CopyBlock());
                Assert.Equal("CopyBlock expected an int; found System.Object", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.NewObject<object>();
                e1.LoadConstant(0);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.CopyBlock());
                Assert.Equal("CopyBlock expected a native int, System.Byte&, or System.Byte*; found System.Object", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.LoadConstant(0);
                e1.Convert<IntPtr>();
                e1.LoadConstant(0);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.CopyBlock());
                Assert.Equal("CopyBlock expected a native int, System.Byte&, or System.Byte*; found System.Object", ex.Message);
            }
        }

        [Fact]
        public void ConvertIllegal()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<InvalidOperationException>(() => e1.ConvertOverflow<float>());
                Assert.Equal("There is no operation for converting to a float with overflow checking", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<InvalidOperationException>(() => e1.ConvertOverflow<double>());
                Assert.Equal("There is no operation for converting to a double with overflow checking", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<InvalidOperationException>(() => e1.UnsignedConvertOverflow<float>());
                Assert.Equal("There is no operation for converting to a float with overflow checking", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<InvalidOperationException>(() => e1.UnsignedConvertOverflow<double>());
                Assert.Equal("There is no operation for converting to a double with overflow checking", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Convert<UIntPtr>());
                Assert.Equal("Convert expected a by ref, double, float, int, long, native int, or pointer; found System.Object", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.ConvertOverflow<int>());
                Assert.Equal("ConvertOverflow expected a by ref, double, float, int, long, native int, or pointer; found System.Object", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.UnsignedConvertOverflow<int>());
                Assert.Equal("UnsignedConvertOverflow expected a by ref, double, float, int, long, native int, or pointer; found System.Object", ex.Message);
            }
        }

        [Fact]
        public void ConvertEmptyStack()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Convert<int>());
                Assert.Equal("Convert expects a value on the stack, but it was empty", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.ConvertOverflow<int>());
                Assert.Equal("ConvertOverflow expects a value on the stack, but it was empty", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.UnsignedConvertOverflow<int>());
                Assert.Equal("UnsignedConvertOverflow expects a value on the stack, but it was empty", ex.Message);
            }
        }

        [Fact]
        public void ConvertNonPrimitives()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentException>(() => e1.Convert(typeof(object)));
                Assert.Equal("Convert expects a non-character primitive type", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentException>(() => e1.ConvertOverflow<object>());
                Assert.Equal("ConvertOverflow expects a non-character primitive type", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentException>(() => e1.UnsignedConvertOverflow<object>());
                Assert.Equal("UnsignedConvertOverflow expects a non-character primitive type", ex.Message);
            }
        }

        [Fact]
        public void ConvertNulls()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.Convert(null));
                Assert.Equal("primitiveType", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.ConvertOverflow(null));
                Assert.Equal("primitiveType", ex.ParamName);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<ArgumentNullException>(() => e1.UnsignedConvertOverflow(null));
                Assert.Equal("primitiveType", ex.ParamName);
            }
        }

        [Fact]
        public void ChecksStacks()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.CompareEqual());
                Assert.Equal("CompareEqual expects 2 values on the stack", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.CompareGreaterThan());
                Assert.Equal("CompareGreaterThan expects 2 values on the stack", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.CompareLessThan());
                Assert.Equal("CompareLessThan expects 2 values on the stack", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.UnsignedCompareGreaterThan());
                Assert.Equal("UnsignedCompareGreaterThan expects 2 values on the stack", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                var ex = Assert.Throws<SigilVerificationException>(() => e1.UnsignedCompareLessThan());
                Assert.Equal("UnsignedCompareLessThan expects 2 values on the stack", ex.Message);
            }
        }

        [Fact]
        public void CheckFiniteStack()
        {
            var e1 = Emit<Action>.NewDynamicMethod();

            var ex = Assert.Throws<SigilVerificationException>(() => e1.CheckFinite());
            Assert.Equal("CheckFinite expects a value on the stack, but it was empty", ex.Message);

            var e2 = Emit<Action>.NewDynamicMethod();
            e2.NewObject<object>();

            var ex2 = Assert.Throws<SigilVerificationException>(() => e2.CheckFinite());
            Assert.Equal("CheckFinite expected a double, or float; found System.Object", ex2.Message);
        }

        [Fact]
        public void CastClassNull()
        {
            var e1 = Emit<Action>.NewDynamicMethod();

            var ex = Assert.Throws<ArgumentNullException>(() => e1.CastClass(null));
            Assert.Equal("referenceType", ex.ParamName);
        }

        [Fact]
        public void CastClassValueType()
        {
            var e1 = Emit<Action>.NewDynamicMethod();

            var ex = Assert.Throws<ArgumentException>(() => e1.CastClass(typeof(int)));
            Assert.Equal("Can only cast to ReferenceTypes, found System.Int32", ex.Message);
        }

        [Fact]
        public void CastClassEmptyStack()
        {
            var e1 = Emit<Action>.NewDynamicMethod();

            var ex = Assert.Throws<SigilVerificationException>(() => e1.CastClass<string>());
            Assert.Equal("CastClass expects a value on the stack, but it was empty", ex.Message);
        }

        [Fact]
        public void CallIndirectNull()
        {
            var e1 = Emit<Action>.NewDynamicMethod();

            var ex = Assert.Throws<ArgumentNullException>(() => e1.CallIndirect(CallingConventions.Any, typeof(void), null));
            Assert.Equal("parameterTypes", ex.ParamName);
        }

        [Fact]
        public void CallIndirectBadConvention()
        {
            var e1 = Emit<Action>.NewDynamicMethod();

            var ex = Assert.Throws<ArgumentException>(() => e1.CallIndirect((CallingConventions)254));
            Assert.Equal("callConventions", ex.ParamName);
        }

        [Fact]
        public void CallIndirectEmptyStack()
        {
            var e1 = Emit<Action>.NewDynamicMethod();

            var ex = Assert.Throws<SigilVerificationException>(() => e1.CallIndirect(CallingConventions.Any));
            Assert.Equal("CallIndirect expects a value on the stack, but it was empty", ex.Message);
        }

        [Fact]
        public void CallIndirectNoPtr()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            e1.NewObject<object>();

            var ex = Assert.Throws<SigilVerificationException>(() => e1.CallIndirect(CallingConventions.Any));
            Assert.Equal("CallIndirect expected a native int; found System.Object", ex.Message);
        }

        [Fact]
        public void CallIndirectKnownBad()
        {
            var toString = typeof(object).GetMethod("ToString");
            var addInt = typeof(List<int>).GetMethod("Add");

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.LoadFunctionPointer(toString);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.CallIndirect(CallingConventions.VarArgs, typeof(string), Type.EmptyTypes, Type.EmptyTypes));
                Assert.Equal("CallIndirect expects method calling conventions to match, found Standard, HasThis on the stack", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.LoadFunctionPointer(addInt);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.CallIndirect(addInt.CallingConvention));
                Assert.Equal("CallIndirect expects a 'this' value assignable to System.Collections.Generic.List`1[System.Int32], found System.Object", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.LoadFunctionPointer(toString);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.CallIndirect(toString.CallingConvention, typeof(object)));
                Assert.Equal("CallIndirect expects method return types to match, found System.String on the stack", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<List<int>>();
                e1.LoadConstant(2.0);
                e1.LoadFunctionPointer(addInt);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.CallIndirect(addInt.CallingConvention, typeof(void), typeof(int)));
                Assert.Equal("CallIndirect expected an int; found double", ex.Message);
            }
        }

        [Fact]
        public void CallBadParam()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            e1.NewObject<object>();

            var ex = Assert.Throws<SigilVerificationException>(() => e1.Call(typeof(Errors).GetMethod("CallBadParam")));
            Assert.Equal("Call expected a SigilTests.Errors; found System.Object", ex.Message);
        }

        [Fact]
        public void CallVirtualBadParam()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            e1.NewObject<object>();

            var ex = Assert.Throws<SigilVerificationException>(() => e1.CallVirtual(typeof(Errors).GetMethod("CallVirtualBadParam")));
            Assert.Equal("CallVirtual expected a SigilTests.Errors; found System.Object", ex.Message);
        }

        [Fact]
        public void NullCallMethod()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            var ex = Assert.Throws<ArgumentNullException>(() => e1.Call((MethodInfo)null));
            Assert.Equal("method", ex.ParamName);
        }

        [Fact]
        public void NullCallConstructor()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            var ex = Assert.Throws<ArgumentNullException>(() => e1.Call((ConstructorInfo)null));
            Assert.Equal("cons", ex.ParamName);
        }

        [Fact]
        public void CallEmptyStack()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            var ex = Assert.Throws<SigilVerificationException>(() => e1.Call(typeof(object).GetMethod("ToString")));
            Assert.Equal("Call expects a value on the stack, but it was empty", ex.Message);
        }

        [Fact]
        public void NullCallVirtualMethod()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            var ex = Assert.Throws<ArgumentNullException>(() => e1.CallVirtual(null));
            Assert.Equal("method", ex.ParamName);
        }

        [Fact]
        public void CallVirtualEmptyStack()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            var ex = Assert.Throws<SigilVerificationException>(() => e1.CallVirtual(typeof(object).GetMethod("ToString")));
            Assert.Equal("CallVirtual expects a value on the stack, but it was empty", ex.Message);
        }

        [Fact]
        public void CallVirtualStatic()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            var ex = Assert.Throws<ArgumentException>(() => e1.CallVirtual(typeof(string).GetMethod("Intern")));
            Assert.Equal("Only non-static methods can be called using CallVirtual, found System.String Intern(System.String)", ex.Message);
        }

        [Fact]
        public void NullBranchLabels()
        {
            var emit = typeof(Emit<Action>);
            var branches =
                new[]
                {
                    emit.GetMethod("Branch", new [] { typeof(Label) }),
                    emit.GetMethod("BranchIfEqual", new [] { typeof(Label) }),
                    emit.GetMethod("BranchIfFalse", new [] { typeof(Label) }),
                    emit.GetMethod("BranchIfGreater", new [] { typeof(Label) }),
                    emit.GetMethod("BranchIfGreaterOrEqual", new [] { typeof(Label) }),
                    emit.GetMethod("BranchIfLess", new [] { typeof(Label) }),
                    emit.GetMethod("BranchIfLessOrEqual", new [] { typeof(Label) }),
                    emit.GetMethod("BranchIfTrue", new [] { typeof(Label) }),
                    emit.GetMethod("UnsignedBranchIfNotEqual", new [] { typeof(Label) }),
                    emit.GetMethod("UnsignedBranchIfGreater", new [] { typeof(Label) }),
                    emit.GetMethod("UnsignedBranchIfGreaterOrEqual", new [] { typeof(Label) }),
                    emit.GetMethod("UnsignedBranchIfLess", new [] { typeof(Label) }),
                    emit.GetMethod("UnsignedBranchIfLessOrEqual", new [] { typeof(Label) })
                };

            foreach (var branch in branches)
            {
                var e1 = Emit<Action>.NewDynamicMethod("E1");

                var ex = Assert.Throws<TargetInvocationException>(() => branch.Invoke(e1, new object[] { null }));
                var f = (ArgumentNullException)ex.InnerException;
                Assert.Equal("label", f.ParamName);
            }
        }

        [Fact]
        public void UnownedBranchLabels()
        {
            var emit = typeof(Emit<Action>);
            var branches =
                new[]
                {
                    emit.GetMethod("Branch", new [] { typeof(Label) }),
                    emit.GetMethod("BranchIfEqual", new [] { typeof(Label) }),
                    emit.GetMethod("BranchIfFalse", new [] { typeof(Label) }),
                    emit.GetMethod("BranchIfGreater", new [] { typeof(Label) }),
                    emit.GetMethod("BranchIfGreaterOrEqual", new [] { typeof(Label) }),
                    emit.GetMethod("BranchIfLess", new [] { typeof(Label) }),
                    emit.GetMethod("BranchIfLessOrEqual", new [] { typeof(Label) }),
                    emit.GetMethod("BranchIfTrue", new [] { typeof(Label) }),
                    emit.GetMethod("UnsignedBranchIfNotEqual", new [] { typeof(Label) }),
                    emit.GetMethod("UnsignedBranchIfGreater", new [] { typeof(Label) }),
                    emit.GetMethod("UnsignedBranchIfGreaterOrEqual", new [] { typeof(Label) }),
                    emit.GetMethod("UnsignedBranchIfLess", new [] { typeof(Label) }),
                    emit.GetMethod("UnsignedBranchIfLessOrEqual", new [] { typeof(Label) })
                };

            foreach (var branch in branches)
            {
                var e1 = Emit<Action>.NewDynamicMethod("E1");
                var e2 = Emit<Action>.NewDynamicMethod("E2");
                var l = e2.DefineLabel("wrong_emit");

                var ex = Assert.Throws<TargetInvocationException>(() => branch.Invoke(e1, new object[] { l }));
                var f = (ArgumentException)ex.InnerException;
                Assert.Equal("wrong_emit is not owned by this Emit, and thus cannot be used", f.Message);
            }
        }

        [Fact]
        public void BranchEmptyStack()
        {
            var emit = typeof(Emit<Action>);
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
                var e1 = Emit<Action>.NewDynamicMethod("E1");
                var l = e1.DefineLabel();

                var ex = Assert.Throws<TargetInvocationException>(() => branch.Invoke(e1, new object[] { l }));
                var f = (SigilVerificationException)ex.InnerException;
                Assert.EndsWith(" expects 2 values on the stack", f.Message);
            }

            {
                var e2 = Emit<Action>.NewDynamicMethod("E2");
                var l = e2.DefineLabel();

                var ex = Assert.Throws<SigilVerificationException>(() => e2.BranchIfFalse(l));
                Assert.Equal("BranchIfFalse expects a value on the stack, but it was empty", ex.Message);
            }

            {
                var e3 = Emit<Action>.NewDynamicMethod("E3");
                var l = e3.DefineLabel();

                var ex = Assert.Throws<SigilVerificationException>(() => e3.BranchIfTrue(l));
                Assert.Equal("BranchIfTrue expects a value on the stack, but it was empty", ex.Message);
            }
        }

        [Fact]
        public void CatchInCatch()
        {
            var e1 = Emit<Action>.NewDynamicMethod("e1");
            var t = e1.BeginExceptionBlock();
            // Maybe for netcoreapp
            // var c1 = e1.BeginCatchBlock<ArgumentException>(t);
            var c1 = e1.BeginCatchBlock<StackOverflowException>(t);

            var ex = Assert.Throws<InvalidOperationException>(() => e1.BeginCatchBlock<Exception>(t));
            Assert.StartsWith("Cannot start a new catch block, ", ex.Message);
        }

        [Fact]
        public void NullTryCatch()
        {
            var e1 = Emit<Action>.NewDynamicMethod("e1");

            var ex = Assert.Throws<ArgumentNullException>(() => e1.BeginCatchAllBlock(null));
            Assert.Equal("forTry", ex.ParamName);
        }

        [Fact]
        public void CatchNonException()
        {
            var e1 = Emit<Action>.NewDynamicMethod("e1");
            var t = e1.BeginExceptionBlock();

            var ex = Assert.Throws<ArgumentException>(() => e1.BeginCatchBlock<string>(t));
            Assert.Equal("exceptionType", ex.ParamName);
        }

        [Fact]
        public void NonEmptyExceptBlock()
        {
            var e1 = Emit<Action>.NewDynamicMethod("e1");
            var t = e1.BeginExceptionBlock();
            e1.LoadConstant("foo");

            var ex = Assert.Throws<SigilVerificationException>(() => e1.BeginCatchAllBlock(t));
            Assert.Equal("BeginCatchBlock expected the stack of be empty", ex.Message);
        }

        [Fact]
        public void CatchAlreadyClosedTry()
        {
            var e1 = Emit<Action>.NewDynamicMethod("e1");
            var t = e1.BeginExceptionBlock();
            var c1 = e1.BeginCatchAllBlock(t);
            e1.Pop();
            e1.EndCatchBlock(c1);
            e1.EndExceptionBlock(t);

            var ex = Assert.Throws<SigilVerificationException>(() => e1.BeginCatchAllBlock(t));
            Assert.StartsWith("BeginCatchBlock expects an unclosed exception block, ", ex.Message);
        }

        [Fact]
        public void CatchExceptionNull()
        {
            var e1 = Emit<Action>.NewDynamicMethod("e1");
            var t = e1.BeginExceptionBlock();

            var ex = Assert.Throws<ArgumentNullException>(() => e1.BeginCatchBlock(t, null));
            Assert.Equal("exceptionType", ex.ParamName);
        }

        [Fact]
        public void CatchOtherTry()
        {
            var e1 = Emit<Action>.NewDynamicMethod("e1");
            var t1 = e1.BeginExceptionBlock();
            var t2 = e1.BeginExceptionBlock();

            var ex = Assert.Throws<InvalidOperationException>(() => e1.BeginCatchAllBlock(t1));
            Assert.StartsWith("Cannot start CatchBlock on ", ex.Message);
        }

        [Fact]
        public void MixedOwners()
        {
            var e1 = Emit<Action>.NewDynamicMethod("e1");
            var t1 = e1.BeginExceptionBlock();

            var e2 = Emit<Action>.NewDynamicMethod("e2");
            var t2 = e2.BeginExceptionBlock();

            var ex = Assert.Throws<ArgumentException>(() => e1.BeginCatchAllBlock(t2));
            Assert.EndsWith(" is not owned by this Emit, and thus cannot be used", ex.Message);
        }

        [Fact]
        public void NonEmptyTry()
        {
            var e1 = Emit<Action>.NewDynamicMethod("e1");
            e1.LoadConstant(123);

            var ex = Assert.Throws<SigilVerificationException>(() => e1.BeginExceptionBlock());
            Assert.Equal("BeginExceptionBlock expected the stack of be empty", ex.Message);
        }

        [Fact]
        public void ShiftEmptyStack()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            var ex = Assert.Throws<SigilVerificationException>(() => e1.ShiftLeft());
            Assert.Equal("ShiftLeft expects 2 values on the stack", ex.Message);
        }

        [Fact]
        public void ShiftBadValues()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");
            e1.LoadConstant("123");
            e1.LoadConstant(4);

            var ex = Assert.Throws<SigilVerificationException>(() => e1.ShiftLeft());
            Assert.Equal("ShiftLeft expected an int, long, or native int; found System.String", ex.Message);

            var e2 = Emit<Action>.NewDynamicMethod("E2");
            e2.LoadConstant(123);
            e2.LoadConstant("4");

            var ex2 = Assert.Throws<SigilVerificationException>(() => e2.ShiftLeft());
            Assert.Equal("ShiftLeft expected an int, or native int; found System.String", ex2.Message);
        }

        [Fact]
        public void Add()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod("E1");
                e1.LoadConstant("123");
                e1.LoadConstant(4);

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Add());
                Assert.Equal("Add expected an int, or native int; found System.String", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod("E2");
                e1.LoadConstant(123);
                e1.LoadConstant("4");

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Add());
                Assert.Equal("Add expected a by ref, double, float, int, long, native int, or pointer; found System.String", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod("E3");
                e1.LoadConstant(123L);
                e1.LoadConstant("4");

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Add());
                Assert.Equal("Add expected a by ref, double, float, int, long, native int, or pointer; found System.String", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod("E4");
                e1.LoadConstant(123);
                e1.Convert<IntPtr>();
                e1.LoadConstant("4");

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Add());
                Assert.Equal("Add expected a by ref, double, float, int, long, native int, or pointer; found System.String", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod("E5");
                e1.LoadConstant(123f);
                e1.LoadConstant("4");

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Add());
                Assert.Equal("Add expected a by ref, double, float, int, long, native int, or pointer; found System.String", ex.Message);
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod("E6");
                e1.LoadConstant(123.0);
                e1.LoadConstant("4");

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Add());
                Assert.Equal("Add expected a by ref, double, float, int, long, native int, or pointer; found System.String", ex.Message);
            }

            {
                var e1 = Emit<Action<int>>.NewDynamicMethod("E7");
                e1.LoadArgumentAddress(0);
                e1.LoadConstant("4");

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Add());
                Assert.Equal("Add expected a by ref, double, float, int, long, native int, or pointer; found System.String", ex.Message);
            }

            {
                var e1 = Emit<Action<int>>.NewDynamicMethod("E8");

                var ex = Assert.Throws<SigilVerificationException>(() => e1.Add());
                Assert.Equal("Add expects 2 values on the stack", ex.Message);
            }
        }

        [Fact]
        public void Multiply()
        {
            var e3 = Emit<Action<int>>.NewDynamicMethod("E3");

            var ex = Assert.Throws<SigilVerificationException>(() => e3.Multiply());
            Assert.Equal("Multiply expects 2 values on the stack", ex.Message);
        }

        [Fact]
        public void AddOverflow()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            var ex = Assert.Throws<SigilVerificationException>(() => e1.AddOverflow());
            Assert.Equal("AddOverflow expects 2 values on the stack", ex.Message);
        }

        [Fact]
        public void UnsignedAddOverflow()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            var ex = Assert.Throws<SigilVerificationException>(() => e1.UnsignedAddOverflow());
            Assert.Equal("UnsignedAddOverflow expects 2 values on the stack", ex.Message);
        }

        [Fact]
        public void MultiplyOverflow()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            var ex = Assert.Throws<SigilVerificationException>(() => e1.MultiplyOverflow());
            Assert.Equal("MultiplyOverflow expects 2 values on the stack", ex.Message);
        }

        [Fact]
        public void UnsignedMultiplyOverflow()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            var ex = Assert.Throws<SigilVerificationException>(() => e1.UnsignedMultiplyOverflow());
            Assert.Equal("UnsignedMultiplyOverflow expects 2 values on the stack", ex.Message);
        }

        [Fact]
        public void Divide()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            var ex = Assert.Throws<SigilVerificationException>(() => e1.Divide());
            Assert.Equal("Divide expects 2 values on the stack", ex.Message);
        }

        [Fact]
        public void UnsignedDivide()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            var ex = Assert.Throws<SigilVerificationException>(() => e1.UnsignedDivide());
            Assert.Equal("UnsignedDivide expects 2 values on the stack", ex.Message);
        }

        [Fact]
        public void Remainder()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            var ex = Assert.Throws<SigilVerificationException>(() => e1.Remainder());
            Assert.Equal("Remainder expects 2 values on the stack", ex.Message);
        }

        [Fact]
        public void UnsignedRemainder()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            var ex = Assert.Throws<SigilVerificationException>(() => e1.UnsignedRemainder());
            Assert.Equal("UnsignedRemainder expects 2 values on the stack", ex.Message);
        }

        [Fact]
        public void Subtract()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            var ex = Assert.Throws<SigilVerificationException>(() => e1.Subtract());
            Assert.Equal("Subtract expects 2 values on the stack", ex.Message);
        }

        [Fact]
        public void SubtractOverflow()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            var ex = Assert.Throws<SigilVerificationException>(() => e1.SubtractOverflow());
            Assert.Equal("SubtractOverflow expects 2 values on the stack", ex.Message);
        }

        [Fact]
        public void UnsignedSubtractOverflow()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            var ex = Assert.Throws<SigilVerificationException>(() => e1.UnsignedSubtractOverflow());
            Assert.Equal("UnsignedSubtractOverflow expects 2 values on the stack", ex.Message);
        }

        [Fact]
        public void Negate()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            var ex = Assert.Throws<SigilVerificationException>(() => e1.Negate());
            Assert.Equal("Negate expects a value on the stack, but it was empty", ex.Message);

            var e2 = Emit<Action>.NewDynamicMethod("E2");
            e2.LoadConstant("Hello");

            var ex2 = Assert.Throws<SigilVerificationException>(() => e2.Negate());
            Assert.Equal("Negate expected a double, float, int, long, or native int; found System.String", ex2.Message);
        }

        [Fact]
        public void BranchOutOfTry()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");
            var l = e1.DefineLabel("end");

            var t = e1.BeginExceptionBlock();
            e1.Branch(l);
            var c = e1.BeginCatchAllBlock(t);
            e1.Pop();
            e1.EndCatchBlock(c);
            e1.EndExceptionBlock(t);

            e1.MarkLabel(l);
            e1.Return();

            var ex = Assert.Throws<SigilVerificationException>(() => e1.CreateDelegate());
            Assert.Equal("Cannot branch from inside Sigil.ExceptionBlock to outside, exit the ExceptionBlock first", ex.Message);
        }

        [Fact]
        public void BranchOutOfCatch()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");
            var l = e1.DefineLabel("end");

            var t = e1.BeginExceptionBlock();
            var c = e1.BeginCatchAllBlock(t);
            e1.Pop();
            e1.Branch(l);
            e1.EndCatchBlock(c);
            e1.EndExceptionBlock(t);

            e1.MarkLabel(l);
            e1.Return();

            var ex = Assert.Throws<SigilVerificationException>(() => e1.CreateDelegate());
            Assert.Equal("Cannot branch from inside Sigil.CatchBlock to outside, exit the ExceptionBlock first", ex.Message);
        }

        [Fact]
        public void BranchOutOfFinally()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");
            var l = e1.DefineLabel("end");
            var dead = e1.DefineLabel();

            var t = e1.BeginExceptionBlock();
            var f = e1.BeginFinallyBlock(t);
            e1.Branch(l);
            e1.MarkLabel(dead);
            e1.EndFinallyBlock(f);
            e1.EndExceptionBlock(t);

            e1.MarkLabel(l);
            e1.Return();

            var ex = Assert.Throws<SigilVerificationException>(() => e1.CreateDelegate());
            Assert.Equal("Cannot branch from inside Sigil.FinallyBlock to outside, exit the ExceptionBlock first", ex.Message);
        }

        [Fact]
        public void BranchIntoFinally()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");
            var l = e1.DefineLabel("inFinally");
            var dead = e1.DefineLabel();

            e1.Branch(l);
            e1.MarkLabel(dead);

            var t = e1.BeginExceptionBlock();
            var f = e1.BeginFinallyBlock(t);
            e1.MarkLabel(l);
            e1.EndFinallyBlock(f);
            e1.EndExceptionBlock(t);

            e1.Return();

            var ex = Assert.Throws<SigilVerificationException>(() => e1.CreateDelegate());
            Assert.Equal("Cannot branch into a FinallyBlock", ex.Message);
        }

        [Fact]
        public void BeginFinallyBlock()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            var ex = Assert.Throws<ArgumentNullException>(() => e1.BeginFinallyBlock(null));
            Assert.Equal("forTry", ex.ParamName);

            var e2 = Emit<Action>.NewDynamicMethod("E2");
            var e3 = Emit<Action>.NewDynamicMethod("E3");
            var t2 = e2.BeginExceptionBlock();

            var ex3 = Assert.Throws<ArgumentException>(() => e3.BeginFinallyBlock(t2));
            Assert.Equal("Sigil.ExceptionBlock is not owned by this Emit, and thus cannot be used", ex3.Message);

            var e4 = Emit<Action>.NewDynamicMethod("E4");
            var t4 = e4.BeginExceptionBlock();
            var c4 = e4.BeginCatchAllBlock(t4);
            e4.Pop();
            e4.EndCatchBlock(c4);
            e4.EndExceptionBlock(t4);

            var ex4 = Assert.Throws<InvalidOperationException>(() => e4.BeginFinallyBlock(t4));
            Assert.Equal("BeginFinallyBlock expects an unclosed exception block, but Sigil.ExceptionBlock is already closed", ex4.Message);

            var e5 = Emit<Action>.NewDynamicMethod("E5");
            var t5 = e5.BeginExceptionBlock();
            var t55 = e5.BeginExceptionBlock();

            var ex5 = Assert.Throws<InvalidOperationException>(() => e5.BeginFinallyBlock(t5));
            Assert.Equal("Cannot begin FinallyBlock on Sigil.ExceptionBlock while inner ExceptionBlock Sigil.ExceptionBlock is still open", ex5.Message);

            var e6 = Emit<Action>.NewDynamicMethod("E6");
            var t6 = e6.BeginExceptionBlock();
            e6.LoadConstant(123);

            var ex6 = Assert.Throws<SigilVerificationException>(() => e6.BeginFinallyBlock(t6));
            Assert.Equal("BeginFinallyBlock expected the stack of be empty", ex6.Message);

            var e7 = Emit<Action>.NewDynamicMethod("E7");
            var t7 = e7.BeginExceptionBlock();
            var f7 = e7.BeginFinallyBlock(t7);
            e7.EndFinallyBlock(f7);

            var ex7 = Assert.Throws<InvalidOperationException>(() => e7.BeginFinallyBlock(t7));
            Assert.Equal("There can only be one finally block per ExceptionBlock, and one is already defined for Sigil.ExceptionBlock", ex7.Message);
        }

        [Fact]
        public void Box()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            var ex = Assert.Throws<ArgumentNullException>(() => e1.Box(null));
            Assert.Equal("valueType", ex.ParamName);

            var e2 = Emit<Action>.NewDynamicMethod("E2");

            var ex2 = Assert.Throws<ArgumentException>(() => e2.Box(typeof(string)));
            Assert.Equal("valueType", ex2.ParamName);

            var e3 = Emit<Action>.NewDynamicMethod("E3");
            var ex3 = Assert.Throws<SigilVerificationException>(() => e3.Box<int>());
            Assert.Equal("Box expects a value on the stack, but it was empty", ex3.Message);

            var e4 = Emit<Action>.NewDynamicMethod("E4");
            e4.LoadConstant("hello world");

            var ex4 = Assert.Throws<SigilVerificationException>(() => e4.Box<byte>());
            Assert.Equal("Box expected a System.Byte; found System.String", ex4.Message);

            var e5 = Emit<Action>.NewDynamicMethod("E5");
            e5.LoadConstant(1234);

            var ex5 = Assert.Throws<SigilVerificationException>(() => e5.Box<Guid>());
            Assert.Equal("Box expected a System.Guid; found int", ex5.Message);
        }

        [Fact]
        public void BadBranch()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");
            var l = e1.DefineLabel("l");
            var dead = e1.DefineLabel("dead_code");

            e1.LoadConstant(1);
            e1.Branch(l);

            e1.MarkLabel(dead);
            e1.Pop();
            e1.Return();

            e1.MarkLabel(l);

            var ex = Assert.Throws<SigilVerificationException>(() => e1.Return());
            Assert.Equal("Return expected the stack of be empty", ex.Message);
        }

        [Fact]
        public void StackCheck()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");
            e1.LoadConstant(1);
            e1.LoadConstant("123");

            var ex = Assert.Throws<SigilVerificationException>(() => e1.Add());
            Assert.Equal("Add expected a by ref, double, float, int, long, native int, or pointer; found System.String", ex.Message);
        }

        [Fact]
        public void MaxStackNonVerifying()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1", doVerify: false);

            var ex = Assert.Throws<InvalidOperationException>(() => e1.MaxStackSize);
            Assert.Equal("MaxStackSize is not available on non-verifying Emits", ex.Message);
        }
    }
}
