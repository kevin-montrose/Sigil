using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace SigilTests
{
    public partial class Calls
    {
        [Fact]
        public void ValueTypeCallIndirectNonGeneric()
        {
            var hasValue = typeof(int?).GetProperty("HasValue");
            var getHasValue = hasValue.GetGetMethod();

            var e1 = Emit.NewDynamicMethod(typeof(bool), new [] { typeof(int?) });
            e1.LoadArgumentAddress(0);
            e1.Duplicate();
            e1.LoadVirtualFunctionPointer(getHasValue);
            e1.CallIndirect(getHasValue.CallingConvention, typeof(bool));
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int?, bool>>();

            Assert.True(d1(1));
            Assert.False(d1(null));
        }

        [Fact]
        public void ValueTypeCallVirtualNonGeneric()
        {
            var hasValue = typeof(int?).GetProperty("HasValue");
            var getHasValue = hasValue.GetGetMethod();

            var e1 = Emit.NewDynamicMethod(typeof(bool), new [] { typeof(int?) });
            e1.LoadArgumentAddress(0);
            e1.CallVirtual(getHasValue);
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int?, bool>>();

            Assert.True(d1(1));
            Assert.False(d1(null));
        }

        [Fact]
        public void ValueTypeCallNonGeneric()
        {
            var hasValue = typeof(int?).GetProperty("HasValue");
            var getHasValue = hasValue.GetGetMethod();

            var e1 = Emit.NewDynamicMethod(typeof(bool), new [] { typeof(int?) });
            e1.LoadArgumentAddress(0);
            e1.Call(getHasValue);
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int?, bool>>();

            Assert.True(d1(1));
            Assert.False(d1(null));
        }

        [Fact]
        public void MultipleTailcallsNonGeneric()
        {
            var toString = typeof(object).GetMethod("ToString");

            var e1 = Emit.NewDynamicMethod(typeof(string), new [] { typeof(int) });
            var l1 = e1.DefineLabel("l1");
            var l2 = e1.DefineLabel("l2");
            var l3 = e1.DefineLabel("l3");

            e1.LoadArgument(0);
            e1.LoadConstant(0);
            e1.BranchIfEqual(l1);

            e1.LoadArgument(0);
            e1.LoadConstant(1);
            e1.BranchIfEqual(l2);

            e1.LoadArgument(0);
            e1.LoadConstant(2);
            e1.BranchIfEqual(l3);

            e1.LoadConstant("Foo");
            e1.Call(toString);
            e1.Return();

            e1.MarkLabel(l1);
            e1.NewObject<_MultipleTailcalls>();
            e1.CallVirtual(toString);
            e1.Return();

            // calli is can get tailcall _in theory_ but in practice it breaks the CLR... often
            //   so Sigil won't actually emit it (but it's still tested here for when it did)
            e1.MarkLabel(l2);
            e1.NewObject<object>();
            e1.Duplicate();
            e1.LoadVirtualFunctionPointer(toString);
            e1.CallIndirect<string>(toString.CallingConvention);
            e1.Return();

            e1.MarkLabel(l3);
            e1.LoadConstant("");
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int, string>>(out string instrs);

            Assert.Equal("123", d1(0));
            Assert.Equal("System.Object", d1(1));
            Assert.Equal("", d1(2));
            Assert.Equal("System.String", d1(314));

            Assert.Equal("ldarg.0\r\nldc.i4.0\r\nbeq.s l1\r\nldarg.0\r\nldc.i4.1\r\nbeq.s l2\r\nldarg.0\r\nldc.i4.2\r\nbeq.s l3\r\nldstr 'Foo'\r\ntail.call System.String ToString()\r\nret\r\n\r\nl1:\r\nnewobj Void .ctor()\r\ntail.callvirt System.String ToString()\r\nret\r\n\r\nl2:\r\nnewobj Void .ctor()\r\ndup\r\nldvirtftn System.String ToString()\r\ncalli Standard, HasThis System.String \r\nret\r\n\r\nl3:\r\nldstr ''\r\nret\r\n", instrs);
        }

        [Fact]
        public void PartialTypeMapping2NonGeneric()
        {
            {
                var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                var mod = asm.DefineDynamicModule("Bar");
                var t = mod.DefineType("T");
                var dictOfT = typeof(IDictionary<,>).MakeGenericType(t, t);

                var e1 = Emit.BuildMethod(typeof(bool), new [] { typeof(object), typeof(object) }, t, "E1", MethodAttributes.Public, CallingConventions.HasThis);
                e1.LoadArgument(1);
                e1.CastClass(dictOfT);
                e1.LoadArgument(2);
                e1.CallVirtual(typeof(object).GetMethod("Equals", new Type[] { typeof(object) }));
                e1.Return();

                e1.CreateMethod();

                var type = t.CreateType();

                var inst = type.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
                var e1Mtd = type.GetMethod("E1");

                Func<object, object, bool> d1 = (a, b) => (bool)e1Mtd.Invoke(inst, new object[] { a, b });

                dictOfT = typeof(Dictionary<,>).MakeGenericType(type, type);
                var cons = dictOfT.GetConstructor(Type.EmptyTypes);

                var dictInst = cons.Invoke(new object[0]);
                var listAdd = dictOfT.GetMethod("Add", new[] { type, type });
                listAdd.Invoke(dictInst, new object[] { inst, inst });

                var x = dictInst;

                Assert.True(d1(x, x));
                Assert.False(d1(x, new List<int> { 1, 2, 3 }));
            }
        }

        [Fact]
        public void PartialTypeMapping1NonGeneric()
        {
            {
                var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                var mod = asm.DefineDynamicModule("Bar");
                var t = mod.DefineType("T");
                var listOfT = typeof(List<>).MakeGenericType(t);

                var e1 = Emit.BuildMethod(typeof(bool), new [] { typeof(object), typeof(object) }, t, "E1", MethodAttributes.Public, CallingConventions.HasThis);
                e1.LoadArgument(1);
                e1.CastClass(listOfT);
                e1.LoadArgument(2);
                e1.CallVirtual(typeof(object).GetMethod("Equals", new Type[] { typeof(object) }));
                e1.Return();

                e1.CreateMethod();

                var type = t.CreateType();

                var inst = type.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
                var e1Mtd = type.GetMethod("E1");

                Func<object, object, bool> d1 = (a, b) => (bool)e1Mtd.Invoke(inst, new object[] { a, b });

                listOfT = typeof(List<>).MakeGenericType(type);
                var cons = listOfT.GetConstructor(Type.EmptyTypes);

                var listInst = cons.Invoke(new object[0]);
                var listAdd = listOfT.GetMethod("Add", new[] { type });
                listAdd.Invoke(listInst, new object[] { inst });

                var x = listInst;

                Assert.True(d1(x, x));
                Assert.False(d1(x, new List<int> { 1, 2, 3 }));
            }
        }

        [Fact]
        public void EnumParamsNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(void), new [] { typeof(int) });
            e1.LoadArgument(0);
            e1.Call(typeof(Calls).GetMethod("EnumParamsMethod"));
            e1.Return();

            var d1 = e1.CreateDelegate<Action<int>>();

            _EnumParamsMethod = null;
            d1((int)EnumParamsEnum.A);
            Assert.Equal(EnumParamsEnum.A, _EnumParamsMethod.Value);

            _EnumParamsMethod = null;
            d1((int)EnumParamsEnum.B);
            Assert.Equal(EnumParamsEnum.B, _EnumParamsMethod.Value);
        }

        [Fact]
        public void VoidStaticNonGeneric()
        {
            DoesNothingWasCalled = false;

            var mi = typeof(Calls).GetMethod("DoesNothing");

            var e1 = Emit.NewDynamicMethod(typeof(void), Type.EmptyTypes, "E1");
            e1.Call(mi);
            e1.Return();

            var del = e1.CreateDelegate<Action>();

            Assert.False(DoesNothingWasCalled);
            del();
            Assert.True(DoesNothingWasCalled);
        }

        [Fact]
        public void VoidInstanceNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes, "E1");
            e1.NewObject<VoidInstanceClass>();
            e1.Call(typeof(VoidInstanceClass).GetMethod("Go"));
            e1.Return();

            var del = e1.CreateDelegate<Func<int>>();

            Assert.Equal(314159, del());
        }

        [Fact]
        public void StringInstanceNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(string), Type.EmptyTypes, "E1");
            e1.NewObject<StringInstanceClass>();
            e1.LoadConstant(8675309);
            e1.Call(typeof(StringInstanceClass).GetMethod("Go"));
            e1.Return();

            var del = e1.CreateDelegate<Func<string>>();

            Assert.Equal("8675309", del());
        }

        [Fact]
        public void MultiParamNonGeneric()
        {
            var func = typeof(Calls).GetMethod("MultiParamFunc");
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(string), typeof(int), typeof(double) });

            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.LoadArgument(2);
            e1.Call(func);
            e1.Return();

            var d1 = e1.CreateDelegate<Func<string, int, double, int>>();

            Assert.Equal(123 + 456 + 7, d1("123", 456, 7.89));
        }

        [Fact]
        public void DynamicRecursiveNonGeneric()
        {
            var impl = Emit.NewDynamicMethod(typeof(int), new[] { typeof(int) }, "factorial");
            var lbl = impl.DefineLabel();
            impl.LoadArgument(0);
            impl.BranchIfTrue(lbl);
            impl.LoadConstant(0);
            impl.Return();
            impl.MarkLabel(lbl);
            impl.LoadArgument(0);
            impl.LoadArgument(0);
            impl.LoadConstant(1);
            impl.Subtract();
            impl.Call(impl);
            impl.Add();
            impl.Return();

            var del = impl.CreateDelegate<Func<int, int>>(out string instr);
            Assert.Equal(1, del(1));
            Assert.Equal(3, del(2));
            Assert.Equal(6, del(3));
            Assert.DoesNotContain("tail.", instr);
        }
    }
}
