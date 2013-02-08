using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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
    [TestClass]
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
            catch (VerificationException e) { }

            {
                var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                var mod = asm.DefineDynamicModule("Bar");
                var t = mod.DefineType("T");

                var e1 = Emit<Action<int[]>>.BuildMethod(t, "E1", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard);
                e1.LoadArgument(0).LoadConstant(0);

                try
                {
                    e1.LoadElementAddress();
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
                    .LoadElementAddress()
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
            catch (VerificationException e) { }

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
    }
}
