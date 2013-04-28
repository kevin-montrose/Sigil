using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection.Emit;

namespace SigilTests
{
    [TestClass]
    public class Disassember
    {
        [TestMethod]
        public void Simple()
        {
            Func<int, int, int> d1 = (a, b) => a + b;

            var ops = Sigil.Disassembler.Decompile<Func<int, int, int>>(d1);

            Assert.IsNotNull(ops);
            Assert.AreEqual(OpCodes.Ldarg_0, ops[0].OpCode);
            Assert.AreEqual(OpCodes.Ldarg_1, ops[1].OpCode);
            Assert.AreEqual(OpCodes.Add, ops[2].OpCode);
            Assert.AreEqual(OpCodes.Ret, ops[3].OpCode);

            var recompiled = ops.EmitAll();
            Assert.IsNotNull(recompiled);

            string instrs;
            var r1 = recompiled.CreateDelegate(out instrs);
            Assert.IsNotNull(r1);

            for (var i = 0; i < 200; i++)
            {
                for (var j = 0; j < 200; j++)
                {
                    Assert.AreEqual(d1(i, j), r1(i, j));
                }
            }

            Assert.AreEqual("ldarg.0\r\nldarg.1\r\nadd\r\nret\r\n", instrs);
        }
    }
}
