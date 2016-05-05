using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestClass, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class Tail
    {
        const string helloWorld = "Hello, World!";

        static private void BugCausingFuntion(TextWriter writer, ref char[] buffer)
        {
            Array.Copy(helloWorld.ToCharArray(), buffer, helloWorld.Length);

            writer.Write(buffer, 0, helloWorld.Length);
        }

        [TestMethod]
        public void TailBugWithRefArgs()
        {
            var bugCausingFuntion= typeof(Tail).GetMethod("BugCausingFuntion", BindingFlags.Static | BindingFlags.NonPublic);

            var emit = Emit<Action<TextWriter>>.NewDynamicMethod();

            var buffer = emit.DeclareLocal<char[]>();
            emit.LoadConstant(helloWorld.Length+1);
            emit.NewArray<char>();
            emit.StoreLocal(buffer);

            emit.LoadArgument(0);
            emit.LoadLocalAddress(buffer);
            emit.Call(bugCausingFuntion);
            emit.Return();
            
            var f = emit.CreateDelegate();

            var sw = new StringWriter();
            f(sw);

            var result = sw.GetStringBuilder().ToString();

            Assert.AreEqual(result, helloWorld);
        }

        private class ClassWithProperty
        {
            public string String { get; set; }
        }

        [TestMethod]
        public void TailBugWithVoidMethods()
        {
            // generate a method that creates a new instance of ClassWithProperty, assigns the property, and returns

            var emit = Emit<Func<ClassWithProperty>>.NewDynamicMethod();

            emit.NewObject<ClassWithProperty>(); // obj

            var prop = typeof(ClassWithProperty).GetProperty("String");
            emit.Duplicate();                   // obj obj
            emit.LoadConstant("please work");   // obj obj string
            emit.CallVirtual(prop.SetMethod);   // obj

            emit.Return();

            var f = emit.CreateDelegate();

            Assert.IsFalse(emit.Instructions().Contains("tail."));

            var obj = f();
            Assert.AreEqual("please work", obj.String);
        }
    }
}
