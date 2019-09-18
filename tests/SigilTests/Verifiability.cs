using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security;
using Xunit;

namespace SigilTests
{
    /// <summary>
    /// This class is a little tricky.
    /// The actual CLR's behavior differs a tad from the ECMA standard.
    /// So, what happens here is we first confirm that something is unverifiable (using DynamicMethod without a specified module)
    /// and then confirm that Sigil won't allow that instruction (or instruction sequence) when in a "verified instructions only" mode.
    /// And then that the same sequence actually works in "unverified" mode.
    /// </summary>
    public class Verifiability
    {
        private class _Usage
        {
#pragma warning disable 0649
            public string Foo;
#pragma warning restore 0649
        }

        [Fact]
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
            Assert.Equal("(ldarg.0) result is used by (ldfld System.String Foo)", x.ElementAt(0).ToString());
            Assert.Equal("(ldfld System.String Foo) result is used by (brfalse _label0, pop, ret)", x.ElementAt(1).ToString());
            Assert.Equal("(dup) result is used by (brfalse _label0)", x.ElementAt(2).ToString());
            Assert.Equal("(brfalse _label0) result is unused", x.ElementAt(3).ToString());
            Assert.Equal("(ret) result is unused", x.ElementAt(4).ToString());
            Assert.Equal("(pop) result is unused", x.ElementAt(5).ToString());
            Assert.Equal("(ldstr foo) result is used by (ret)", x.ElementAt(6).ToString());
            Assert.Equal("(ret) result is unused", x.ElementAt(7).ToString());
        }

        private class _FollowTrace
        {
#pragma warning disable 0649
            public int _Bar;
#pragma warning restore 0649
        }

        [Fact]
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

                Assert.Equal(4, steps.Count);
                Assert.Equal("(ldarg.0) result is used by (ldfld Int32 _Bar)", string.Join(", ", steps[0]));
                Assert.Equal("(ldfld Int32 _Bar) result is used by (add)", string.Join(", ", steps[1]));
                Assert.Equal("(add) result is used by (ret)", string.Join(", ", steps[2]));
                Assert.Equal("(ret) result is unused", string.Join(", ", steps[3]));
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

