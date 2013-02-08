using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestClass]
    public class Readme
    {
        [TestMethod]
        public void Block1()
        {
            {
                var emiter = Emit<Func<int>>.NewDynamicMethod("MyMethod");
                Assert.IsNotNull(emiter);
            }

            {
                var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                var mod = asm.DefineDynamicModule("Bar");

                TypeBuilder myBuilder = mod.DefineType("T");
                var emiter = Emit<Func<int, string>>.BuildMethod(myBuilder, "Static", MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard);

                Assert.IsNotNull(emiter);
            }

            {
                var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                var mod = asm.DefineDynamicModule("Bar");

                TypeBuilder myBuilder = mod.DefineType("T");
                var emiter = Emit<Func<int, string>>.BuildMethod(myBuilder, "Instance", MethodAttributes.Public, CallingConventions.Standard | CallingConventions.HasThis);
                // Technically this is a Func<myBuilder, int string>; but because myBuilder isn't complete
                //   the generic parameters skip the `this` reference.  myBuilder will still be available as the
                //   first argument to the method

                Assert.IsNotNull(emiter);
            }
        }

        [TestMethod]
        public void Block2()
        {
            // Create a delegate that sums two integers
            var emiter = Emit<Func<int, int, int>>.NewDynamicMethod("MyMethod");
            emiter.LoadArgument(0);
            emiter.LoadArgument(1);
            emiter.Add();
            emiter.Return();
            var del = emiter.CreateDelegate();

            // prints "473"
            //Console.WriteLine(del(314, 159));
            Assert.AreEqual(473, del(314, 159));
        }

        [TestMethod]
        public void Block3()
        {
            try
            {
                var emiter = Emit<Func<int, string, int>>.NewDynamicMethod("MyMethod");
                emiter.LoadArgument(0);
                emiter.LoadArgument(1);
                emiter.Add();   // Throws a SigilVerificationException, indicating that Add() isn't defined for [int, string]
                emiter.Return();

                Assert.Fail("An exception should have been thrown");
            }
            catch (SigilVerificationException e)
            {
                Assert.AreEqual("Add with an int32 expects an int32, native int, reference, or pointer as a second value; found System.String", e.Message);
            }
        }

        private static bool MayFailFirstCall = true;
        public static void MayFail()
        {
            if (MayFailFirstCall)
            {
                MayFailFirstCall = false;
                return;
            }

            throw new Exception();
        }

        private static bool AlwaysCallCalled;
        public static void AlwaysCall()
        {
            AlwaysCallCalled = true;
        }

        [TestMethod]
        public void Block4()
        {
            MethodInfo mayFail = typeof(Readme).GetMethod("MayFail");
            MethodInfo alwaysCall = typeof(Readme).GetMethod("AlwaysCall");
            var emiter = Emit<Func<string, bool>>.NewDynamicMethod("TryCatchFinally");

            var inputIsNull = emiter.DefineLabel("ifNull");  // names are purely for ease of debugging, and are optional
            var tryCall = emiter.DefineLabel("tryCall");

            emiter.LoadArgument(0);
            emiter.LoadNull();
            emiter.BranchIfEqual(inputIsNull);
            emiter.Branch(tryCall);

            emiter.MarkLabel(inputIsNull);
            emiter.LoadConstant(false);
            emiter.Return();

            emiter.MarkLabel(tryCall);

            var succeeded = emiter.DeclareLocal<bool>("succeeded");
            var t = emiter.BeginExceptionBlock();
            emiter.Call(mayFail);
            emiter.LoadConstant(true);
            emiter.StoreLocal(succeeded);

            var c = emiter.BeginCatchAllBlock(t);
            emiter.Pop();   // Remove exception
            emiter.LoadConstant(false);
            emiter.StoreLocal(succeeded);
            emiter.EndCatchBlock(c);

            var f = emiter.BeginFinallyBlock(t);
            emiter.Call(alwaysCall);
            emiter.EndFinallyBlock(f);

            emiter.EndExceptionBlock(t);

            emiter.LoadLocal(succeeded);
            emiter.Return();

            var del = emiter.CreateDelegate();

            MayFailFirstCall = true;
            AlwaysCallCalled = false;
            Assert.IsTrue(del("hello"));
            Assert.IsTrue(AlwaysCallCalled);

            AlwaysCallCalled = false;
            Assert.IsFalse(del("world"));
            Assert.IsTrue(AlwaysCallCalled);

            AlwaysCallCalled = false;
            Assert.IsFalse(del(null));
            Assert.IsFalse(AlwaysCallCalled);
        }

        [TestMethod]
        public void Block5()
        {
            Action mayFail = () => MayFail();
            Action alwaysCall = () => AlwaysCall();

            Func<string, bool> del =
               s =>
               {
                   if (s == null) return false;

                   bool succeeded;
                   try
                   {
                       mayFail();
                       succeeded = true;
                   }
                   catch
                   {
                       succeeded = false;
                   }
                   finally
                   {
                       alwaysCall();
                   }

                   return succeeded;
               };

            MayFailFirstCall = true;
            AlwaysCallCalled = false;
            Assert.IsTrue(del("hello"));
            Assert.IsTrue(AlwaysCallCalled);

            AlwaysCallCalled = false;
            Assert.IsFalse(del("world"));
            Assert.IsTrue(AlwaysCallCalled);

            AlwaysCallCalled = false;
            Assert.IsFalse(del(null));
            Assert.IsFalse(AlwaysCallCalled);
        }
    }
}
