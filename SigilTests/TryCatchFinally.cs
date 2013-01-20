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
    [TestClass]
    public class TryCatchFinally
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
            var y = e1.CreateLocal<string>("y");

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
    }
}
