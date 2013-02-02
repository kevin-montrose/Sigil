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
    [TestClass]
    public class Errors
    {
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
                catch (SigilException e)
                {
                    Assert.AreEqual("CopyObject expects two values to be on the stack", e.Message);
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
                catch (SigilException e)
                {
                    Assert.AreEqual("CopyObject expects the source value to be a pointer, reference, or native int; found System.Object", e.Message);
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
                catch (SigilException e)
                {
                    Assert.AreEqual("CopyObject expects the destination value to be a pointer, reference, or native int; found System.Object", e.Message);
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
                catch (SigilException e)
                {
                    Assert.AreEqual("CopyBlock expects three values to be on the stack", e.Message);
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
                catch (SigilException e)
                {
                    Assert.AreEqual("CopyBlock expects the destination value to be a pointer, reference, or native int; found System.Object", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.LoadConstant(0);
                e1.Convert<IntPtr>();
                e1.NewObject<object>();
                e1.NewObject<object>();
                try
                {
                    e1.CopyBlock();
                    Assert.Fail();
                }
                catch (SigilException e)
                {
                    Assert.AreEqual("CopyBlock expects the source value to be a pointer, reference, or native int; found System.Object", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.LoadConstant(0);
                e1.Convert<IntPtr>();
                e1.Duplicate();
                e1.NewObject<object>();
                try
                {
                    e1.CopyBlock();
                    Assert.Fail();
                }
                catch (SigilException e)
                {
                    Assert.AreEqual("CopyBlock expects the count value to be an int; found System.Object", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                var l = e1.DeclareLocal<double>();
                e1.LoadConstant(0);
                e1.Convert<IntPtr>();
                e1.LoadLocalAddress(l);
                e1.LoadConstant(0);
                try
                {
                    e1.CopyBlock();
                    Assert.Fail();
                }
                catch (SigilException e)
                {
                    Assert.AreEqual("CopyBlock expects source and destination types to match; found System.Double* and native int", e.Message);
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
                catch (SigilException e)
                {
                    Assert.AreEqual("Convert expected a value on the stack, but it was empty", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                try
                {
                    e1.ConvertOverflow<int>();
                    Assert.Fail();
                }
                catch (SigilException e)
                {
                    Assert.AreEqual("ConvertOverflow expected a value on the stack, but it was empty", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                try
                {
                    e1.UnsignedConvertOverflow<int>();
                    Assert.Fail();
                }
                catch (SigilException e)
                {
                    Assert.AreEqual("UnsignedConvertOverflow expected a value on the stack, but it was empty", e.Message);
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
                    e1.Convert<object>();
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
                catch (SigilException e)
                {
                    Assert.AreEqual("CompareEqual expects two values on the stack", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.CompareGreaterThan();
                    Assert.Fail();
                }
                catch (SigilException e)
                {
                    Assert.AreEqual("CompareGreaterThan expects two values on the stack", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.CompareLessThan();
                    Assert.Fail();
                }
                catch (SigilException e)
                {
                    Assert.AreEqual("CompareLessThan expects two values on the stack", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.UnsignedCompareGreaterThan();
                    Assert.Fail();
                }
                catch (SigilException e)
                {
                    Assert.AreEqual("UnsignedCompareGreaterThan expects two values on the stack", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();

                try
                {
                    e1.UnsignedCompareLessThan();
                    Assert.Fail();
                }
                catch (SigilException e)
                {
                    Assert.AreEqual("UnsignedCompareLessThan expects two values on the stack", e.Message);
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
            catch (SigilException e)
            {
                Assert.AreEqual("CheckFinite expects a value to be on the stack, but it was empty", e.Message);
            }

            var e2= Emit<Action>.NewDynamicMethod();
            e2.NewObject<object>();

            try
            {
                e2.CheckFinite();
            }
            catch (SigilException e)
            {
                Assert.AreEqual("CheckFinite expects a floating point value, found System.Object", e.Message);
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
                e1.CastClass<int>();
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
            catch (SigilException e)
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
            catch (SigilException e)
            {
                Assert.AreEqual("CallIndirect expected 1 values on the stack", e.Message);
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
            catch (SigilException e)
            {
                Assert.AreEqual("CallIndirect expects a native int to be on the top of the stack, found System.Object", e.Message);
            }
        }

        [TestMethod]
        public void CallIndirectKnownBad()
        {
            var toString = typeof(object).GetMethod("ToString");
            var add = typeof(List<>).GetMethod("Add");
            var addInt = typeof(List<int>).GetMethod("Add");

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.LoadFunctionPointer(toString);

                try
                {
                    e1.CallIndirect(CallingConventions.VarArgs);
                    Assert.Fail();
                }
                catch (SigilException e)
                {
                    Assert.AreEqual("CallIndirect expects method calling conventions to match, found Standard, HasThis on the stack", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod();
                e1.NewObject<object>();
                e1.LoadFunctionPointer(add);

                try
                {
                    e1.CallIndirect(add.CallingConvention);
                    Assert.Fail();
                }
                catch (SigilException e)
                {
                    Assert.AreEqual("CallIndirect expects a 'this' value assignable to System.Collections.Generic.List`1[T], found System.Object", e.Message);
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
                catch (SigilException e)
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
                catch (SigilException e)
                {
                    Assert.AreEqual("CallIndirect expected a value assignable to System.Int32, found System.Double", e.Message);
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
            catch (SigilException e)
            {
                Assert.AreEqual("Parameter #0 to Void CallBadParam() should be SigilTests.Errors, but found System.Object", e.Message);
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
            catch (SigilException e)
            {
                Assert.AreEqual("Parameter #0 to Void CallVirtualBadParam() should be SigilTests.Errors, but found System.Object", e.Message);
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
            catch (SigilException e)
            {
                Assert.AreEqual("Call to System.String ToString() expected parameters [System.Object] to be on the stack", e.Message);
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
            catch (SigilException e)
            {
                Assert.AreEqual("CallVirtual to System.String ToString() expected parameters [System.Object] to be on the stack", e.Message);
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
                    emit.GetMethod("Branch"),
                    emit.GetMethod("BranchIfEqual"),
                    emit.GetMethod("BranchIfFalse"),
                    emit.GetMethod("BranchIfGreater"),
                    emit.GetMethod("BranchIfGreaterOrEqual"),
                    emit.GetMethod("BranchIfLess"),
                    emit.GetMethod("BranchIfLessOrEqual"),
                    emit.GetMethod("BranchIfTrue"),
                    emit.GetMethod("UnsignedBranchIfNotEqual"),
                    emit.GetMethod("UnsignedBranchIfGreater"),
                    emit.GetMethod("UnsignedBranchIfGreaterOrEqual"),
                    emit.GetMethod("UnsignedBranchIfLess"),
                    emit.GetMethod("UnsignedBranchIfLessOrEqual")
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
                    emit.GetMethod("Branch"),
                    emit.GetMethod("BranchIfEqual"),
                    emit.GetMethod("BranchIfFalse"),
                    emit.GetMethod("BranchIfGreater"),
                    emit.GetMethod("BranchIfGreaterOrEqual"),
                    emit.GetMethod("BranchIfLess"),
                    emit.GetMethod("BranchIfLessOrEqual"),
                    emit.GetMethod("BranchIfTrue"),
                    emit.GetMethod("UnsignedBranchIfNotEqual"),
                    emit.GetMethod("UnsignedBranchIfGreater"),
                    emit.GetMethod("UnsignedBranchIfGreaterOrEqual"),
                    emit.GetMethod("UnsignedBranchIfLess"),
                    emit.GetMethod("UnsignedBranchIfLessOrEqual")
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
                    Assert.AreEqual("label is not owned by this Emit, and thus cannot be used", f.Message);
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
                try
                {
                    branch.Invoke(e1, new object[] { l });
                    Assert.Fail();
                }
                catch (TargetInvocationException e)
                {
                    var f = (SigilException)e.InnerException;
                    Assert.IsTrue(f.Message.EndsWith(" expects two values to be on the stack"));
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
                catch (SigilException e)
                {
                    Assert.AreEqual("BranchIfFalse expects one value to be on the stack", e.Message);
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
                catch (SigilException e)
                {
                    Assert.AreEqual("BranchIfTrue expects one value to be on the stack", e.Message);
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
            catch (SigilException e)
            {
                Assert.AreEqual("Stack should be empty when BeginCatchBlock is called", e.Message);
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
            catch (SigilException e)
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
            catch (SigilException s)
            {
                Assert.AreEqual("Stack should be empty when BeginExceptionBlock is called", s.Message);
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
            catch (SigilException e)
            {
                Assert.AreEqual("ShiftLeft expects two values on the stack", e.Message);
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
            catch (SigilException e)
            {
                Assert.AreEqual("ShiftLeft expects the value to be shifted to be an int, long, or native int; found System.String", e.Message);
            }

            var e2= Emit<Action>.NewDynamicMethod("E2");
            e2.LoadConstant(123);
            e2.LoadConstant("4");

            try
            {
                e2.ShiftLeft();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilException e)
            {
                Assert.AreEqual("ShiftLeft expects the shift to be an int or native int; found System.String", e.Message);
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
            catch (SigilException e)
            {
                Assert.AreEqual("Add expects an int32, int64, native int, float, reference, or pointer as first value; found System.String", e.Message);
            }

            var e2 = Emit<Action>.NewDynamicMethod("E2");
            e2.LoadConstant(123);
            e2.LoadConstant("4");

            try
            {
                e2.Add();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilException e)
            {
                Assert.AreEqual("Add with an int32 expects an int32, native int, reference, or pointer as a second value; found System.String", e.Message);
            }

            var e3 = Emit<Action>.NewDynamicMethod("E3");
            e3.LoadConstant(123L);
            e3.LoadConstant("4");

            try
            {
                e3.Add();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilException e)
            {
                Assert.AreEqual("Add with to an int64 expects an in64 as second value; found System.String", e.Message);
            }

            var e4 = Emit<Action>.NewDynamicMethod("E4");
            e4.LoadConstant(123);
            //e4.ConvertToNativeInt();
            e4.Convert<IntPtr>();
            e4.LoadConstant("4");

            try
            {
                e4.Add();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilException e)
            {
                Assert.AreEqual("Add with a native int expects an int32, native int, reference, or pointer as a second value; found System.String", e.Message);
            }

            var e5 = Emit<Action>.NewDynamicMethod("E5");
            e5.LoadConstant(123f);
            e5.LoadConstant("4");

            try
            {
                e5.Add();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilException e)
            {
                Assert.AreEqual("Add with a float expects a float as second value; found System.String", e.Message);
            }

            var e6 = Emit<Action>.NewDynamicMethod("E6");
            e6.LoadConstant(123.0);
            e6.LoadConstant("4");

            try
            {
                e6.Add();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilException e)
            {
                Assert.AreEqual("Add with a double expects a double as second value; found System.String", e.Message);
            }

            var e7 = Emit<Action<int>>.NewDynamicMethod("E7");
            e7.LoadArgumentAddress(0);
            e7.LoadConstant("4");

            try
            {
                e7.Add();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilException e)
            {
                Assert.AreEqual("Add with a reference or pointer expects an int32, or a native int as second value; found System.String", e.Message);
            }

            var e8 = Emit<Action<int>>.NewDynamicMethod("E8");

            try
            {
                e8.Add();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilException e)
            {
                Assert.AreEqual("Add requires 2 arguments be on the stack", e.Message);
            }
        }

        [TestMethod]
        public void Multiply()
        {
            var e1 = Emit<Action<int>>.NewDynamicMethod("E1");
            e1.LoadArgumentAddress(0);
            e1.LoadConstant(1);

            try
            {
                e1.Multiply();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilException e)
            {
                Assert.AreEqual("Multiply expects an int32, int64, native int, or float as a first value; found System.Int32*", e.Message);
            }

            var e2 = Emit<Action<int>>.NewDynamicMethod("E2");
            e2.LoadConstant(1);
            e2.LoadArgumentAddress(0);

            try
            {
                e2.Multiply();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilException e)
            {
                Assert.AreEqual("Multiply with an int32 expects an int32 or native int as a second value; found System.Int32*", e.Message);
            }

            var e3 = Emit<Action<int>>.NewDynamicMethod("E3");

            try
            {
                e3.Multiply();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilException e)
            {
                Assert.AreEqual("Multiply requires 2 arguments be on the stack", e.Message);
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
            catch (SigilException e)
            {
                Assert.AreEqual("AddOverflow requires 2 arguments be on the stack", e.Message);
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
            catch (SigilException e)
            {
                Assert.AreEqual("UnsignedAddOverflow requires 2 arguments be on the stack", e.Message);
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
            catch (SigilException e)
            {
                Assert.AreEqual("MultiplyOverflow requires 2 arguments be on the stack", e.Message);
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
            catch (SigilException e)
            {
                Assert.AreEqual("UnsignedMultiplyOverflow requires 2 arguments be on the stack", e.Message);
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
            catch (SigilException e)
            {
                Assert.AreEqual("Divide requires 2 arguments be on the stack", e.Message);
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
            catch (SigilException e)
            {
                Assert.AreEqual("UnsignedDivide requires 2 arguments be on the stack", e.Message);
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
            catch (SigilException e)
            {
                Assert.AreEqual("Remainder requires 2 arguments be on the stack", e.Message);
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
            catch (SigilException e)
            {
                Assert.AreEqual("UnsignedRemainder requires 2 arguments be on the stack", e.Message);
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
            catch (SigilException e)
            {
                Assert.AreEqual("Subtract requires 2 arguments be on the stack", e.Message);
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
            catch (SigilException e)
            {
                Assert.AreEqual("SubtractOverflow requires 2 arguments be on the stack", e.Message);
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
            catch (SigilException e)
            {
                Assert.AreEqual("UnsignedSubtractOverflow requires 2 arguments be on the stack", e.Message);
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
            catch (SigilException e)
            {
                Assert.AreEqual("Negate expected a value to be on the stack, but it was empty", e.Message);
            }

            var e2 = Emit<Action>.NewDynamicMethod("E2");
            e2.LoadConstant("Hello");

            try
            {
                e2.Negate();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilException e)
            {
                Assert.AreEqual("Negate expects an int, long, float, double, or native int; found System.String", e.Message);
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
            catch (SigilException e)
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
            catch (SigilException e)
            {
                Assert.AreEqual("Cannot branch from inside Sigil.CatchBlock to outside, exit the ExceptionBlock first", e.Message);
            }
        }

        [TestMethod]
        public void BranchOutOfFinally()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");
            var l = e1.DefineLabel("end");

            var t = e1.BeginExceptionBlock();
            var f = e1.BeginFinallyBlock(t);
            e1.Branch(l);
            e1.EndFinallyBlock(f);
            e1.EndExceptionBlock(t);

            e1.MarkLabel(l);
            e1.Return();

            try
            {
                var d1 = e1.CreateDelegate();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilException e)
            {
                Assert.AreEqual("Cannot branch from inside Sigil.FinallyBlock to outside, exit the ExceptionBlock first", e.Message);
            }
        }

        [TestMethod]
        public void BranchIntoFinally()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");
            var l = e1.DefineLabel("inFinally");

            e1.Branch(l);

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
            catch (SigilException e)
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
                Assert.AreEqual("forTry is not owned by this Emit, and thus cannot be used", e.Message);
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
            catch (SigilException e)
            {
                Assert.AreEqual("Stack should be empty when BeginFinallyBlock is called", e.Message);
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
                e2.Box<string>();
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
            catch (SigilException e)
            {
                Assert.AreEqual("Box expects a value on the stack, but found none", e.Message);
            }

            var e4 = Emit<Action>.NewDynamicMethod("E4");
            e4.LoadConstant("hello world");

            try
            {
                e4.Box<byte>();
            }
            catch (SigilException e)
            {
                Assert.AreEqual("System.String cannot be boxed as an System.Byte", e.Message);
            }

            var e5 = Emit<Action>.NewDynamicMethod("E5");
            e5.LoadConstant(1234);

            try
            {
                e5.Box<Guid>();
            }
            catch (SigilException e)
            {
                Assert.AreEqual("Expected System.Guid to be on the stack, found System.Int32", e.Message);
            }
        }

        [TestMethod]
        public void BadBranch()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");
            var l = e1.DefineLabel("l");

            e1.LoadConstant(1);
            e1.Branch(l);
            
            e1.Pop();
            e1.Return();

            e1.MarkLabel(l);
            e1.Return();

            try
            {
                var d1 = e1.CreateDelegate();
                Assert.Fail("Shouldn't be possible");
            }
            catch (SigilException e)
            {
                Assert.AreEqual("Branch to l has a stack that doesn't match the destination", e.Message);
                Assert.AreEqual("Top of stack at branch\r\n----------------------\r\nSystem.Int32\r\n\r\nTop of stack at label\r\n---------------------\r\n!!EMPTY!!\r\n", e.PrintStacks());
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
            catch (SigilException e)
            {
                Assert.AreEqual("Add with an int32 expects an int32, native int, reference, or pointer as a second value; found System.String", e.Message);
                Assert.AreEqual("Top of stack\r\n------------\r\nSystem.String\r\nSystem.Int32\r\n", e.PrintStacks());
            }
        }
    }
}
