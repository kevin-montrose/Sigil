using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestClass, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Instructions
    {
        [TestMethod]
        public void Simple()
        {
            var emitter = Emit<Func<int, string>>.NewDynamicMethod("E1");
            emitter
                .LoadArgument(0)
                .LoadArgument(0)
                .Add()
                .LoadArgument(0)
                .Multiply()
                .Box<int>()
                .CallVirtual(typeof(object).GetMethod("ToString", Type.EmptyTypes))
                .Return();

            var unoptimized = emitter.Instructions();
            string optimized;

            var d1 = emitter.CreateDelegate(out optimized);

            Assert.AreEqual(((2 + 2) * 2).ToString(), d1(2));

            Assert.AreEqual("ldarg.0\r\nldarg.0\r\nadd\r\nldarg.0\r\nmul\r\nbox System.Int32\r\ncallvirt System.String ToString()\r\nret", unoptimized);
            Assert.AreEqual("ldarg.0\r\nldarg.0\r\nadd\r\nldarg.0\r\nmul\r\nbox System.Int32\r\ntail.callvirt System.String ToString()\r\nret\r\n", optimized);
        }
    }
}
