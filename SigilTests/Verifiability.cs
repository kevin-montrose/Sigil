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
        class _Usage
        {
#pragma warning disable 0649
            public string Foo;
#pragma warning restore 0649
        }

        [TestMethod]
        public void Usage()
        {
            var e1 = Emit<Func<_Usage, string>>.NewDynamicMethod();
            var l1 = e1.DefineLabel();
            

            e1.LoadArgument(0);
            e1.LoadField(typeof(_Usage).GetField("Foo"));
            e1.Duplicate();
            e1.BranchIfFalse(l1);
            e1.Return();

            e1.MarkLabel(l1);
            e1.Pop();
            e1.LoadConstant("foo");
            e1.Return();

            var x = e1.TraceOperationResultUsage();
            Assert.AreEqual("(ldarg.0) result is used by (ldfld System.String Foo)", x.ElementAt(0).ToString());
            Assert.AreEqual("(ldfld System.String Foo) result is used by (brfalse _label0, pop, ret)", x.ElementAt(1).ToString());
            Assert.AreEqual("(dup) result is used by (brfalse _label0)", x.ElementAt(2).ToString());
            Assert.AreEqual("(brfalse _label0) result is unused", x.ElementAt(3).ToString());
            Assert.AreEqual("(ret) result is unused", x.ElementAt(4).ToString());
            Assert.AreEqual("(pop) result is unused", x.ElementAt(5).ToString());
            Assert.AreEqual("(ldstr foo) result is used by (ret)", x.ElementAt(6).ToString());
            Assert.AreEqual("(ret) result is unused", x.ElementAt(7).ToString());
        }

        class _FollowTrace
        {
#pragma warning disable 0649
            public int _Bar;
#pragma warning restore 0649
        }

        [TestMethod]
        public void FollowTrace()
        {
            var e1 = Emit<Func<_FollowTrace, _FollowTrace, int>>.NewDynamicMethod();
            var l = e1.DeclareLocal<int>("l");

            var bar = typeof(_FollowTrace).GetField("_Bar");

            e1.LoadArgument(0);
            e1.LoadField(bar);
            e1.LoadConstant(1);
            e1.Add();
            e1.LoadArgument(1);
            e1.LoadField(bar);
            e1.LoadConstant(2);
            e1.Multiply();
            e1.StoreLocal(l);
            e1.Return();

            var x = e1.TraceOperationResultUsage();

            {
                var steps = new List<IEnumerable<OperationResultUsage<Func<_FollowTrace, _FollowTrace, int>>>>();

                IEnumerable<OperationResultUsage<Func<_FollowTrace, _FollowTrace, int>>> prev = x.Where(r => r.ProducesResult.OpCode == OpCodes.Ldarg_0).ToList();
                IEnumerable<OperationResultUsage<Func<_FollowTrace, _FollowTrace, int>>> cur = prev;

                while (cur.Count() > 0)
                {
                    prev = cur;

                    steps.Add(prev);

                    cur = x.Where(r => prev.Any(p => p.ResultUsedBy.Contains(r.ProducesResult))).ToList();
                }

                Assert.AreEqual(4, steps.Count);
                Assert.AreEqual("(ldarg.0) result is used by (ldfld Int32 _Bar)", string.Join(", ", steps[0]));
                Assert.AreEqual("(ldfld Int32 _Bar) result is used by (add)", string.Join(", ", steps[1]));
                Assert.AreEqual("(add) result is used by (ret)", string.Join(", ", steps[2]));
                Assert.AreEqual("(ret) result is unused", string.Join(", ", steps[3]));
            }

            {
                var steps = new List<IEnumerable<OperationResultUsage<Func<_FollowTrace, _FollowTrace, int>>>>();

                IEnumerable<OperationResultUsage<Func<_FollowTrace, _FollowTrace, int>>> prev = x.Where(r => r.ProducesResult.OpCode == OpCodes.Ldarg_1).ToList();
                IEnumerable<OperationResultUsage<Func<_FollowTrace, _FollowTrace, int>>> cur = prev;

                while (cur.Count() > 0)
                {
                    prev = cur;

                    steps.Add(prev);

                    cur = x.Where(r => prev.Any(p => p.ResultUsedBy.Contains(r.ProducesResult))).ToList();
                }

                Assert.AreEqual(4, steps.Count);
                Assert.AreEqual("(ldarg.1) result is used by (ldfld Int32 _Bar)", string.Join(", ", steps[0]));
                Assert.AreEqual("(ldfld Int32 _Bar) result is used by (mul)", string.Join(", ", steps[1]));
                Assert.AreEqual("(mul) result is used by (stloc.0)", string.Join(", ", steps[2]));
                Assert.AreEqual("(stloc.0) result is unused", string.Join(", ", steps[3]));
            }
        }

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
#if COREFX
                il.EmitCalli(OpCodes.Calli, System.Reflection.CallingConventions.Standard, typeof(void), Type.EmptyTypes, null);
#else
                il.EmitCalli(OpCodes.Calli, System.Runtime.InteropServices.CallingConvention.StdCall, typeof(void), Type.EmptyTypes);
#endif
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

        [TestMethod]
        public void StrictBranchVerification()
        {
            // See: https://github.com/kevin-montrose/Sigil/issues/15

            var il = Sigil.Emit<Func<int>>.NewDynamicMethod(strictBranchVerification: true);
            var a = il.DefineLabel();
            var b = il.DefineLabel();
            var c = il.DefineLabel();

            il.Branch(a);

            il.MarkLabel(b);    // <- See that there?  The spec says we assume the stack is empty NOW since we haven't seen a branch to B
            il.LoadConstant(3);
            il.Branch(c);

            il.MarkLabel(a);    // stack should be assumed to be empty, because we SAW a branch to A
            il.LoadConstant(2);

            try
            {
                il.Branch(b);       // <- should explode, stack is *known* to be empty at b but there's a constant on the stack at this branch
                
                Assert.Fail();
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Branch expected the stack of be empty", e.Message);
            }
        }
    }
}