                Assert.Equal(4, steps.Count);
                Assert.Equal("(ldarg.1) result is used by (ldfld Int32 _Bar)", string.Join(", ", steps[0]));
                Assert.Equal("(ldfld Int32 _Bar) result is used by (mul)", string.Join(", ", steps[1]));
                Assert.Equal("(mul) result is used by (stloc.0)", string.Join(", ", steps[2]));
                Assert.Equal("(stloc.0) result is unused", string.Join(", ", steps[3]));
            }
        }

        [Fact]
        public void Ldelema()
        {
            {
                var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                var mod = asm.DefineDynamicModule("Bar");
                var t = mod.DefineType("T");

                var e1 = Emit<Action<int[]>>.BuildMethod(t, "E1", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard);
                e1.LoadArgument(0).LoadConstant(0);

                var ex = Assert.Throws<InvalidOperationException>(() => e1.LoadElementAddress<int>());
                Assert.Equal("LoadElementAddress isn't verifiable", ex.Message);
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

        [Fact]
        public void Calli()
        {
            var writeLine = typeof(Console).GetMethod("WriteLine", Type.EmptyTypes);

            // only framework seems to throw here, so we still need to RESTRICT this
            //   for portability but this part of the test is bogus
#if !NETCOREAPP
            {
                var dyn = new DynamicMethod("E1", typeof(void), Type.EmptyTypes);
                var il = dyn.GetILGenerator();
                il.Emit(OpCodes.Ldftn, writeLine);

                il.EmitCalli(OpCodes.Calli, System.Runtime.InteropServices.CallingConvention.StdCall, typeof(void), Type.EmptyTypes);

                il.Emit(OpCodes.Ret);

                var d1 = (Action)dyn.CreateDelegate(typeof(Action));

                Assert.Throws<VerificationException>(() => d1());
            }
#endif

            {
                var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                var mod = asm.DefineDynamicModule("Bar");
                var t = mod.DefineType("T");

                var e1 = Emit<Action>.BuildMethod(t, "E1", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard);
                e1.LoadFunctionPointer(writeLine);

                var ex = Assert.Throws<InvalidOperationException>(() => e1.CallIndirect(CallingConventions.Standard));
                Assert.Equal("CallIndirect isn't verifiable", ex.Message);
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

        [Fact]
        public void Localloc()
        {
            // only framework seems to throw here, so we still need to RESTRICT this
            //   for portability but this part of the test is bogus
#if !NETCOREAPP
            {
                var dyn = new DynamicMethod("E1", typeof(void), Type.EmptyTypes);
                var il = dyn.GetILGenerator();
                il.Emit(OpCodes.Ldc_I4_8);
                il.Emit(OpCodes.Localloc);
                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Ret);

                var d1 = (Action)dyn.CreateDelegate(typeof(Action));
                Assert.Throws<VerificationException>(() => d1());
            }
#endif

            {
                var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                var mod = asm.DefineDynamicModule("Bar");
                var t = mod.DefineType("T");

                var e1 = Emit<Action>.BuildMethod(t, "E1", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard);
                e1.LoadConstant(8);

                var ex = Assert.Throws<InvalidOperationException>(() => e1.LocalAllocate());
                Assert.Equal("LocalAllocate isn't verifiable", ex.Message);
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

        [Fact]
        public void Cpblk()
        {
            var src = Marshal.AllocHGlobal(8);
            var dest = Marshal.AllocHGlobal(8);

            try
            {
                // Cpblk isn't verifiable in framework, but is in Core now
                //    so we still need to forbid it by default for portability 
                //    purposes but this part of the test can't run in Core
#if !NETCOREAPP
                {
                    var dyn = new DynamicMethod("E1", typeof(void), new[] { typeof(IntPtr), typeof(IntPtr) });
                    var il = dyn.GetILGenerator();
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldc_I4_8);
                    il.Emit(OpCodes.Cpblk);
                    il.Emit(OpCodes.Ret);

                    var d1 = (Action<IntPtr, IntPtr>)dyn.CreateDelegate(typeof(Action<IntPtr, IntPtr>));
                    Assert.Throws<VerificationException>(() => d1(dest, src));
                }
#endif

                {
                    var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                    var mod = asm.DefineDynamicModule("Bar");
                    var t = mod.DefineType("T");

                    var e1 = Emit<Action<IntPtr, IntPtr>>.BuildMethod(t, "E1", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard);
                    e1.LoadArgument(0);
                    e1.LoadArgument(1);
                    e1.LoadConstant(8);

                    var ex = Assert.Throws<InvalidOperationException>(() => e1.CopyBlock());
                    Assert.Equal("CopyBlock isn't verifiable", ex.Message);
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

        [Fact]
        public void Initblk()
        {
            var blk = Marshal.AllocHGlobal(8);

            try
            {
                // Initblk isn't verifiable in framework, but is in Core now
                //    so we still need to forbid it by default for portability 
                //    purposes but this part of the test can't run in Core
#if !NETCOREAPP
                {
                    var dyn = new DynamicMethod("E1", typeof(void), new[] { typeof(IntPtr) });
                    var il = dyn.GetILGenerator();
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldc_I4_1);
                    il.Emit(OpCodes.Ldc_I4_8);
                    il.Emit(OpCodes.Initblk);
                    il.Emit(OpCodes.Ret);

                    var d1 = (Action<IntPtr>)dyn.CreateDelegate(typeof(Action<IntPtr>));
                    Assert.Throws<VerificationException>(() => d1(blk));
                }
#endif

                {
                    var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                    var mod = asm.DefineDynamicModule("Bar");
                    var t = mod.DefineType("T");

                    var e1 = Emit<Action<IntPtr>>.BuildMethod(t, "E1", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard);
                    e1.LoadArgument(0);
                    e1.LoadConstant(1);
                    e1.LoadConstant(8);

                    var ex = Assert.Throws<InvalidOperationException>(() => e1.InitializeBlock());
                    Assert.Equal("InitializeBlock isn't verifiable", ex.Message);
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

        [Fact]
        public void Jmp()
        {
            var writeLine = typeof(Console).GetMethod("WriteLine", Type.EmptyTypes);

            // Initblk isn't verifiable in framework, but is in Core now
            //    so we still need to forbid it by default for portability 
            //    purposes but this part of the test can't run in Core
#if !NETCOREAPP
            {
                var dyn = new DynamicMethod("E1", typeof(void), Type.EmptyTypes);
                var il = dyn.GetILGenerator();
                il.Emit(OpCodes.Jmp, writeLine);
                il.Emit(OpCodes.Ret);

                var d1 = (Action)dyn.CreateDelegate(typeof(Action));
                Assert.Throws<VerificationException>(() => d1());
            }
#endif

            {
                var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                var mod = asm.DefineDynamicModule("Bar");
                var t = mod.DefineType("T");

                var e1 = Emit<Action>.BuildMethod(t, "E1", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard);

                var ex = Assert.Throws<InvalidOperationException>(() => e1.Jump(writeLine));
                Assert.Equal("Jump isn't verifiable", ex.Message);
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

        [Fact]
        public void LdfldaInitOnly()
        {
            var fld = typeof(LdfldaClass).GetField("Field");

            // Ldflda on INIT ONLY fields isn't verifiable in framework, but is in Core now
            //    so we still need to forbid it by default for portability 
            //    purposes but this part of the test can't run in Core
#if !NETCOREAPP
            {
                var dyn = new DynamicMethod("E1", typeof(void), new[] { typeof(LdfldaClass) });
                var il = dyn.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldflda, fld);
                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Ret);

                var d1 = (Action<LdfldaClass>)dyn.CreateDelegate(typeof(Action<LdfldaClass>));
                Assert.Throws<VerificationException>(() => d1(new LdfldaClass()));
            }
#endif

            {
                var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                var mod = asm.DefineDynamicModule("Bar");
                var t = mod.DefineType("T");

                var e1 = Emit<Action<LdfldaClass>>.BuildMethod(t, "E1", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard);
                e1.LoadArgument(0);

                var ex = Assert.Throws<InvalidOperationException>(() => e1.LoadFieldAddress(fld));
                Assert.Equal("LoadFieldAddress on InitOnly fields is not verifiable", ex.Message);
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

        [Fact]
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

            // <- should explode, stack is *known* to be empty at b but there's a constant on the stack at this branch
            var ex = Assert.Throws<SigilVerificationException>(() => il.Branch(b));
            Assert.Equal("Branch expected the stack of be empty", ex.Message);
        }
    }
}
