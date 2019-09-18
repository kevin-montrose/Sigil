using Sigil;
using System;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace SigilTests
{
    public partial class Readme
    {
        [Fact]
        public void Block1()
        {
            {
                var emiter = Emit<Func<int>>.NewDynamicMethod("MyMethod");
                Assert.NotNull(emiter);
            }

            {
                var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                var mod = asm.DefineDynamicModule("Bar");

                TypeBuilder myBuilder = mod.DefineType("T");
                var emiter = Emit<Func<int, string>>.BuildMethod(myBuilder, "Static", MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard);

                Assert.NotNull(emiter);
            }

            {
                var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                var mod = asm.DefineDynamicModule("Bar");

                TypeBuilder myBuilder = mod.DefineType("T");
                var emiter = Emit<Func<int, string>>.BuildMethod(myBuilder, "Instance", MethodAttributes.Public, CallingConventions.HasThis);
                // Technically this is a Func<myBuilder, int string>; but because myBuilder isn't complete
                //   the generic parameters skip the `this` reference.  myBuilder will still be available as the
                //   first argument to the method

                Assert.NotNull(emiter);
            }
        }

        [Fact]
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
            Assert.Equal(473, del(314, 159));
        }

        [Fact]
        public void Block3()
        {
            var emiter = Emit<Func<int, string, int>>.NewDynamicMethod("MyMethod");
            emiter.LoadArgument(0);
            emiter.LoadArgument(1);

            var ex = Assert.Throws<SigilVerificationException>(() => emiter.Add());
            Assert.Equal("Add expected a by ref, double, float, int, long, native int, or pointer; found System.String", ex.Message);
        }

        private static bool MayFailFirstCall = true;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1013:Public method should be marked as test", Justification = "It needs to be public")]
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1013:Public method should be marked as test", Justification = "It needs to be public")]
        public static void AlwaysCall()
        {
            AlwaysCallCalled = true;
        }

        [Fact]
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
            Assert.True(del("hello"));
            Assert.True(AlwaysCallCalled);

            AlwaysCallCalled = false;
            Assert.False(del("world"));
            Assert.True(AlwaysCallCalled);

            AlwaysCallCalled = false;
            Assert.False(del(null));
            Assert.False(AlwaysCallCalled);
        }

        [Fact]
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
            Assert.True(del("hello"));
            Assert.True(AlwaysCallCalled);

            AlwaysCallCalled = false;
            Assert.False(del("world"));
            Assert.True(AlwaysCallCalled);

            AlwaysCallCalled = false;
            Assert.False(del(null));
            Assert.False(AlwaysCallCalled);
        }

        [Fact]
        public void Block6()
        {
            var emiter = Emit<Func<int>>.NewDynamicMethod("Unconditional");
            var label1 = emiter.DefineLabel("label1");
            var label2 = emiter.DefineLabel("label2");
            var label3 = emiter.DefineLabel("label3");

            emiter.LoadConstant(1);
            emiter.Branch(label1);

            emiter.MarkLabel(label2);
            emiter.LoadConstant(2);
            emiter.Branch(label3);

            emiter.MarkLabel(label1);
            emiter.Branch(label2);

            emiter.MarkLabel(label3); // the top of the stack is the first element
            emiter.Add();
            emiter.Return();

            var d = emiter.CreateDelegate();

            Assert.Equal(3, d());
        }
    }
}
