using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestClass]
    public class LoadConstants
    {
        [TestMethod]
        public void AllBools()
        {
            {
                var e1 = Emit<Func<bool>>.NewDynamicMethod();
                e1.LoadConstant(true);
                e1.Return();

                var d1 = e1.CreateDelegate();
                Assert.IsTrue(d1());
            }

            {
                var e1 = Emit<Func<bool>>.NewDynamicMethod();
                e1.LoadConstant(false);
                e1.Return();

                var d1 = e1.CreateDelegate();
                Assert.IsFalse(d1());
            }
        }

        [TestMethod]
        public void AllInts()
        {
            for (var i = -1; i <= 256; i++)
            {
                var e1 = Emit<Func<int>>.NewDynamicMethod();
                e1.LoadConstant(i);
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual(i, d1());
            }
        }

        [TestMethod]
        public void AllUInts()
        {
            for (uint i = 0; i <= 256; i++)
            {
                var e1 = Emit<Func<uint>>.NewDynamicMethod();
                e1.LoadConstant(i);
                e1.Return();

                var d1 = e1.CreateDelegate();

                Assert.AreEqual(i, d1());
            }
        }

        [TestMethod]
        public void Long()
        {
            var e1 = Emit<Func<long>>.NewDynamicMethod();
            e1.LoadConstant(long.MaxValue);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(long.MaxValue, d1());
        }

        [TestMethod]
        public void ULong()
        {
            var e1 = Emit<Func<ulong>>.NewDynamicMethod();
            e1.LoadConstant(ulong.MaxValue);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(ulong.MaxValue, d1());
        }

        [TestMethod]
        public void Float()
        {
            var e1 = Emit<Func<float>>.NewDynamicMethod();
            e1.LoadConstant(12.34f);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(12.34f, d1());
        }

        [TestMethod]
        public void Double()
        {
            var e1 = Emit<Func<double>>.NewDynamicMethod();
            e1.LoadConstant(12.34);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(12.34, d1());
        }

        [TestMethod]
        public void String()
        {
            var e1 = Emit<Func<string>>.NewDynamicMethod();
            e1.LoadConstant("hello world");
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual("hello world", d1());
        }

        [TestMethod]
        public void Type()
        {
            var e1 = Emit<Func<Type>>.NewDynamicMethod();
            e1.LoadConstant<string>();
            e1.Call(typeof(Type).GetMethod("GetTypeFromHandle"));
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(typeof(string), d1());
        }

        [TestMethod]
        public void Method()
        {
            var e1 = Emit<Func<RuntimeMethodHandle>>.NewDynamicMethod();
            e1.LoadConstant(typeof(RuntimeMethodHandle).GetMethod("GetFunctionPointer"));
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.IsNotNull(d1());
        }

        class FieldClass
        {
            public int Foo;

            public FieldClass() { Foo = 123; }
        }

        [TestMethod]
        public void Field()
        {
            var e1 = Emit<Func<RuntimeFieldHandle>>.NewDynamicMethod();
            e1.LoadConstant(typeof(FieldClass).GetField("Foo"));
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.IsNotNull(d1());
        }
    }
}
