using Sigil;
using System;
using System.Runtime.CompilerServices;
using Xunit;

namespace SigilTests
{
    public partial class TryCatchFinally
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1013:Public method should be marked as test", Justification = "It needs to be public")]
        public static void Throws()
        {
            if(DateTime.UtcNow > new DateTime(1970, 1, 1))
                throw new Exception("Whatever");
        }

        [Fact]
        public void Simple()
        {
            var e1 = Emit<Func<string>>.NewDynamicMethod("E1");
            var y = e1.DeclareLocal<string>("y");

            e1.LoadConstant("");
            e1.StoreLocal(y);
            var exc = e1.BeginExceptionBlock();
            e1.Call(typeof(TryCatchFinally).GetMethod("Throws"));
            var excCatch = e1.BeginCatchBlock<Exception>(exc);
            e1.CallVirtual(typeof(Exception).GetProperty("Message").GetGetMethod());
            e1.StoreLocal(y);
            e1.EndCatchBlock(excCatch);
            e1.EndExceptionBlock(exc);
            e1.LoadLocal(y);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal("Whatever", d1());
        }

        [Fact]
        public void Finally()
        {
            var e1 = Emit<Func<string>>.NewDynamicMethod("E1");
            var y = e1.DeclareLocal<string>("y");

            e1.LoadConstant("");
            e1.StoreLocal(y);

            var exc = e1.BeginExceptionBlock();
            e1.Call(typeof(TryCatchFinally).GetMethod("Throws"));

            var excCatch = e1.BeginCatchBlock<Exception>(exc);
            e1.CallVirtual(typeof(Exception).GetProperty("Message").GetGetMethod());
            e1.StoreLocal(y);
            e1.EndCatchBlock(excCatch);

            var fbk = e1.BeginFinallyBlock(exc);
            e1.LoadConstant("Finally!");
            e1.StoreLocal(y);
            e1.EndFinallyBlock(fbk);

            e1.EndExceptionBlock(exc);

            e1.LoadLocal(y);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal("Finally!", d1());
        }

        [Fact]
        public void IsCatchAll()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");
            var t1 = e1.BeginExceptionBlock();
            var c1 = e1.BeginCatchAllBlock(t1);

            Assert.True(c1.IsCatchAll);

            var e2 = Emit<Action>.NewDynamicMethod("E2");
            var t2 = e2.BeginExceptionBlock();
            var c2 = e2.BeginCatchBlock<Exception>(t2);

            Assert.True(c2.IsCatchAll);

            var e3 = Emit<Action>.NewDynamicMethod("E3");
            var t3 = e3.BeginExceptionBlock();

            // Maybe on netcoreapp
            // var c3 = e3.BeginCatchBlock<ArgumentException>(t3);
            var c3 = e3.BeginCatchBlock<StackOverflowException>(t3);

            Assert.False(c3.IsCatchAll);
        }
    }
}
