using Sigil;
using System;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace SigilTests
{
    public partial class Arglist
    {
        [Fact]
        public void Simple()
        {
            MethodInfo mtd;

            {
                var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                var mod = asm.DefineDynamicModule("Bar");
                var t = mod.DefineType("T");

                var e1 = Emit<Func<int>>.BuildMethod(t, "VarArgsMethod", MethodAttributes.Static | MethodAttributes.Public, CallingConventions.VarArgs, allowUnverifiableCode: true);
                e1.ArgumentList();
                e1.Convert<int>();
                e1.Return();

                e1.CreateMethod(out string instr1);

                var type = t.CreateType();
                mtd = type.GetMethod("VarArgsMethod");
            }

            {
                var e2 = Emit<Func<int>>.NewDynamicMethod();
                e2.LoadConstant("hello");
                e2.LoadConstant(123);
                e2.LoadConstant(1.23);
                e2.Call(mtd, new[] { typeof(string), typeof(int), typeof(double) });
                e2.Return();

                var d2 = e2.CreateDelegate(out string instr2);

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
