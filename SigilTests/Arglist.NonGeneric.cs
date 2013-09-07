using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    public partial class Arglist
    {
        [TestMethod]
        public void SimpleNonGeneric()
        {
            MethodInfo mtd;
            string instr1;
            string instr2;

            {
                var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                var mod = asm.DefineDynamicModule("Bar");
                var t = mod.DefineType("T");

                var e1 = Emit.BuildMethod(typeof(int), Type.EmptyTypes, t, "VarArgsMethod", MethodAttributes.Static | MethodAttributes.Public, CallingConventions.VarArgs, allowUnverifiableCode: true);
                e1.ArgumentList();
                e1.Convert<int>();
                e1.Return();

                e1.CreateMethod(out instr1);

                var type = t.CreateType();
                mtd = type.GetMethod("VarArgsMethod");
            }

            {
                var e2 = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes);
                e2.LoadConstant("hello");
                e2.LoadConstant(123);
                e2.LoadConstant(1.23);
                e2.Call(mtd, new[] { typeof(string), typeof(int), typeof(double) });
                e2.Return();

                var d2 = (Func<int>)e2.CreateDelegate(typeof(Func<int>), out instr2);

                var i = d2();
                Assert.AreNotEqual(0, i);
            }
        }
    }
}
