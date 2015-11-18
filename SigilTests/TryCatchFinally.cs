using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestClass, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class TryCatchFinally
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Throws()
        {
            if(DateTime.UtcNow > new DateTime(1970, 1, 1))
                throw new Exception("Whatever");
        }

        [TestMethod]
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

            Assert.AreEqual("Whatever", d1());
        }

        [TestMethod]
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

            Assert.AreEqual("Finally!", d1());
        }

        [TestMethod]
        public void IsCatchAll()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");
            var t1 = e1.BeginExceptionBlock();
            var c1 = e1.BeginCatchAllBlock(t1);

            Assert.IsTrue(c1.IsCatchAll);

            var e2 = Emit<Action>.NewDynamicMethod("E2");
            var t2 = e2.BeginExceptionBlock();
            var c2 = e2.BeginCatchBlock<Exception>(t2);

            Assert.IsTrue(c2.IsCatchAll);

            var e3 = Emit<Action>.NewDynamicMethod("E3");
            var t3 = e3.BeginExceptionBlock();
#if COREFX
            var c3 = e1.BeginCatchBlock<Exception>(t3);
#else
            var c3 = e3.BeginCatchBlock<StackOverflowException>(t3);
#endif

            Assert.IsFalse(c3.IsCatchAll);
        }
    }
}
