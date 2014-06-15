using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestClass, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class Errors
    {
        class _CallNonBaseClassConstructor1
        {
            public string Foo;

            public _CallNonBaseClassConstructor1(string foo)
            {
                Foo = foo;
            }
        }

        class _CallNonBaseClassConstructor2 : _CallNonBaseClassConstructor1
        {
            public _CallNonBaseClassConstructor2(string bar) : base(bar + bar) { }
        }

        [TestMethod]
        public void CallNonBaseClassConstructor()
        {
            var assembly = 
                AppDomain.CurrentDomain.DefineDynamicAssembly(
                    new AssemblyName("SigilTests_CallNonBaseClassConstructor"),
                    System.Reflection.Emit.AssemblyBuilderAccess.RunAndCollect
                );

            var module = assembly.DefineDynamicModule("RuntimeModule");
            var type = module.DefineType("_CallNonBaseClassConstructor3", TypeAttributes.Class, typeof(_CallNonBaseClassConstructor2));

            var ctor = Emit<Action>.BuildConstructor(type, MethodAttributes.Public);

            var baseBaseCtor = typeof(_CallNonBaseClassConstructor1).GetConstructor(new[] { typeof(string) });

            ctor.LoadArgument(0);
            ctor.LoadConstant("abc123");

            try
            {
                ctor.Call(baseBaseCtor);
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Only constructors defined in the current class or it's base class may be called", e.Message);
            }
        }

        [TestMethod]
        public void CallNonBaseClassConstructorNonGeneric()
        {
            var assembly =
                AppDomain.CurrentDomain.DefineDynamicAssembly(
                    new AssemblyName("SigilTests_CallNonBaseClassConstructorNonGeneric"),
                    System.Reflection.Emit.AssemblyBuilderAccess.RunAndCollect
                );

            var module = assembly.DefineDynamicModule("RuntimeModule");
            var type = module.DefineType("_CallNonBaseClassConstructor3", TypeAttributes.Class, typeof(_CallNonBaseClassConstructor2));

            var ctor = Sigil.NonGeneric.Emit.BuildConstructor(Type.EmptyTypes, type, MethodAttributes.Public);

            var baseBaseCtor = typeof(_CallNonBaseClassConstructor1).GetConstructor(new[] { typeof(string) });

            ctor.LoadArgument(0);
            ctor.LoadConstant("abc123");

            try
            {
                ctor.Call(baseBaseCtor);
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Only constructors defined in the current class or it's base class may be called", e.Message);
            }
        }

        class _CallContructorFromNonConstructor
        {
            public _CallContructorFromNonConstructor() { }
        }

        [TestMethod]
        public void CallContructorFromNonConstructor()
        {
            var cons = typeof(_CallContructorFromNonConstructor).GetConstructor(Type.EmptyTypes);

            var e1 = Emit<Action>.NewDynamicMethod();
            e1.NewObject(cons);

            try
            {
                e1.Call(cons);
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Constructors may only be called directly from within a constructor, use NewObject to allocate a new object with a specific constructor.", e.Message);
            }
        }

        [TestMethod]
        public void DisassemblingClosure()
        {
            var ret = 4;
            Func<int> d = () => ret;

            var ops = Disassembler<Func<int>>.Disassemble(d);
            Assert.IsFalse(ops.CanEmit);

            try
            {
                var e = ops.EmitAll();
                Assert.Fail();
            }
            catch (InvalidOperationException e)
            {
                Assert.AreEqual("Cannot emit this DisassembledOperations object, check CanEmit before calling any Emit methods", e.Message);
            }
        }

        [TestMethod]
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

            try
            {
                e1.CreateDelegate();

                Assert.Fail();
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("All execution paths must end with Return", e.Message);
                Assert.AreEqual("Bad Path\r\n========\r\n__start\r\nl1\r\n\r\nInstructions\r\n============\r\nbr.s l1\r\n\r\nl2:\r\nret\r\n\r\nl1:\r\nldc.i4.1\r\nbrtrue.s l2\r\n", e.GetDebugInfo());
            }
        }

        [TestMethod]
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

            try
            {
                e1.Pop();
                Assert.Fail();
            }
            catch (SigilVerificationException e)
            {
                var f = e.GetDebugInfo();
                Assert.AreEqual("Pop expects a value on the stack, but it was empty", e.Message);
                Assert.AreEqual("Stack\r\n=====\r\n--empty--\r\n\r\nInstructions\r\n============\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label0\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label1\r\n\r\n_label0:\r\n\r\n_label1:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label2\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label3\r\n\r\n_label2:\r\n\r\n_label3:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label4\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label5\r\n\r\n_label4:\r\n\r\n_label5:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label6\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label7\r\n\r\n_label6:\r\n\r\n_label7:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label8\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label9\r\n\r\n_label8:\r\n\r\n_label9:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label10\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label11\r\n\r\n_label10:\r\n\r\n_label11:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label12\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label13\r\n\r\n_label12:\r\n\r\n_label13:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label14\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label15\r\n\r\n_label14:\r\n\r\n_label15:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label16\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label17\r\n\r\n_label16:\r\n\r\n_label17:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label18\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label19\r\n\r\n_label18:\r\n\r\n_label19:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label20\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label21\r\n\r\n_label20:\r\n\r\n_label21:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label22\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label23\r\n\r\n_label22:\r\n\r\n_label23:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label24\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label25\r\n\r\n_label24:\r\n\r\n_label25:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label26\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label27\r\n\r\n_label26:\r\n\r\n_label27:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label28\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label29\r\n\r\n_label28:\r\n\r\n_label29:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label30\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label31\r\n\r\n_label30:\r\n\r\n_label31:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label32\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label33\r\n\r\n_label32:\r\n\r\n_label33:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label34\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label35\r\n\r\n_label34:\r\n\r\n_label35:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label36\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label37\r\n\r\n_label36:\r\n\r\n_label37:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label38\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label39\r\n\r\n_label38:\r\n\r\n_label39:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label40\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label41\r\n\r\n_label40:\r\n\r\n_label41:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label42\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label43\r\n\r\n_label42:\r\n\r\n_label43:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label44\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label45\r\n\r\n_label44:\r\n\r\n_label45:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label46\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label47\r\n\r\n_label46:\r\n\r\n_label47:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label48\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label49\r\n\r\n_label48:\r\n\r\n_label49:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label50\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label51\r\n\r\n_label50:\r\n\r\n_label51:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label52\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label53\r\n\r\n_label52:\r\n\r\n_label53:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label54\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label55\r\n\r\n_label54:\r\n\r\n_label55:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label56\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label57\r\n\r\n_label56:\r\n\r\n_label57:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label58\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label59\r\n\r\n_label58:\r\n\r\n_label59:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label60\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label61\r\n\r\n_label60:\r\n\r\n_label61:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label62\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label63\r\n\r\n_label62:\r\n\r\n_label63:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label64\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label65\r\n\r\n_label64:\r\n\r\n_label65:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label66\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label67\r\n\r\n_label66:\r\n\r\n_label67:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label68\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label69\r\n\r\n_label68:\r\n\r\n_label69:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label70\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label71\r\n\r\n_label70:\r\n\r\n_label71:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label72\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label73\r\n\r\n_label72:\r\n\r\n_label73:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label74\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label75\r\n\r\n_label74:\r\n\r\n_label75:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label76\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label77\r\n\r\n_label76:\r\n\r\n_label77:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label78\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label79\r\n\r\n_label78:\r\n\r\n_label79:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label80\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label81\r\n\r\n_label80:\r\n\r\n_label81:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label82\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label83\r\n\r\n_label82:\r\n\r\n_label83:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label84\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label85\r\n\r\n_label84:\r\n\r\n_label85:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label86\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label87\r\n\r\n_label86:\r\n\r\n_label87:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label88\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label89\r\n\r\n_label88:\r\n\r\n_label89:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label90\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label91\r\n\r\n_label90:\r\n\r\n_label91:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label92\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label93\r\n\r\n_label92:\r\n\r\n_label93:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label94\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label95\r\n\r\n_label94:\r\n\r\n_label95:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label96\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label97\r\n\r\n_label96:\r\n\r\n_label97:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label98\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label99\r\n\r\n_label98:\r\n\r\n_label99:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label100\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label101\r\n\r\n_label100:\r\n\r\n_label101:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label102\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label103\r\n\r\n_label102:\r\n\r\n_label103:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label104\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label105\r\n\r\n_label104:\r\n\r\n_label105:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label106\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label107\r\n\r\n_label106:\r\n\r\n_label107:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label108\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label109\r\n\r\n_label108:\r\n\r\n_label109:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label110\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label111\r\n\r\n_label110:\r\n\r\n_label111:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label112\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label113\r\n\r\n_label112:\r\n\r\n_label113:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label114\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label115\r\n\r\n_label114:\r\n\r\n_label115:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label116\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label117\r\n\r\n_label116:\r\n\r\n_label117:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label118\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label119\r\n\r\n_label118:\r\n\r\n_label119:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label120\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label121\r\n\r\n_label120:\r\n\r\n_label121:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label122\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label123\r\n\r\n_label122:\r\n\r\n_label123:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label124\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label125\r\n\r\n_label124:\r\n\r\n_label125:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label126\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label127\r\n\r\n_label126:\r\n\r\n_label127:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label128\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label129\r\n\r\n_label128:\r\n\r\n_label129:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label130\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label131\r\n\r\n_label130:\r\n\r\n_label131:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label132\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label133\r\n\r\n_label132:\r\n\r\n_label133:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label134\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label135\r\n\r\n_label134:\r\n\r\n_label135:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label136\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label137\r\n\r\n_label136:\r\n\r\n_label137:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label138\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label139\r\n\r\n_label138:\r\n\r\n_label139:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label140\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label141\r\n\r\n_label140:\r\n\r\n_label141:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label142\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label143\r\n\r\n_label142:\r\n\r\n_label143:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label144\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label145\r\n\r\n_label144:\r\n\r\n_label145:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label146\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label147\r\n\r\n_label146:\r\n\r\n_label147:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label148\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label149\r\n\r\n_label148:\r\n\r\n_label149:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label150\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label151\r\n\r\n_label150:\r\n\r\n_label151:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label152\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label153\r\n\r\n_label152:\r\n\r\n_label153:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label154\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label155\r\n\r\n_label154:\r\n\r\n_label155:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label156\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label157\r\n\r\n_label156:\r\n\r\n_label157:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label158\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label159\r\n\r\n_label158:\r\n\r\n_label159:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label160\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label161\r\n\r\n_label160:\r\n\r\n_label161:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label162\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label163\r\n\r\n_label162:\r\n\r\n_label163:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label164\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label165\r\n\r\n_label164:\r\n\r\n_label165:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label166\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label167\r\n\r\n_label166:\r\n\r\n_label167:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label168\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label169\r\n\r\n_label168:\r\n\r\n_label169:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label170\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label171\r\n\r\n_label170:\r\n\r\n_label171:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label172\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label173\r\n\r\n_label172:\r\n\r\n_label173:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label174\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label175\r\n\r\n_label174:\r\n\r\n_label175:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label176\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label177\r\n\r\n_label176:\r\n\r\n_label177:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label178\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label179\r\n\r\n_label178:\r\n\r\n_label179:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label180\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label181\r\n\r\n_label180:\r\n\r\n_label181:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label182\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label183\r\n\r\n_label182:\r\n\r\n_label183:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label184\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label185\r\n\r\n_label184:\r\n\r\n_label185:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label186\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label187\r\n\r\n_label186:\r\n\r\n_label187:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label188\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label189\r\n\r\n_label188:\r\n\r\n_label189:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label190\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label191\r\n\r\n_label190:\r\n\r\n_label191:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label192\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label193\r\n\r\n_label192:\r\n\r\n_label193:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label194\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label195\r\n\r\n_label194:\r\n\r\n_label195:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label196\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label197\r\n\r\n_label196:\r\n\r\n_label197:\r\nldc.i4.0\r\nldc.i4.1\r\nbgt _label198\r\nldc.i4.2\r\nldc.i4.3\r\nblt _label199\r\n\r\n_label198:\r\n\r\n_label199:\r\n", f);
            }
        }

        [TestMethod]
        public void BadOptimizationOptions()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            e1.Return();

            try
            {
                e1.CreateDelegate((OptimizationOptions)123);
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("optimizationOptions contained unknown flags, found 123", e.Message);
            }
        }

        [TestMethod]
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

            try
            {
                e1.Return();

                Assert.Fail();
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Return expects a value on the stack, but it was empty", e.Message);
            }
        }

        [TestMethod]
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

            try
            {
                e1.Branch(l2);

                Assert.Fail();
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Branch expects a value on the stack, but it was empty", e.Message);
                var debug = e.GetDebugInfo();
                Assert.AreEqual("Stack\r\n=====\r\n--empty--\r\n\r\nInstructions\r\n============\r\nldarg.0\r\nbr l1\r\n\r\nl2:\r\npop\r\ncallvirt System.String ToString()  // relevant instruction\r\nbr l3\r\n\r\nl1:\r\nbr l2\r\n", debug);
            }
        }

        [TestMethod]
        public void BadStackVals()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.LoadConstant(1);
                e1.LoadConstant("foo");

                try
                {
                    e1.Add();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    var debug = e.GetDebugInfo();
                    Assert.AreEqual("Stack\r\n=====\r\nSystem.String  // bad value\r\nint\r\n\r\nInstructions\r\n============\r\nldc.i4.1\r\nldstr 'foo'\r\n", debug);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.LoadConstant("foo");
                e1.LoadConstant(1);

                try
                {
                    e1.Add();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    var debug = e.GetDebugInfo();
                    Assert.AreEqual("Stack\r\n=====\r\nint\r\nSystem.String  // bad value\r\n\r\nInstructions\r\n============\r\nldstr 'foo'\r\nldc.i4.1\r\n", debug);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var substring = typeof(string).GetMethod("Substring", new[] { typeof(int), typeof(int) });
                e1.LoadConstant("foo");
                e1.LoadConstant(1);
                e1.LoadConstant(2.0);

                try
                {
                    e1.Call(substring);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    var debug = e.GetDebugInfo();
                    Assert.AreEqual("Stack\r\n=====\r\ndouble  // bad value\r\nint\r\nSystem.String\r\n\r\nInstructions\r\n============\r\nldstr 'foo'\r\nldc.i4.1\r\nldc.r8 2\r\n", debug);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var substring = typeof(string).GetMethod("Substring", new[] { typeof(int), typeof(int) });
                e1.LoadConstant("foo");
                e1.LoadConstant(1f);
                e1.LoadConstant(2);

                try
                {
                    e1.Call(substring);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    var debug = e.GetDebugInfo();
                    Assert.AreEqual("Stack\r\n=====\r\nint\r\nfloat  // bad value\r\nSystem.String\r\n\r\nInstructions\r\n============\r\nldstr 'foo'\r\nldc.r4 1\r\nldc.i4.2\r\n", debug);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var substring = typeof(string).GetMethod("Substring", new[] { typeof(int), typeof(int) });
                e1.NewObject<object>();
                e1.LoadConstant(1);
                e1.LoadConstant(2);

                try
                {
                    e1.Call(substring);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    var debug = e.GetDebugInfo();
                    Assert.AreEqual("Stack\r\n=====\r\nint\r\nint\r\nSystem.Object  // bad value\r\n\r\nInstructions\r\n============\r\nnewobj Void .ctor()\r\nldc.i4.1\r\nldc.i4.2\r\n", debug);
                }
            }
        }

        [TestMethod]
        public void LabelDiffStack()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var l = e1.DefineLabel();

                e1.LoadConstant(1);
                e1.BranchIfFalse(l);
                e1.LoadConstant(4);

                try
                {
                    e1.MarkLabel(l);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    var f = e.GetDebugInfo();
                    Assert.AreEqual("MarkLabel expects 0 values on the stack", e.Message);
                    Assert.AreEqual("\r\nInstructions\r\n============\r\nldc.i4.1\r\nbrfalse _label0\r\nldc.i4.4\r\n", f);
                }
            }
        }

        [TestMethod]
        public void BranchDiffStack()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            var l = e1.DefineLabel();

            e1.LoadConstant(1);
            e1.MarkLabel(l);
            e1.Pop();
            e1.LoadConstant("123");

            try
            {
                e1.Branch(l);
                Assert.Fail();
            }
            catch (SigilVerificationException e)
            {
                var f = e.GetDebugInfo();
                Assert.AreEqual("Branch expected an int; found System.String", e.Message);
                Assert.AreEqual("\r\nInstructions\r\n============\r\nldc.i4.1\r\n\r\n_label0:\r\npop\r\nldstr '123'\r\nbr _label0\r\n", f);
            }
        }

        [TestMethod]
        public void Unreachable()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            var l = e1.DefineLabel();

            e1.Branch(l);

            try
            {
                e1.Pop();
                Assert.Fail();
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Unreachable code detected", e.Message);
            }
        }

        [TestMethod]
        public void WriteLine()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.WriteLine(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("line", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.WriteLine("foo", null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("locals", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var e2 = Emit<Action>.NewDynamicMethod();
                var a = e2.DeclareLocal<int>();

                try
                {
                    e1.WriteLine("foo", a);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("System.Int32 _local0 is not owned by this Emit, and thus cannot be used", e.Message);
                }
            }
        }

        [TestMethod]
        public void DoubleLabelDeclaration()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            e1.DefineLabel("a");

            try
            {
                e1.DefineLabel("a");
                Assert.Fail();
            }
            catch (InvalidOperationException e)
            {
                Assert.AreEqual("Label with name 'a' already exists", e.Message);
            }
        }

        [TestMethod]
        public void DoubleLocalDeclaration()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            var a = e1.DeclareLocal<int>("a");

            try
            {
                e1.DeclareLocal<string>("a");
                Assert.Fail();
            }
            catch (InvalidOperationException e)
            {
                Assert.AreEqual("Local with name 'a' already exists", e.Message);
            }

            a.Dispose();

            e1.DeclareLocal<string>("a");
        }

        [TestMethod]
        public void GenericThisCall()
        {
            {
                var e1 = Emit<Action<Action<string>>>.NewDynamicMethod("E1");
                var invoke = typeof(Action<int>).GetMethod("Invoke");

                try
                {
                    e1.LoadArgument(0);
                    e1.LoadConstant(1);
                    e1.CallVirtual(invoke);

                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("CallVirtual expected a System.Action`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]; found System.Action`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", e.Message);
                }
            }
        }

        [TestMethod]
        public void Compare()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.LoadConstant(0);
                e1.LoadConstant(1.0f);

                try
                {
                    e1.CompareLessThan();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("CompareLessThan expected a float; found int", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.NewObject<object>();

                try
                {
                    e1.CompareLessThan();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("CompareLessThan expected a double, float, int, long, or native int; found System.Object", e.Message);
                }
            }
        }

        [TestMethod]
        public void AdditionalValidation()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var t = e1.BeginExceptionBlock();
                e1.Return();

                try
                {
                    var d1 = e1.CreateDelegate();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("Unended ExceptionBlock Sigil.ExceptionBlock", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.Pop();

                try
                {
                    var d1 = e1.CreateDelegate();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("All execution paths must end with Return", e.Message);
                }
            }
        }

        [TestMethod]
        public void UnboxAny()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.UnboxAny(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("valueType", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.UnboxAny(typeof(void));
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("Void cannot be boxed, and thus cannot be unboxed", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.UnboxAny<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("UnboxAny expects a value on the stack, but it was empty", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.LoadConstant("hello");

                try
                {
                    e1.UnboxAny<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("UnboxAny expected a System.Object; found System.String", e.Message);
                }
            }
        }

        [TestMethod]
        public void Unbox()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.Unbox(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("valueType", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.Unbox(typeof(string));
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("Unbox expects a ValueType, found System.String", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.Unbox(typeof(void));
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("Void cannot be boxed, and thus cannot be unboxed", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.Unbox<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("Unbox expects a value on the stack, but it was empty", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.LoadConstant("hello");

                try
                {
                    e1.Unbox<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("Unbox expected a System.Object; found System.String", e.Message);
                }
            }
        }

        [TestMethod]
        public void EndFinallyBlock()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.EndFinallyBlock(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("forFinally", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var e2 = Emit<Action>.NewDynamicMethod();
                var t = e2.BeginExceptionBlock();
                var f = e2.BeginFinallyBlock(t);

                try
                {
                    e1.EndFinallyBlock(f);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("Sigil.FinallyBlock is not owned by this Emit, and thus cannot be used", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var t = e1.BeginExceptionBlock();
                var f = e1.BeginFinallyBlock(t);
                e1.EndFinallyBlock(f);

                try
                {
                    e1.EndFinallyBlock(f);
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("EndFinallyBlock expects an unclosed finally block, but Sigil.FinallyBlock is already closed", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var t = e1.BeginExceptionBlock();
                var f = e1.BeginFinallyBlock(t);
                e1.NewObject<object>();

                try
                {
                    e1.EndFinallyBlock(f);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("EndFinallyBlock expected the stack of be empty", e.Message);
                }
            }
        }

        [TestMethod]
        public void EndCatchBlock()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.EndCatchBlock(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("forCatch", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var e2 = Emit<Action>.NewDynamicMethod();

                var t = e2.BeginExceptionBlock();
                var c = e2.BeginCatchAllBlock(t);

                try
                {
                    e1.EndCatchBlock(c);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("Sigil.CatchBlock is not owned by this Emit, and thus cannot be used", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var t = e1.BeginExceptionBlock();
                var c = e1.BeginCatchAllBlock(t);

                try
                {
                    e1.EndCatchBlock(c);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("EndCatchBlock expected the stack of be empty", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var t = e1.BeginExceptionBlock();
                var c = e1.BeginCatchAllBlock(t);
                e1.Pop();
                e1.EndCatchBlock(c);

                try
                {
                    e1.EndCatchBlock(c);
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("CatchBlock has already been ended", e.Message);
                }
            }
        }

        [TestMethod]
        public void EndExceptionBlock()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.EndExceptionBlock(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("forTry", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var e2 = Emit<Action>.NewDynamicMethod();
                var t = e2.BeginExceptionBlock();

                try
                {
                    e1.EndExceptionBlock(t);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("Sigil.ExceptionBlock is not owned by this Emit, and thus cannot be used", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var t = e1.BeginExceptionBlock();
                var c = e1.BeginCatchAllBlock(t);
                e1.Pop();
                e1.EndCatchBlock(c);
                e1.EndExceptionBlock(t);

                try
                {
                    e1.EndExceptionBlock(t);
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("ExceptionBlock has already been ended", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var t = e1.BeginExceptionBlock();
                var c = e1.BeginCatchAllBlock(t);

                try
                {
                    e1.EndExceptionBlock(t);
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("Cannot end ExceptionBlock, CatchBlock Sigil.CatchBlock has not been ended", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var t = e1.BeginExceptionBlock();
                var f = e1.BeginFinallyBlock(t);

                try
                {
                    e1.EndExceptionBlock(t);
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("Cannot end ExceptionBlock, FinallyBlock Sigil.FinallyBlock has not been ended", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var t = e1.BeginExceptionBlock();

                try
                {
                    e1.EndExceptionBlock(t);
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("Cannot end ExceptionBlock without defining at least one of a catch or finally block", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var t1 = e1.BeginExceptionBlock();
                var t2 = e1.BeginExceptionBlock();

                try
                {
                    e1.EndExceptionBlock(t1);
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("Cannot end outer ExceptionBlock Sigil.ExceptionBlock while inner EmitExceptionBlock Sigil.ExceptionBlock is open", e.Message);
                }
            }
        }

        [TestMethod]
        public void Throw()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.Throw();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("Throw expects a value on the stack, but it was empty", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();

                try
                {
                    e1.Throw();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("Throw expected a System.Exception; found System.Object", e.Message);
                }
            }
        }

        [TestMethod]
        public void Switch()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();
                try
                {
                    e1.Switch((Label[])null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("labels", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.Switch(new Label[0]);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("labels must have at least one element", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var e2 = Emit<Action>.NewDynamicMethod();
                var l = e2.DefineLabel();
                try
                {
                    e1.Switch(l);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("_label0 is not owned by this Emit, and thus cannot be used", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var l = e1.DefineLabel();
                try
                {
                    e1.Switch(l);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("Switch expects a value on the stack, but it was empty", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var l = e1.DefineLabel();

                e1.NewObject<object>();

                try
                {
                    e1.Switch(l);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("Switch expected an int, or native int; found System.Object", e.Message);
                }
            }
        }

        [TestMethod]
        public void StoreObject()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.StoreObject(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("valueType", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.StoreObject(typeof(string));
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("valueType must be a ValueType", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.StoreObject(typeof(int), unaligned: 3);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("unaligned must be null, 1, 2, or 4", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.StoreObject(typeof(int));
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("StoreObject expects 2 values on the stack", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.NewObject<object>();

                try
                {
                    e1.StoreObject(typeof(int));
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("StoreObject expected an int; found System.Object", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.LoadConstant(0);

                try
                {
                    e1.StoreObject(typeof(int));
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("StoreObject expected a native int, System.Int32&, or System.Int32*; found System.Object", e.Message);
                }
            }
        }

        [TestMethod]
        public void StoreLocal()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.StoreLocal((Sigil.Local)null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("local", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var e2 = Emit<Action>.NewDynamicMethod();
                var l = e2.DeclareLocal<int>();

                try
                {
                    e1.StoreLocal(l);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("System.Int32 _local0 is not owned by this Emit, and thus cannot be used", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var l = e1.DeclareLocal<int>();

                try
                {
                    e1.StoreLocal(l);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("StoreLocal expects a value on the stack, but it was empty", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var l = e1.DeclareLocal<int>();
                e1.NewObject<object>();

                try
                {
                    e1.StoreLocal(l);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("StoreLocal expected an int; found System.Object", e.Message);
                }
            }
        }

        [TestMethod]
        public void StoreIndirect()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.StoreIndirect(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("type", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.StoreIndirect(typeof(int), unaligned: 3);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("unaligned must be null, 1, 2, or 4", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.StoreIndirect(typeof(int));
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("StoreIndirect expects 2 values on the stack", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.NewObject<object>();

                try
                {
                    e1.StoreIndirect(typeof(int));
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("StoreIndirect expected an int; found System.Object", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.LoadConstant(0);

                try
                {
                    e1.StoreIndirect(typeof(int));
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("StoreIndirect expected a native int, System.Int32&, System.Int32*, System.UInt32&, or System.UInt32*; found System.Object", e.Message);
                }
            }

            {
                var e1 = Emit<Action<DateTime>>.NewDynamicMethod();
                e1.LoadConstant(0);
                e1.Convert<IntPtr>();
                e1.LoadArgument(0);

                try
                {
                    e1.StoreIndirect(typeof(DateTime));
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("StoreIndirect cannot be used with System.DateTime, StoreObject may be more appropriate", e.Message);
                }
            }
        }

        class StoreFieldClass
        {
            public static int Static;
            public int Instance;

            public StoreFieldClass()
            {
                Static = Instance = 2;
            }
        }

        [TestMethod]
        public void StoreField()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.StoreField(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("field", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(int).GetField("MaxValue");

                try
                {
                    e1.StoreField(f, unaligned: 3);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("unaligned must be null, 1, 2, or 4", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(int).GetField("MaxValue");

                try
                {
                    e1.StoreField(f, unaligned: 4);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("unaligned cannot be used with static fields", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(StoreFieldClass).GetField("Instance");

                try
                {
                    e1.StoreField(f);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("StoreField expects 2 values on the stack", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(StoreFieldClass).GetField("Instance");
                e1.NewObject<object>();
                e1.NewObject<object>();

                try
                {
                    e1.StoreField(f);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("StoreField expected an int; found System.Object", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(StoreFieldClass).GetField("Instance");
                e1.NewObject<object>();
                e1.LoadConstant(0);

                try
                {
                    e1.StoreField(f);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("StoreField expected a SigilTests.Errors+StoreFieldClass; found System.Object", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(StoreFieldClass).GetField("Static");

                try
                {
                    e1.StoreField(f);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("StoreField expects a value on the stack, but it was empty", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(StoreFieldClass).GetField("Static");
                e1.NewObject<object>();

                try
                {
                    e1.StoreField(f);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("StoreField expected an int; found System.Object", e.Message);
                }
            }
        }

        [TestMethod]
        public void StoreElement()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.StoreElement<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("StoreElement expects 3 values on the stack", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.NewObject<object>();
                e1.NewObject<object>();

                try
                {
                    e1.StoreElement<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("StoreElement expected an int; found System.Object", e.Message);
                }
            }

            {
                var e1 = Emit<Action<int[], int, int>>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.NewObject<object>();
                e1.LoadArgument(2);

                try
                {
                    e1.StoreElement<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("StoreElement expected an int, or native int; found System.Object", e.Message);
                }
            }

            {
                var e1 = Emit<Action<int[], int, int>>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.LoadArgument(1);
                e1.LoadArgument(2);

                try
                {
                    e1.StoreElement<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("StoreElement expected a System.Int32[]; found System.Object", e.Message);
                }
            }
        }

        [TestMethod]
        public void StoreArgument()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.StoreArgument(0);
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("Delegate of type System.Action takes no parameters", e.Message);
                }
            }

            {
                var e1 = Emit<Action<int>>.NewDynamicMethod();

                try
                {
                    e1.StoreArgument(4);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("index must be between 0 and 0, inclusive", e.Message);
                }
            }

            {
                var e1 = Emit<Action<int>>.NewDynamicMethod();

                try
                {
                    e1.StoreArgument(0);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("StoreArgument expects a value on the stack, but it was empty", e.Message);
                }
            }

            {
                var e1 = Emit<Action<int>>.NewDynamicMethod();
                e1.NewObject<object>();

                try
                {
                    e1.StoreArgument(0);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("StoreArgument expected an int; found System.Object", e.Message);
                }
            }
        }

        [TestMethod]
        public void SizeOf()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.SizeOf(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("valueType", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.SizeOf(typeof(string));
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("valueType must be a ValueType", e.Message);
                }
            }
        }

        [TestMethod]
        public void Return()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.LoadConstant(0);

                try
                {
                    e1.Return();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("Return expected the stack of be empty", e.Message);
                }
            }

            {
                var e1 = Emit<Func<int>>.NewDynamicMethod();

                try
                {
                    e1.Return();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("Return expects a value on the stack, but it was empty", e.Message);
                }
            }

            {
                var e1 = Emit<Func<int>>.NewDynamicMethod();
                e1.NewObject<object>();

                try
                {
                    e1.Return();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("Return expected an int; found System.Object", e.Message);
                }
            }

            {
                var e1 = Emit<Func<int>>.NewDynamicMethod();
                e1.LoadConstant(0);
                e1.LoadConstant(0);

                try
                {
                    e1.Return();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("Return expected the stack of be empty", e.Message);
                }
            }
        }

        [TestMethod]
        public void ReThrow()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.ReThrow();
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("ReThrow is only legal in a catch block", e.Message);
                }
            }
        }

        [TestMethod]
        public void Pop()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.Pop();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("Pop expects a value on the stack, but it was empty", e.Message);
                }
            }
        }

        struct NewObjectStruct
        {
            public NewObjectStruct(int i)
            {

            }
        }

        [TestMethod]
        public void NewObject()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.NewObject(null, null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("type", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.NewObject(typeof(object), null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("parameterTypes", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.NewObject(typeof(object), typeof(int), typeof(string));
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("Type System.Object must have a constructor that matches parameters [System.Int32, System.String]", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.NewObject((ConstructorInfo)null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("constructor", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.NewObject<string, char[]>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("NewObject expects a value on the stack, but it was empty", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();

                try
                {
                    e1.NewObject<string, char[]>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("NewObject expected a System.Char[]; found System.Object", e.Message);
                }
            }
        }

        [TestMethod]
        public void NewArray()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.NewArray(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("elementType", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.NewArray<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("NewArray expects a value on the stack, but it was empty", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();

                try
                {
                    e1.NewArray<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("NewArray expected an int, or native int; found System.Object", e.Message);
                }
            }
        }

        [TestMethod]
        public void Locals()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.DeclareLocal(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("type", e.ParamName);
                }
            }
        }

        [TestMethod]
        public void LocalAllocate()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var t = e1.BeginExceptionBlock();
                var c = e1.BeginCatchAllBlock(t);
                try
                {
                    e1.LocalAllocate();
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("LocalAllocate cannot be used in a catch block", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var t = e1.BeginExceptionBlock();
                var c = e1.BeginCatchAllBlock(t);
                e1.Pop();
                e1.EndCatchBlock(c);
                var f = e1.BeginFinallyBlock(t);
                try
                {
                    e1.LocalAllocate();
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("LocalAllocate cannot be used in a finally block", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.LocalAllocate();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("LocalAllocate expected the stack to have 1 value", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();

                try
                {
                    e1.LocalAllocate();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("LocalAllocate expected an int, or native int; found System.Object", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.LoadConstant(0);
                e1.LoadConstant(0);

                try
                {
                    e1.LocalAllocate();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("LocalAllocate expected the stack to have 1 value", e.Message);
                }
            }
        }

        [TestMethod]
        public void LoadVirtualFunctionPointer()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.LoadVirtualFunctionPointer(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("method", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(string).GetMethod("Intern");

                try
                {
                    e1.LoadVirtualFunctionPointer(f);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("Only non-static methods can be passed to LoadVirtualFunctionPointer, found System.String Intern(System.String)", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(object).GetMethod("ToString");

                try
                {
                    e1.LoadVirtualFunctionPointer(f);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("LoadVirtualFunctionPointer expects a value on the stack, but it was empty", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(List<int>).GetMethod("Add");

                e1.LoadConstant(0);

                try
                {
                    e1.LoadVirtualFunctionPointer(f);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("LoadVirtualFunctionPointer expected a System.Collections.Generic.List`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]; found int", e.Message);
                }
            }
        }

        [TestMethod]
        public void LoadObject()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.LoadObject(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("valueType", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.LoadObject(typeof(string));
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("valueType must be a ValueType", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.LoadObject<DateTime>(unaligned: 3);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("unaligned must be null, 1, 2, or 4", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.LoadObject<DateTime>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("LoadObject expects a value on the stack, but it was empty", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();

                try
                {
                    e1.LoadObject<DateTime>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("LoadObject expected a native int, System.DateTime&, or System.DateTime*; found System.Object", e.Message);
                }
            }
        }

        [TestMethod]
        public void LoadLocalAddress()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.LoadLocalAddress((Sigil.Local)null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("local", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var e2 = Emit<Action>.NewDynamicMethod();
                var l = e2.DeclareLocal<int>();

                try
                {
                    e1.LoadLocalAddress(l);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("System.Int32 _local0 is not owned by this Emit, and thus cannot be used", e.Message);
                }
            }
        }

        [TestMethod]
        public void LoadLocal()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.LoadLocal((Sigil.Local)null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("local", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var e2 = Emit<Action>.NewDynamicMethod();
                var l = e2.DeclareLocal<int>();

                try
                {
                    e1.LoadLocal(l);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("System.Int32 _local0 is not owned by this Emit, and thus cannot be used", e.Message);
                }
            }
        }

        [TestMethod]
        public void LoadLength()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.LoadLength<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("LoadLength expects a value on the stack, but it was empty", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();

                try
                {
                    e1.LoadLength<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("LoadLength expected a System.Int32[]; found System.Object", e.Message);
                }
            }
        }

        [TestMethod]
        public void LoadIndirect()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.LoadIndirect(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("type", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.LoadIndirect<int>(unaligned: 3);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("unaligned must be null, 1, 2, or 4", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.LoadIndirect<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("LoadIndirect expects a value on the stack, but it was empty", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();

                try
                {
                    e1.LoadIndirect<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("LoadIndirect expected a native int, System.Int32&, or System.Int32*; found System.Object", e.Message);
                }
            }

            {
                var e1 = Emit<Action<DateTime>>.NewDynamicMethod();
                e1.LoadArgumentAddress(0);

                try
                {
                    e1.LoadIndirect<DateTime>();
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("LoadIndirect cannot be used with System.DateTime, LoadObject may be more appropriate", e.Message);
                }
            }
        }

        [TestMethod]
        public void LoadFunctionPointer()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.LoadFunctionPointer(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("method", e.ParamName);
                }
            }
        }

        [TestMethod]
        public void LoadFieldAddress()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.LoadFieldAddress(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("field", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(LoadFieldClass).GetField("A");

                try
                {
                    e1.LoadFieldAddress(f);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("LoadFieldAddress expects a value on the stack, but it was empty", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(LoadFieldClass).GetField("A");

                e1.NewObject<object>();

                try
                {
                    e1.LoadFieldAddress(f);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("LoadFieldAddress expected a SigilTests.Errors+LoadFieldClass; found System.Object", e.Message);
                }
            }
        }

        class LoadFieldClass
        {
            public int A;

            public LoadFieldClass()
            {
                A = 123;
            }
        }

        [TestMethod]
        public void LoadField()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.LoadField(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("field", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(int).GetField("MaxValue");

                try
                {
                    e1.LoadField(f, unaligned: 1);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("unaligned cannot be used with static fields", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(int).GetField("MaxValue");

                try
                {
                    e1.LoadField(f, unaligned: 3);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("unaligned must be null, 1, 2, or 4", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(LoadFieldClass).GetField("A");

                try
                {
                    e1.LoadField(f);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("LoadField expects a value on the stack, but it was empty", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var f = typeof(LoadFieldClass).GetField("A");

                e1.NewObject<object>();

                try
                {
                    e1.LoadField(f);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("LoadField expected a SigilTests.Errors+LoadFieldClass; found System.Object", e.Message);
                }
            }
        }

        [TestMethod]
        public void LoadElementAddress()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.LoadElementAddress<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("LoadElementAddress expects 2 values on the stack", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.NewObject<object>();

                try
                {
                    e1.LoadElementAddress<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("LoadElementAddress expected an int, or native int; found System.Object", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.LoadConstant(0);
                e1.LoadConstant(0);

                try
                {
                    e1.LoadElementAddress<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("LoadElementAddress expected a System.Int32[]; found int", e.Message);
                }
            }

            {
                var e1 = Emit<Action<int[,]>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadConstant(0);

                try
                {
                    e1.LoadElementAddress<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("LoadElementAddress expected a System.Int32[]; found System.Int32[,]", e.Message);
                }
            }
        }

        [TestMethod]
        public void LoadElement()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.LoadElement<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("LoadElement expects 2 values on the stack", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.NewObject<object>();

                try
                {
                    e1.LoadElement<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("LoadElement expected an int, or native int; found System.Object", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.LoadConstant(0);

                try
                {
                    e1.LoadElement<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("LoadElement expected a System.Int32[]; found System.Object", e.Message);
                }
            }

            {
                var e1 = Emit<Action<int[,], int>>.NewDynamicMethod();
                e1.LoadArgument(0);
                e1.LoadArgument(1);

                try
                {
                    e1.LoadElement<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("LoadElement expected a System.Int32[]; found System.Int32[,]", e.Message);
                }
            }
        }

        [TestMethod]
        public void LoadConstant()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.LoadConstant((Type)null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("type", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.LoadConstant((MethodInfo)null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("method", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.LoadConstant((FieldInfo)null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("field", e.ParamName);
                }
            }
        }

        [TestMethod]
        public void LoadArgumentAddress()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.LoadArgumentAddress(0);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("Delegate of type System.Action takes no parameters", e.Message);
                }
            }

            {
                var e1 = Emit<Action<int>>.NewDynamicMethod();

                try
                {
                    e1.LoadArgumentAddress(4);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("index must be between 0 and 0, inclusive", e.Message);
                }
            }
        }

        [TestMethod]
        public void LoadArgument()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();
                try
                {
                    e1.LoadArgument(0);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("Delegate of type System.Action takes no parameters", e.Message);
                }
            }

            {
                var e1 = Emit<Action<int>>.NewDynamicMethod();
                try
                {
                    e1.LoadArgument(4);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("index must be between 0 and 0, inclusive", e.Message);
                }
            }
        }

        [TestMethod]
        public void Leave()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.Leave((Sigil.Label)null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("label", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var e2 = Emit<Action>.NewDynamicMethod();
                var l = e2.DefineLabel();

                try
                {
                    e1.Leave(l);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("_label0 is not owned by this Emit, and thus cannot be used", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var l = e1.DefineLabel();

                try
                {
                    e1.Leave(l);
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("Leave can only be used within an exception or catch block", e.Message);
                }
            }
        }

        [TestMethod]
        public void Labels()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var l = e1.DefineLabel();
                var d = e1.DefineLabel();
                e1.Branch(l);
                e1.MarkLabel(d);
                e1.Return();

                try
                {
                    var d1 = e1.CreateDelegate();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("Usage of unmarked label _label0", e.Message);
                }
            }
        }

        [TestMethod]
        public void MarkLabel()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.MarkLabel((Sigil.Label)null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("label", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var e2 = Emit<Action>.NewDynamicMethod();
                var l = e2.DefineLabel();

                try
                {
                    e1.MarkLabel(l);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("_label0 is not owned by this Emit, and thus cannot be used", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var l = e1.DefineLabel();
                e1.MarkLabel(l);

                try
                {
                    e1.MarkLabel(l);
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("label [_label0] has already been marked, and cannot be marked a second time", e.Message);
                }
            }
        }

        [TestMethod]
        public void Jump()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.Jump(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("method", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var toString = typeof(object).GetMethod("ToString");
                try
                {
                    e1.Jump(toString);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("Jump expected a calling convention of Standard, found Standard, HasThis", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var intern = typeof(string).GetMethod("Intern");
                try
                {
                    e1.Jump(intern);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("Jump expected a method with 0 parameters, found 1", e.Message);
                }
            }

            {
                var e1 = Emit<Action<string>>.NewDynamicMethod();
                var intern = typeof(string).GetMethod("Intern");

                e1.LoadArgument(0);

                try
                {
                    e1.Jump(intern);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("Jump expected the stack of be empty", e.Message);
                }
            }

            {
                var e1 = Emit<Action<int>>.NewDynamicMethod();
                var intern = typeof(string).GetMethod("Intern");

                try
                {
                    e1.Jump(intern);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("Jump expected the #0 parameter to be assignable from System.Int32, but found System.String", e.Message);
                }
            }

            {
                var e1 = Emit<Action<string>>.NewDynamicMethod();
                var intern = typeof(string).GetMethod("Intern");

                e1.BeginExceptionBlock();

                try
                {
                    e1.Jump(intern);
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("Jump cannot transfer control from an exception block", e.Message);
                }
            }

            {
                var e1 = Emit<Action<string>>.NewDynamicMethod();
                var intern = typeof(string).GetMethod("Intern");

                var t = e1.BeginExceptionBlock();
                e1.BeginCatchAllBlock(t);

                try
                {
                    e1.Jump(intern);
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("Jump cannot transfer control from a catch block", e.Message);
                }
            }

            {
                var e1 = Emit<Action<string>>.NewDynamicMethod();
                var intern = typeof(string).GetMethod("Intern");

                var t = e1.BeginExceptionBlock();
                var c = e1.BeginCatchAllBlock(t);
                e1.Pop();
                e1.EndCatchBlock(c);
                e1.BeginFinallyBlock(t);

                try
                {
                    e1.Jump(intern);
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("Jump cannot transfer control from a finally block", e.Message);
                }
            }
        }

        [TestMethod]
        public void IsInstance()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.IsInstance(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("type", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.IsInstance<string>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("IsInstance expects a value on the stack, but it was empty", e.Message);
                }
            }
        }

        [TestMethod]
        public void InitializeObject()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.InitializeObject(null);
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("valueType", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.InitializeObject<int>();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("InitializeObject expects a value on the stack, but it was empty", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();

                try
                {
                    e1.InitializeObject<DateTime>();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("InitializeObject expected a native int, System.DateTime&, or System.DateTime*; found System.Object", e.Message);
                }
            }
        }

        [TestMethod]
        public void InitializeBlock()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.InitializeBlock(unaligned: 3);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("unaligned must be null, 1, 2, or 4", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.InitializeBlock();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("InitializeBlock expects 3 values on the stack", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.NewObject<object>();
                e1.NewObject<object>();

                try
                {
                    e1.InitializeBlock();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("InitializeBlock expected an int; found System.Object", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.NewObject<object>();
                e1.LoadConstant(0);

                try
                {
                    e1.InitializeBlock();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("InitializeBlock expected an int, or native int; found System.Object", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.LoadConstant(0);
                e1.Convert<IntPtr>();
                e1.LoadConstant(0);

                try
                {
                    e1.InitializeBlock();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("InitializeBlock expected a native int, System.Byte&, or System.Byte*; found System.Object", e.Message);
                }
            }
        }

        [TestMethod]
        public void Duplicate()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.Duplicate();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("Duplicate expects a value on the stack, but it was empty", e.Message);
                }
            }
        }

        [TestMethod]
        public void CopyObject()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.CopyObject(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("valueType", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.CopyObject(typeof(string));
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("CopyObject expects a ValueType; found System.String", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.CopyObject<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("CopyObject expects 2 values on the stack", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.NewObject<object>();

                try
                {
                    e1.CopyObject<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("CopyObject expected a native int, System.Int32&, or System.Int32*; found System.Object", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.LoadConstant(0);
                e1.Convert<IntPtr>();

                try
                {
                    e1.CopyObject<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("CopyObject expected a native int, System.Int32&, or System.Int32*; found System.Object", e.Message);
                }
            }
        }

        [TestMethod]
        public void CopyBlock()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();
                try
                {
                    e1.CopyBlock(unaligned: 3);
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("unaligned must be null, 1, 2, or 4\r\nParameter name: unaligned", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                try
                {
                    e1.CopyBlock();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("CopyBlock expects 3 values on the stack", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.NewObject<object>();
                e1.NewObject<object>();
                try
                {
                    e1.CopyBlock();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("CopyBlock expected an int; found System.Object", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.NewObject<object>();
                e1.LoadConstant(0);
                try
                {
                    e1.CopyBlock();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("CopyBlock expected a native int, System.Byte&, or System.Byte*; found System.Object", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.LoadConstant(0);
                e1.Convert<IntPtr>();
                e1.LoadConstant(0);
                try
                {
                    e1.CopyBlock();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("CopyBlock expected a native int, System.Byte&, or System.Byte*; found System.Object", e.Message);
                }
            }
        }

        [TestMethod]
        public void ConvertIllegal()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();
                try
                {
                    e1.ConvertOverflow<float>();
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("There is no operation for converting to a float with overflow checking", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                try
                {
                    e1.ConvertOverflow<double>();
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("There is no operation for converting to a double with overflow checking", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                try
                {
                    e1.UnsignedConvertOverflow<float>();
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("There is no operation for converting to a float with overflow checking", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                try
                {
                    e1.UnsignedConvertOverflow<double>();
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("There is no operation for converting to a double with overflow checking", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();

                try
                {
                    e1.Convert<UIntPtr>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("Convert expected a by ref, double, float, int, long, native int, or pointer; found System.Object", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();

                try
                {
                    e1.ConvertOverflow<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("ConvertOverflow expected a by ref, double, float, int, long, native int, or pointer; found System.Object", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();

                try
                {
                    e1.UnsignedConvertOverflow<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("UnsignedConvertOverflow expected a by ref, double, float, int, long, native int, or pointer; found System.Object", e.Message);
                }
            }
        }

        [TestMethod]
        public void ConvertEmptyStack()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();
                try
                {
                    e1.Convert<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("Convert expects a value on the stack, but it was empty", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                try
                {
                    e1.ConvertOverflow<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("ConvertOverflow expects a value on the stack, but it was empty", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                try
                {
                    e1.UnsignedConvertOverflow<int>();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("UnsignedConvertOverflow expects a value on the stack, but it was empty", e.Message);
                }
            }
        }

        [TestMethod]
        public void ConvertNonPrimitives()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();
                try
                {
                    e1.Convert(typeof(object));
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("Convert expects a non-character primitive type", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                try
                {
                    e1.ConvertOverflow<object>();
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("ConvertOverflow expects a non-character primitive type", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                try
                {
                    e1.UnsignedConvertOverflow<object>();
                    Assert.Fail();
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("UnsignedConvertOverflow expects a non-character primitive type", e.Message);
                }
            }
        }

        [TestMethod]
        public void ConvertNulls()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();
                try
                {
                    e1.Convert(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("primitiveType", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                try
                {
                    e1.ConvertOverflow(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("primitiveType", e.ParamName);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                try
                {
                    e1.UnsignedConvertOverflow(null);
                    Assert.Fail();
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("primitiveType", e.ParamName);
                }
            }
        }

        [TestMethod]
        public void ChecksStacks()
        {
            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.CompareEqual();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("CompareEqual expects 2 values on the stack", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.CompareGreaterThan();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("CompareGreaterThan expects 2 values on the stack", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.CompareLessThan();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("CompareLessThan expects 2 values on the stack", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.UnsignedCompareGreaterThan();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("UnsignedCompareGreaterThan expects 2 values on the stack", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.UnsignedCompareLessThan();
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("UnsignedCompareLessThan expects 2 values on the stack", e.Message);
                }
            }
        }

        [TestMethod]
        public void CheckFiniteStack()
        {
            var e1 = Emit<Action>.NewDynamicMethod();

            try
            {
                e1.CheckFinite();
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("CheckFinite expects a value on the stack, but it was empty", e.Message);
            }

            var e2 = Emit<Action>.NewDynamicMethod();
            e2.NewObject<object>();

            try
            {
                e2.CheckFinite();
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("CheckFinite expected a double, or float; found System.Object", e.Message);
            }
        }

        [TestMethod]
        public void CastClassNull()
        {
            var e1 = Emit<Action>.NewDynamicMethod();

            try
            {
                e1.CastClass(null);
                Assert.Fail();
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("referenceType", e.ParamName);
            }
        }

        [TestMethod]
        public void CastClassValueType()
        {
            var e1 = Emit<Action>.NewDynamicMethod();

            try
            {
                e1.CastClass(typeof(int));
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("Can only cast to ReferenceTypes, found System.Int32", e.Message);
            }
        }

        [TestMethod]
        public void CastClassEmptyStack()
        {
            var e1 = Emit<Action>.NewDynamicMethod();

            try
            {
                e1.CastClass<string>();
                Assert.Fail();
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("CastClass expects a value on the stack, but it was empty", e.Message);
            }
        }

        [TestMethod]
        public void CallIndirectNull()
        {
            var e1 = Emit<Action>.NewDynamicMethod();

            try
            {
                e1.CallIndirect(CallingConventions.Any, null);
                Assert.Fail();
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("returnType", e.ParamName);
            }

            try
            {
                e1.CallIndirect(CallingConventions.Any, typeof(void), null);
                Assert.Fail();
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("parameterTypes", e.ParamName);
            }
        }

        [TestMethod]
        public void CallIndirectBadConvention()
        {
            var e1 = Emit<Action>.NewDynamicMethod();

            try
            {
                e1.CallIndirect((CallingConventions)254);
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("callConventions", e.ParamName);
            }
        }

        [TestMethod]
        public void CallIndirectEmptyStack()
        {
            var e1 = Emit<Action>.NewDynamicMethod();

            try
            {
                e1.CallIndirect(CallingConventions.Any);
                Assert.Fail();
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("CallIndirect expects a value on the stack, but it was empty", e.Message);
            }
        }

        [TestMethod]
        public void CallIndirectNoPtr()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            e1.NewObject<object>();

            try
            {
                e1.CallIndirect(CallingConventions.Any);
                Assert.Fail();
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("CallIndirect expected a native int; found System.Object", e.Message);
            }
        }

        [TestMethod]
        public void CallIndirectKnownBad()
        {
            var toString = typeof(object).GetMethod("ToString");
            var addInt = typeof(List<int>).GetMethod("Add");

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.LoadFunctionPointer(toString);

                try
                {
                    e1.CallIndirect(CallingConventions.VarArgs, typeof(string), Type.EmptyTypes, Type.EmptyTypes);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("CallIndirect expects method calling conventions to match, found Standard, HasThis on the stack", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.LoadFunctionPointer(addInt);

                try
                {
                    e1.CallIndirect(addInt.CallingConvention);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("CallIndirect expects a 'this' value assignable to System.Collections.Generic.List`1[System.Int32], found System.Object", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.LoadFunctionPointer(toString);

                try
                {
                    e1.CallIndirect(toString.CallingConvention, typeof(object));
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("CallIndirect expects method return types to match, found System.String on the stack", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<List<int>>();
                e1.LoadConstant(2.0);
                e1.LoadFunctionPointer(addInt);

                try
                {
                    e1.CallIndirect(addInt.CallingConvention, typeof(void), typeof(int));
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("CallIndirect expected an int; found double", e.Message);
                }
            }
        }

        [TestMethod]
        public void CallBadParam()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            e1.NewObject<object>();

            try
            {
                e1.Call(typeof(Errors).GetMethod("CallBadParam"));
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Call expected a SigilTests.Errors; found System.Object", e.Message);
            }
        }

        [TestMethod]
        public void CallVirtualBadParam()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            e1.NewObject<object>();

            try
            {
                e1.CallVirtual(typeof(Errors).GetMethod("CallVirtualBadParam"));
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("CallVirtual expected a SigilTests.Errors; found System.Object", e.Message);
            }
        }

        [TestMethod]
        public void NullCallMethod()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            try
            {
                e1.Call((MethodInfo)null);
                Assert.Fail();
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("method", e.ParamName);
            }
        }


        [TestMethod]
        public void NullCallConstructor()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            try
            {
                e1.Call((ConstructorInfo)null);
                Assert.Fail();
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("cons", e.ParamName);
            }
        }

        [TestMethod]
        public void CallEmptyStack()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            try
            {
                e1.Call(typeof(object).GetMethod("ToString"));
                Assert.Fail();
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Call expects a value on the stack, but it was empty", e.Message);
            }
        }

        [TestMethod]
        public void NullCallVirtualMethod()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            try
            {
                e1.CallVirtual(null);
                Assert.Fail();
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("method", e.ParamName);
            }
        }

        [TestMethod]
        public void CallVirtualEmptyStack()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            try
            {
                e1.CallVirtual(typeof(object).GetMethod("ToString"));
                Assert.Fail();
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("CallVirtual expects a value on the stack, but it was empty", e.Message);
            }
        }

        [TestMethod]
        public void CallVirtualStatic()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            try
            {
                e1.CallVirtual(typeof(string).GetMethod("Intern"));
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("Only non-static methods can be called using CallVirtual, found System.String Intern(System.String)", e.Message);
            }
        }

        [TestMethod]
        public void NullBranchLabels()
        {
            var emit = typeof(Emit<Action>);
            var branches =
                new[]
                {
                    emit.GetMethod("Branch", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("BranchIfEqual", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("BranchIfFalse", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("BranchIfGreater", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("BranchIfGreaterOrEqual", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("BranchIfLess", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("BranchIfLessOrEqual", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("BranchIfTrue", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("UnsignedBranchIfNotEqual", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("UnsignedBranchIfGreater", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("UnsignedBranchIfGreaterOrEqual", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("UnsignedBranchIfLess", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("UnsignedBranchIfLessOrEqual", new [] { typeof(Sigil.Label) })
                };

            foreach (var branch in branches)
            {
                var e1 = Emit<Action>.NewDynamicMethod("E1");
                try
                {
                    branch.Invoke(e1, new object[] { null });
                    Assert.Fail();
                }
                catch (TargetInvocationException e)
                {
                    var f = (ArgumentNullException)e.InnerException;
                    Assert.AreEqual("label", f.ParamName);
                }
            }
        }

        [TestMethod]
        public void UnownedBranchLabels()
        {
            var emit = typeof(Emit<Action>);
            var branches =
                new[]
                {
                    emit.GetMethod("Branch", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("BranchIfEqual", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("BranchIfFalse", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("BranchIfGreater", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("BranchIfGreaterOrEqual", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("BranchIfLess", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("BranchIfLessOrEqual", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("BranchIfTrue", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("UnsignedBranchIfNotEqual", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("UnsignedBranchIfGreater", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("UnsignedBranchIfGreaterOrEqual", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("UnsignedBranchIfLess", new [] { typeof(Sigil.Label) }),
                    emit.GetMethod("UnsignedBranchIfLessOrEqual", new [] { typeof(Sigil.Label) })
                };

            foreach (var branch in branches)
            {
                var e1 = Emit<Action>.NewDynamicMethod("E1");
                var e2 = Emit<Action>.NewDynamicMethod("E2");
                var l = e2.DefineLabel("wrong_emit");
                try
                {
                    branch.Invoke(e1, new object[] { l });
                    Assert.Fail();
                }
                catch (TargetInvocationException e)
                {
                    var f = (ArgumentException)e.InnerException;
                    Assert.AreEqual("wrong_emit is not owned by this Emit, and thus cannot be used", f.Message);
                }
            }
        }

        [TestMethod]
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
                try
                {
                    branch.Invoke(e1, new object[] { l });
                    Assert.Fail();
                }
                catch (TargetInvocationException e)
                {
                    var f = (SigilVerificationException)e.InnerException;
                    Assert.IsTrue(f.Message.EndsWith(" expects 2 values on the stack"));
                }
            }

            {
                var e2 = Emit<Action>.NewDynamicMethod("E2");
                var l = e2.DefineLabel();

                try
                {
                    e2.BranchIfFalse(l);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("BranchIfFalse expects a value on the stack, but it was empty", e.Message);
                }
            }

            {
                var e3 = Emit<Action>.NewDynamicMethod("E3");
                var l = e3.DefineLabel();

                try
                {
                    e3.BranchIfTrue(l);
                    Assert.Fail();
                }
                catch (SigilVerificationException e)
                {
                    Assert.AreEqual("BranchIfTrue expects a value on the stack, but it was empty", e.Message);
                }
            }
        }

        [TestMethod]
        public void CatchInCatch()
        {
            var e1 = Emit<Action>.NewDynamicMethod("e1");
            var t = e1.BeginExceptionBlock();
            var c1 = e1.BeginCatchBlock<StackOverflowException>(t);

            try
            {
                var c2 = e1.BeginCatchBlock<Exception>(t);
                Assert.Fail("Shouldn't be legal to have two catches open at the same time");
            }
            catch (InvalidOperationException s)
            {
                Assert.IsTrue(s.Message.StartsWith("Cannot start a new catch block, "));
            }
        }

        [TestMethod]
        public void NullTryCatch()
        {
            var e1 = Emit<Action>.NewDynamicMethod("e1");

            try
            {
                e1.BeginCatchAllBlock(null);
                Assert.Fail("Shouldn't be possible");
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("forTry", e.ParamName);
            }
        }

        [TestMethod]
        public void CatchNonException()
        {
            var e1 = Emit<Action>.NewDynamicMethod("e1");
            var t = e1.BeginExceptionBlock();

            try
            {
                e1.BeginCatchBlock<string>(t);
                Assert.Fail("Shouldn't be possible");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("exceptionType", e.ParamName);
            }
        }

        [TestMethod]
        public void NonEmptyExceptBlock()
        {
            var e1 = Emit<Action>.NewDynamicMethod("e1");
            var t = e1.BeginExceptionBlock();
            e1.LoadConstant("foo");

            try
            {
                var c = e1.BeginCatchAllBlock(t);
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("BeginCatchBlock expected the stack of be empty", e.Message);
            }
        }

        [TestMethod]
        public void CatchAlreadyClosedTry()
        {
            var e1 = Emit<Action>.NewDynamicMethod("e1");
            var t = e1.BeginExceptionBlock();
            var c1 = e1.BeginCatchAllBlock(t);
            e1.Pop();
            e1.EndCatchBlock(c1);
            e1.EndExceptionBlock(t);

            try
            {
                var c2 = e1.BeginCatchAllBlock(t);
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.IsTrue(e.Message.StartsWith("BeginCatchBlock expects an unclosed exception block, "));
            }
        }

        [TestMethod]
        public void CatchExceptionNull()
        {
            var e1 = Emit<Action>.NewDynamicMethod("e1");
            var t = e1.BeginExceptionBlock();

            try
            {
                var c1 = e1.BeginCatchBlock(t, null);
                Assert.Fail("Shouldn't be possible");
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("exceptionType", e.ParamName);
            }
        }

        [TestMethod]
        public void CatchOtherTry()
        {
            var e1 = Emit<Action>.NewDynamicMethod("e1");
            var t1 = e1.BeginExceptionBlock();
            var t2 = e1.BeginExceptionBlock();

            try
            {
                var c1 = e1.BeginCatchAllBlock(t1);
                Assert.Fail("Shouldn't be possible");
            }
            catch (InvalidOperationException e)
            {
                Assert.IsTrue(e.Message.StartsWith("Cannot start CatchBlock on "));
            }
        }

        [TestMethod]
        public void MixedOwners()
        {
            var e1 = Emit<Action>.NewDynamicMethod("e1");
            var t1 = e1.BeginExceptionBlock();

            var e2 = Emit<Action>.NewDynamicMethod("e2");
            var t2 = e2.BeginExceptionBlock();

            try
            {
                e1.BeginCatchAllBlock(t2);
                Assert.Fail("Shouldn't be possible");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e.Message.EndsWith(" is not owned by this Emit, and thus cannot be used"));
            }
        }

        [TestMethod]
        public void NonEmptyTry()
        {
            var e1 = Emit<Action>.NewDynamicMethod("e1");
            e1.LoadConstant(123);

            try
            {
                e1.BeginExceptionBlock();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException s)
            {
                Assert.AreEqual("BeginExceptionBlock expected the stack of be empty", s.Message);
            }
        }

        [TestMethod]
        public void ShiftEmptyStack()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            try
            {
                e1.ShiftLeft();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("ShiftLeft expects 2 values on the stack", e.Message);
            }
        }

        [TestMethod]
        public void ShiftBadValues()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");
            e1.LoadConstant("123");
            e1.LoadConstant(4);

            try
            {
                e1.ShiftLeft();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("ShiftLeft expected an int, long, or native int; found System.String", e.Message);
            }

            var e2 = Emit<Action>.NewDynamicMethod("E2");
            e2.LoadConstant(123);
            e2.LoadConstant("4");

            try
            {
                e2.ShiftLeft();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("ShiftLeft expected an int, or native int; found System.String", e.Message);
            }
        }

        [TestMethod]
        public void Add()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");
            e1.LoadConstant("123");
            e1.LoadConstant(4);

            try
            {
                e1.Add();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Add expected an int, or native int; found System.String", e.Message);
            }

            var e2 = Emit<Action>.NewDynamicMethod("E2");
            e2.LoadConstant(123);
            e2.LoadConstant("4");

            try
            {
                e2.Add();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Add expected a by ref, double, float, int, long, native int, or pointer; found System.String", e.Message);
            }

            var e3 = Emit<Action>.NewDynamicMethod("E3");
            e3.LoadConstant(123L);
            e3.LoadConstant("4");

            try
            {
                e3.Add();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Add expected a by ref, double, float, int, long, native int, or pointer; found System.String", e.Message);
            }

            var e4 = Emit<Action>.NewDynamicMethod("E4");
            e4.LoadConstant(123);
            e4.Convert<IntPtr>();
            e4.LoadConstant("4");

            try
            {
                e4.Add();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Add expected a by ref, double, float, int, long, native int, or pointer; found System.String", e.Message);
            }

            var e5 = Emit<Action>.NewDynamicMethod("E5");
            e5.LoadConstant(123f);
            e5.LoadConstant("4");

            try
            {
                e5.Add();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Add expected a by ref, double, float, int, long, native int, or pointer; found System.String", e.Message);
            }

            var e6 = Emit<Action>.NewDynamicMethod("E6");
            e6.LoadConstant(123.0);
            e6.LoadConstant("4");

            try
            {
                e6.Add();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Add expected a by ref, double, float, int, long, native int, or pointer; found System.String", e.Message);
            }

            var e7 = Emit<Action<int>>.NewDynamicMethod("E7");
            e7.LoadArgumentAddress(0);
            e7.LoadConstant("4");

            try
            {
                e7.Add();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Add expected a by ref, double, float, int, long, native int, or pointer; found System.String", e.Message);
            }

            var e8 = Emit<Action<int>>.NewDynamicMethod("E8");

            try
            {
                e8.Add();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Add expects 2 values on the stack", e.Message);
            }
        }

        [TestMethod]
        public void Multiply()
        {
            var e3 = Emit<Action<int>>.NewDynamicMethod("E3");

            try
            {
                e3.Multiply();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Multiply expects 2 values on the stack", e.Message);
            }
        }

        [TestMethod]
        public void AddOverflow()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            try
            {
                e1.AddOverflow();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("AddOverflow expects 2 values on the stack", e.Message);
            }
        }

        [TestMethod]
        public void UnsignedAddOverflow()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            try
            {
                e1.UnsignedAddOverflow();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("UnsignedAddOverflow expects 2 values on the stack", e.Message);
            }
        }

        [TestMethod]
        public void MultiplyOverflow()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            try
            {
                e1.MultiplyOverflow();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("MultiplyOverflow expects 2 values on the stack", e.Message);
            }
        }

        [TestMethod]
        public void UnsignedMultiplyOverflow()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            try
            {
                e1.UnsignedMultiplyOverflow();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("UnsignedMultiplyOverflow expects 2 values on the stack", e.Message);
            }
        }

        [TestMethod]
        public void Divide()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            try
            {
                e1.Divide();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Divide expects 2 values on the stack", e.Message);
            }
        }

        [TestMethod]
        public void UnsignedDivide()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            try
            {
                e1.UnsignedDivide();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("UnsignedDivide expects 2 values on the stack", e.Message);
            }
        }

        [TestMethod]
        public void Remainder()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            try
            {
                e1.Remainder();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Remainder expects 2 values on the stack", e.Message);
            }
        }

        [TestMethod]
        public void UnsignedRemainder()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            try
            {
                e1.UnsignedRemainder();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("UnsignedRemainder expects 2 values on the stack", e.Message);
            }
        }

        [TestMethod]
        public void Subtract()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            try
            {
                e1.Subtract();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Subtract expects 2 values on the stack", e.Message);
            }
        }

        [TestMethod]
        public void SubtractOverflow()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            try
            {
                e1.SubtractOverflow();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("SubtractOverflow expects 2 values on the stack", e.Message);
            }
        }

        [TestMethod]
        public void UnsignedSubtractOverflow()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            try
            {
                e1.UnsignedSubtractOverflow();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("UnsignedSubtractOverflow expects 2 values on the stack", e.Message);
            }
        }

        [TestMethod]
        public void Negate()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            try
            {
                e1.Negate();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Negate expects a value on the stack, but it was empty", e.Message);
            }

            var e2 = Emit<Action>.NewDynamicMethod("E2");
            e2.LoadConstant("Hello");

            try
            {
                e2.Negate();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Negate expected a double, float, int, long, or native int; found System.String", e.Message);
            }
        }

        [TestMethod]
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

            try
            {
                var d1 = e1.CreateDelegate();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Cannot branch from inside Sigil.ExceptionBlock to outside, exit the ExceptionBlock first", e.Message);
            }
        }

        [TestMethod]
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

            try
            {
                var d1 = e1.CreateDelegate();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Cannot branch from inside Sigil.CatchBlock to outside, exit the ExceptionBlock first", e.Message);
            }
        }

        [TestMethod]
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

            try
            {
                var d1 = e1.CreateDelegate();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Cannot branch from inside Sigil.FinallyBlock to outside, exit the ExceptionBlock first", e.Message);
            }
        }

        [TestMethod]
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

            try
            {
                var d1 = e1.CreateDelegate();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Cannot branch into a FinallyBlock", e.Message);
            }
        }

        [TestMethod]
        public void BeginFinallyBlock()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            try
            {
                e1.BeginFinallyBlock(null);
                Assert.Fail("Shouldn't be possible");
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("forTry", e.ParamName);
            }

            var e2 = Emit<Action>.NewDynamicMethod("E2");
            var e3 = Emit<Action>.NewDynamicMethod("E3");
            var t2 = e2.BeginExceptionBlock();

            try
            {
                e3.BeginFinallyBlock(t2);
                Assert.Fail("Shouldn't be possible");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("Sigil.ExceptionBlock is not owned by this Emit, and thus cannot be used", e.Message);
            }

            var e4 = Emit<Action>.NewDynamicMethod("E4");
            var t4 = e4.BeginExceptionBlock();
            var c4 = e4.BeginCatchAllBlock(t4);
            e4.Pop();
            e4.EndCatchBlock(c4);
            e4.EndExceptionBlock(t4);

            try
            {
                e4.BeginFinallyBlock(t4);
                Assert.Fail("Shouldn't be possible");
            }
            catch (InvalidOperationException e)
            {
                Assert.AreEqual("BeginFinallyBlock expects an unclosed exception block, but Sigil.ExceptionBlock is already closed", e.Message);
            }

            var e5 = Emit<Action>.NewDynamicMethod("E5");
            var t5 = e5.BeginExceptionBlock();
            var t55 = e5.BeginExceptionBlock();

            try
            {
                var f5 = e5.BeginFinallyBlock(t5);
                Assert.Fail("Shouldn't be possible");
            }
            catch (InvalidOperationException e)
            {
                Assert.AreEqual("Cannot begin FinallyBlock on Sigil.ExceptionBlock while inner ExceptionBlock Sigil.ExceptionBlock is still open", e.Message);
            }

            var e6 = Emit<Action>.NewDynamicMethod("E6");
            var t6 = e6.BeginExceptionBlock();
            e6.LoadConstant(123);

            try
            {
                e6.BeginFinallyBlock(t6);
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("BeginFinallyBlock expected the stack of be empty", e.Message);
            }

            var e7 = Emit<Action>.NewDynamicMethod("E7");
            var t7 = e7.BeginExceptionBlock();
            var f7 = e7.BeginFinallyBlock(t7);
            e7.EndFinallyBlock(f7);

            try
            {
                e7.BeginFinallyBlock(t7);
                Assert.Fail("Shouldn't be possible");
            }
            catch (InvalidOperationException e)
            {
                Assert.AreEqual("There can only be one finally block per ExceptionBlock, and one is already defined for Sigil.ExceptionBlock", e.Message);
            }
        }

        [TestMethod]
        public void Box()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            try
            {
                e1.Box(null);
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("valueType", e.ParamName);
            }

            var e2 = Emit<Action>.NewDynamicMethod("E2");

            try
            {
                e2.Box(typeof(string));
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("valueType", e.ParamName);
            }

            var e3 = Emit<Action>.NewDynamicMethod("E3");

            try
            {
                e3.Box<int>();
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Box expects a value on the stack, but it was empty", e.Message);
            }

            var e4 = Emit<Action>.NewDynamicMethod("E4");
            e4.LoadConstant("hello world");

            try
            {
                e4.Box<byte>();
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Box expected a System.Byte; found System.String", e.Message);
            }

            var e5 = Emit<Action>.NewDynamicMethod("E5");
            e5.LoadConstant(1234);

            try
            {
                e5.Box<Guid>();
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Box expected a System.Guid; found int", e.Message);
            }
        }

        [TestMethod]
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
            

            try
            {
                e1.Return();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Return expected the stack of be empty", e.Message);
            }
        }

        [TestMethod]
        public void StackCheck()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");
            e1.LoadConstant(1);
            e1.LoadConstant("123");

            try
            {
                e1.Add();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Add expected a by ref, double, float, int, long, native int, or pointer; found System.String", e.Message);
            }
        }

        [TestMethod]
        public void MaxStackNonVerifying()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1", doVerify: false);

            try
            {
                Assert.AreEqual(0, e1.MaxStackSize);
                Assert.Fail("Shouldn't be possible");
            }
            catch (InvalidOperationException e)
            {
                Assert.AreEqual("MaxStackSize is not available on non-verifying Emits", e.Message);
            }
        }
    }
}
