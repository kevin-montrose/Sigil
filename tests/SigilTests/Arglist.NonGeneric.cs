using Sigil.NonGeneric;
using System;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace SigilTests
{
    public partial class Arglist
    {
        [Fact]
        public void SimpleNonGeneric()
        {
            MethodInfo mtd;

            {
                var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                var mod = asm.DefineDynamicModule("Bar");
                var t = mod.DefineType("T");

                var e1 = Emit.BuildMethod(typeof(int), Type.EmptyTypes, t, "VarArgsMethod", MethodAttributes.Static | MethodAttributes.Public, CallingConventions.VarArgs, allowUnverifiableCode: true);
                e1.ArgumentList();
                e1.Convert<int>();
                e1.Return();

                e1.CreateMethod(out string instr1);

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

                var d2 = (Func<int>)e2.CreateDelegate(typeof(Func<int>), out string instr2);

#if NETCOREAPP2_2
                Assert.Throws<InvalidProgramException>(() => d2());
#else
                var i = d2();
                Assert.NotEqual(0, i);
#endif
            }
        }
    }
}
