using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    /// <summary>
    /// This class is a little tricky.
    /// 
    /// The actual CLR's behavior differs a tad from the ECMA standard.
    /// So, what happens here is we first confirm that something is unverifiable (using DynamicMethod without a specified module)
    /// and then confirm that Sigil won't allow that instruction (or instruction sequence) when in a "verified instructions only" mode.
    /// And then that the same sequence actually works in "unverified" mode.
    /// </summary>
    [TestClass, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Verifiability
    {
        [TestMethod]
        public void Ldelema()
        {
            try
            {
                var dyn = new DynamicMethod("E1", typeof(void), new[] { typeof(int[]) });
                var il = dyn.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Ldelema, typeof(int));
                il.Emit(OpCodes.Ldind_I4);
                il.Emit(OpCodes.Ret);

                var d1 = (Action<int[]>)dyn.CreateDelegate(typeof(Action<int[]>));

                d1(new int[] { 123 });
                Assert.Fail();
            }
            catch (VerificationException) { }

            {
                var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                var mod = asm.DefineDynamicModule("Bar");
                var t = mod.DefineType("T");

                var e1 = Emit<Action<int[]>>.BuildMethod(t, "E1", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard);
                e1.LoadArgument(0).LoadConstant(0);

                try
                {
                    e1.LoadElementAddress<int>();
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("LoadElementAddress isn't verifiable", e.Message);
                }
            }

            {
                var e1 = Emit<Action<int[]>>.NewDynamicMethod("E1");
                var d1 = e1
                    .LoadArgument(0)
                    .LoadConstant(0)
                    .LoadElementAddress<int>()
                    .LoadIndirect<int>()
                    .Pop()
                    .Return()
                    .CreateDelegate();

                d1(new int[] { 123 });
            }
        }

        [TestMethod]
        public void Calli()
        {
            var writeLine = typeof(Console).GetMethod("WriteLine", Type.EmptyTypes);

            try
            {
                var dyn = new DynamicMethod("E1", typeof(void), Type.EmptyTypes);
                var il = dyn.GetILGenerator();
                il.Emit(OpCodes.Ldftn, writeLine);
                il.EmitCalli(OpCodes.Calli, System.Runtime.InteropServices.CallingConvention.StdCall, typeof(void), Type.EmptyTypes);
                il.Emit(OpCodes.Ret);

                var d1 = (Action)dyn.CreateDelegate(typeof(Action));

                d1();
                Assert.Fail();
            }
            catch (VerificationException) { }

            {
                var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                var mod = asm.DefineDynamicModule("Bar");
                var t = mod.DefineType("T");

                var e1 = Emit<Action>.BuildMethod(t, "E1", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard);
                e1.LoadFunctionPointer(writeLine);
                
                try
                {
                    e1.CallIndirect(CallingConventions.Standard);
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("CallIndirect isn't verifiable", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod("E1");
                var d1 =
                    e1.LoadFunctionPointer(writeLine)
                    .CallIndirect(CallingConventions.Standard)
                    .Return()
                    .CreateDelegate();

                d1();
            }
        }

        [TestMethod]
        public void Localloc()
        {
            try
            {
                var dyn = new DynamicMethod("E1", typeof(void), Type.EmptyTypes);
                var il = dyn.GetILGenerator();
                il.Emit(OpCodes.Ldc_I4_8);
                il.Emit(OpCodes.Localloc);
                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Ret);

                var d1 = (Action)dyn.CreateDelegate(typeof(Action));
                d1();

                Assert.Fail();
            }
            catch (VerificationException) { }

            {
                var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                var mod = asm.DefineDynamicModule("Bar");
                var t = mod.DefineType("T");

                var e1 = Emit<Action>.BuildMethod(t, "E1", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard);
                e1.LoadConstant(8);

                try
                {
                    e1.LocalAllocate();
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("LocalAllocate isn't verifiable", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod("E1");
                var d1 =
                    e1.LoadConstant(8)
                    .LocalAllocate()
                    .Pop()
                    .Return()
                    .CreateDelegate();

                d1();
            }
        }

        [TestMethod]
        public void Cpblk()
        {
            var src = Marshal.AllocHGlobal(8);
            var dest = Marshal.AllocHGlobal(8);

            try
            {
                try
                {
                    var dyn = new DynamicMethod("E1", typeof(void), new[] { typeof(IntPtr), typeof(IntPtr) });
                    var il = dyn.GetILGenerator();
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldc_I4_8);
                    il.Emit(OpCodes.Cpblk);
                    il.Emit(OpCodes.Ret);

                    var d1 = (Action<IntPtr, IntPtr>)dyn.CreateDelegate(typeof(Action<IntPtr, IntPtr>));
                    d1(dest, src);

                    Assert.Fail();
                }
                catch (VerificationException) { }

                {
                    var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                    var mod = asm.DefineDynamicModule("Bar");
                    var t = mod.DefineType("T");

                    var e1 = Emit<Action<IntPtr, IntPtr>>.BuildMethod(t, "E1", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard);
                    e1.LoadArgument(0);
                    e1.LoadArgument(1);
                    e1.LoadConstant(8);

                    try
                    {
                        e1.CopyBlock();
                        Assert.Fail();
                    }
                    catch (InvalidOperationException e)
                    {
                        Assert.AreEqual("CopyBlock isn't verifiable", e.Message);
                    }
                }

                {
                    var e1 = Emit<Action<IntPtr, IntPtr>>.NewDynamicMethod("E1");
                    var d1 =
                        e1.LoadArgument(0)
                        .LoadArgument(1)
                        .LoadConstant(8)
                        .CopyBlock()
                        .Return()
                        .CreateDelegate();

                    d1(dest, src);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(src);
                Marshal.FreeHGlobal(dest);
            }
        }

        [TestMethod]
        public void Initblk()
        {
            var blk = Marshal.AllocHGlobal(8);

            try
            {
                try
                {
                    var dyn = new DynamicMethod("E1", typeof(void), new[] { typeof(IntPtr) });
                    var il = dyn.GetILGenerator();
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldc_I4_1);
                    il.Emit(OpCodes.Ldc_I4_8);
                    il.Emit(OpCodes.Initblk);
                    il.Emit(OpCodes.Ret);

                    var d1 = (Action<IntPtr>)dyn.CreateDelegate(typeof(Action<IntPtr>));
                    d1(blk);

                    Assert.Fail();
                }
                catch (VerificationException) { }

                {
                    var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                    var mod = asm.DefineDynamicModule("Bar");
                    var t = mod.DefineType("T");

                    var e1 = Emit<Action<IntPtr>>.BuildMethod(t, "E1", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard);
                    e1.LoadArgument(0);
                    e1.LoadConstant(1);
                    e1.LoadConstant(8);

                    try
                    {
                        e1.InitializeBlock();
                        Assert.Fail();
                    }
                    catch (InvalidOperationException e)
                    {
                        Assert.AreEqual("InitializeBlock isn't verifiable", e.Message);
                    }
                }

                {
                    var e1 = Emit<Action<IntPtr>>.NewDynamicMethod("E1");
                    var d1 =
                        e1.LoadArgument(0)
                        .LoadConstant(1)
                        .LoadConstant(8)
                        .InitializeBlock()
                        .Return()
                        .CreateDelegate();

                    d1(blk);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(blk);
            }
        }

        [TestMethod]
        public void Jmp()
        {
            var writeLine = typeof(Console).GetMethod("WriteLine", Type.EmptyTypes);

            try
            {
                var dyn = new DynamicMethod("E1", typeof(void), Type.EmptyTypes);
                var il = dyn.GetILGenerator();
                il.Emit(OpCodes.Jmp, writeLine);
                il.Emit(OpCodes.Ret);

                var d1 = (Action)dyn.CreateDelegate(typeof(Action));
                d1();

                Assert.Fail();
            }
            catch (VerificationException) { }

            {
                var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                var mod = asm.DefineDynamicModule("Bar");
                var t = mod.DefineType("T");

                var e1 = Emit<Action>.BuildMethod(t, "E1", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard);

                try
                {
                    e1.Jump(writeLine);
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("Jump isn't verifiable", e.Message);
                }
            }

            {
                var e1 = Emit<Action>.NewDynamicMethod("E1");
                var d1 =
                    e1.Jump(writeLine)
                    .Return()
                    .CreateDelegate();

                d1();
            }
        }

        public class LdfldaClass
        {
            public readonly int Field;
        }

        [TestMethod]
        public void LdfldaInitOnly()
        {
            var fld = typeof(LdfldaClass).GetField("Field");

            try
            {
                var dyn = new DynamicMethod("E1", typeof(void), new[] { typeof(LdfldaClass) });
                var il = dyn.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldflda, fld);
                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Ret);

                var d1 = (Action<LdfldaClass>)dyn.CreateDelegate(typeof(Action<LdfldaClass>));
                d1(new LdfldaClass());

                Assert.Fail();
            }
            catch (VerificationException) { }

            {
                var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                var mod = asm.DefineDynamicModule("Bar");
                var t = mod.DefineType("T");

                var e1 = Emit<Action<LdfldaClass>>.BuildMethod(t, "E1", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard);
                e1.LoadArgument(0);

                try
                {

                    e1.LoadFieldAddress(fld);
                    Assert.Fail();
                }
                catch (InvalidOperationException e)
                {
                    Assert.AreEqual("LoadFieldAddress on InitOnly fields is not verifiable", e.Message);
                }
            }

            {
                var e1 = Emit<Action<LdfldaClass>>.NewDynamicMethod("E1");
                var d1 =
                    e1.LoadArgument(0)
                    .LoadFieldAddress(fld)
                    .Pop()
                    .Return()
                    .CreateDelegate();

                d1(new LdfldaClass());
            }
        }
    }
}
