using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestClass, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class CallIndirect
    {
        public static string Foo(int i)
        {
            var ret = "";

            for (var j = 0; j < i; j++)
            {
                ret += "Bar";
            }

            return ret;
        }

        [TestMethod]
        public void Simple()
        {
            var foo = typeof(CallIndirect).GetMethod("Foo");

            var e1 = Emit<Func<string>>.NewDynamicMethod("E1");
            e1.LoadConstant(3);
            e1.LoadFunctionPointer(foo);
            e1.CallIndirect<string, int>(foo.CallingConvention);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual("BarBarBar", d1());
        }

        public class VirtualClass
        {
            public override string ToString()
            {
                return "I'm Virtual!";
            }
        }

        [TestMethod]
        public void Virtual()
        {
            var toString = typeof(object).GetMethod("ToString");

            var e1 = Emit<Func<string>>.NewDynamicMethod("E1");
            e1.NewObject<VirtualClass>();
            e1.Duplicate();
            e1.LoadVirtualFunctionPointer(toString);
            e1.CallIndirect<string>(toString.CallingConvention);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual("I'm Virtual!", d1());

            /*var toString = typeof(object).GetMethod("ToString");

            var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
            var mod = asm.DefineDynamicModule("Bar");

            var dynMethod = new DynamicMethod("E1", typeof(string), Type.EmptyTypes, mod, skipVisibility: true);
            var il = dynMethod.GetILGenerator();

            il.Emit(OpCodes.Newobj, typeof(VirtualClass).GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Ldvirtftn, toString);
            il.EmitCalli(OpCodes.Calli, toString.CallingConvention, typeof(string), Type.EmptyTypes, null);
            il.Emit(OpCodes.Ret);

            var d1 = (Func<string>)dynMethod.CreateDelegate(typeof(Func<string>));

            d1();*/
        }
    }
}
