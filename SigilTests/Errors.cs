using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestClass, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Errors
    {
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
                    Assert.AreEqual("Delegate must end with Return", e.Message);
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
                    Assert.AreEqual("LocalAllocateexpected the stack to have 1 value", e.Message);
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
                    Assert.AreEqual("LocalAllocateexpected the stack to have 1 value", e.Message);
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
                try
                {
                    e1.UnsignedConvertOverflow<IntPtr>();
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("There is no operation for converting to a pointer with overflow checking", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                try
                {
                    e1.UnsignedConvertOverflow<UIntPtr>();
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("There is no operation for converting to a pointer with overflow checking", e.Message);
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
                    Assert.AreEqual("Convert expected a double, float, int, long, native int, or pointer; found System.Object", e.Message);
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
                    Assert.AreEqual("ConvertOverflow expected a double, float, int, long, native int, or pointer; found System.Object", e.Message);
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
                    Assert.AreEqual("UnsignedConvertOverflow expected a double, float, int, long, native int, or pointer; found System.Object", e.Message);
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
                    e1.CallIndirect(CallingConventions.VarArgs);
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
                e1.Call(null);
                Assert.Fail();
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("method", e.ParamName);
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
                Assert.AreEqual("Add expected a double, float, int, long, or native int; found System.String", e.Message);
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
                Assert.AreEqual("Add expected a double, float, int, long, or native int; found System.String", e.Message);
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
                Assert.AreEqual("Add expected a double, float, int, long, or native int; found System.String", e.Message);
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
                Assert.AreEqual("Add expected a double, float, int, long, or native int; found System.String", e.Message);
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
                Assert.AreEqual("Add expected a double, float, int, long, or native int; found System.String", e.Message);
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
                Assert.AreEqual("Add expected a double, float, int, long, or native int; found System.String", e.Message);
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
                Assert.AreEqual("Add expected a double, float, int, long, or native int; found System.String", e.Message);
            }
        }
    }
}
