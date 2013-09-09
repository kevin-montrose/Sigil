using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection.Emit;
using System.Linq;
using System.Reflection;
using System.IO;

namespace SigilTests
{
    [TestClass, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Disassembler
    {
        [TestMethod]
        public void Simple()
        {
            Func<int, int, int> d1 = (a, b) => a + b;

            var ops = Sigil.Disassembler<Func<int, int, int>>.Disassemble(d1);

            Assert.IsNotNull(ops);
            Assert.AreEqual(OpCodes.Ldarg_0, ops[0].OpCode);
            Assert.AreEqual(OpCodes.Ldarg_1, ops[1].OpCode);
            Assert.AreEqual(OpCodes.Add, ops[2].OpCode);
            Assert.AreEqual(OpCodes.Ret, ops[3].OpCode);
            Assert.IsTrue(ops.CanEmit);

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

        class _Volatile
        {
            public volatile int Foo;
        }

        [TestMethod]
        public void Volatile()
        {
            Action<_Volatile, int> d1 = (a, b) => { var x = a.Foo; x += b; a.Foo = b; };

            var ops = Sigil.Disassembler<Action<_Volatile, int>>.Disassemble(d1);

            var loadField = ops[1];
            Assert.AreEqual(OpCodes.Ldfld, loadField.OpCode);
            Assert.AreEqual(typeof(_Volatile).GetField("Foo"), loadField.Parameters.ElementAt(0));
            Assert.AreEqual(true, loadField.Parameters.ElementAt(1));
            Assert.IsTrue(ops.CanEmit);

            var e1 = ops.EmitAll();
            string instrs;
            var r1 = e1.CreateDelegate(out instrs);

            var v1 = new _Volatile();
            var v2 = new _Volatile();

            for (var i = 0; i < 100; i++)
            {
                d1(v1, i);
                r1(v2, i);

                Assert.AreEqual(v1.Foo, v2.Foo);
            }

            Assert.AreEqual("ldarg.0\r\nvolatile.ldfld Int32 Foo\r\nstloc.0\r\nldloc.0\r\nldarg.1\r\nadd\r\nstloc.0\r\nldarg.0\r\nldarg.1\r\nvolatile.stfld Int32 Foo\r\nret\r\n", instrs);
        }

        [TestMethod]
        public void Branch()
        {
            Func<int, int, int> d1 = (a, b) =>
            {
                if (a == 0) return -1;
                if (b == 0) return -1;

                var c = a * b;

                if (c % 2 == 0) return 1;
                
                return 0;
            };

            var ops = Sigil.Disassembler<Func<int, int, int>>.Disassemble(d1);
            Assert.IsNotNull(ops);
            Assert.IsTrue(ops.CanEmit);

            var e1 = ops.EmitAll();

            string instrs;
            var r1 = e1.CreateDelegate(out instrs);

            for (var i = 0; i < 100; i++)
            {
                for (var j = 0; j < 100; j++)
                {
                    Assert.AreEqual(d1(i, j), r1(i, j));
                }
            }

            Assert.AreEqual("ldarg.0\r\nbrtrue.s _label5\r\nldc.i4.m1\r\nret\r\n\r\n_label5:\r\nldarg.1\r\nbrtrue.s _label10\r\nldc.i4.m1\r\nret\r\n\r\n_label10:\r\nldarg.0\r\nldarg.1\r\nmul\r\nstloc.0\r\nldloc.0\r\nldc.i4.2\r\nrem\r\nbrtrue.s _label21\r\nldc.i4.1\r\nret\r\n\r\n_label21:\r\nldc.i4.0\r\nret\r\n", instrs);
        }

        [TestMethod]
        public void TryCatch()
        {
            Func<int, int, string> d1 =
                (a,b) =>
                {
                    try
                    {
                        var c = a / b;
                        return c.ToString();
                    }
                    catch (Exception f)
                    {
                        return f.Message;
                    }
                };

            var ops = Sigil.Disassembler<Func<int, int, string>>.Disassemble(d1);
            Assert.IsNotNull(ops);
            Assert.IsTrue(ops.CanEmit);

            var e1 = ops.EmitAll();

            string instrs;
            var r1 = e1.CreateDelegate(out instrs);

            for (var i = 0; i < 100; i++)
            {
                for (var j = 0; j < 100; j++)
                {
                    Assert.AreEqual(d1(i, j), r1(i, j));
                }
            }

            Assert.AreEqual("--BeginExceptionBlock--\r\nldarg.0\r\nldarg.1\r\ndiv\r\nstloc.0\r\nldloca.s 0\r\ncall System.String ToString()\r\nstloc.2\r\nleave.s _label24\r\n\r\n__autolabel0:\r\n--BeginCatchBlock(System.Exception)--\r\nstloc.1\r\nldloc.1\r\ncallvirt System.String get_Message()\r\nstloc.2\r\nleave.s _label24\r\n\r\n__autolabel1:\r\n--EndCatchBlock--\r\n--EndExceptionBlock--\r\n\r\n_label24:\r\nldloc.2\r\nret\r\n", instrs);
        }

        [TestMethod]
        public void TryFinally()
        {
            Func<int, int, double> d1 =
                (a, b) =>
                {
                    int d = 0;

                    try
                    {
                        d = a / b;
                        d++;
                    }
                    finally
                    {
                        d *= 2;
                    }

                    return d;
                };

            var ops = Sigil.Disassembler<Func<int, int, double>>.Disassemble(d1);
            Assert.IsNotNull(ops);
            Assert.IsTrue(ops.CanEmit);

            var e1 = ops.EmitAll();

            string instrs;
            var r1 = e1.CreateDelegate(out instrs);

            for (var i = 1; i < 100; i++)
            {
                for (var j = 1; j < 100; j++)
                {
                    Assert.AreEqual(d1(i, j), r1(i, j));
                }
            }

            Assert.AreEqual("ldc.i4.0\r\nstloc.0\r\n--BeginExceptionBlock--\r\nldarg.0\r\nldarg.1\r\ndiv\r\nstloc.0\r\nldloc.0\r\nldc.i4.1\r\nadd\r\nstloc.0\r\nleave.s _label17\r\n\r\n__autolabel0:\r\n--BeginFinallyBlock--\r\nldloc.0\r\nldc.i4.2\r\nmul\r\nstloc.0\r\n--EndFinallyBlock--\r\n--EndExceptionBlock--\r\n\r\n_label17:\r\nldloc.0\r\nconv.r8\r\nret\r\n", instrs);
        }

        [TestMethod]
        public void TryCatchFinally()
        {
            Func<int, int, double> d1 =
                (a, b) =>
                {
                    int d = 0;

                    try
                    {
                        d = a / b;
                        d++;
                    }
                    catch (Exception)
                    {
                        d = -1;
                    }
                    finally
                    {
                        d *= 2;
                    }

                    return d;
                };

            var ops = Sigil.Disassembler<Func<int, int, double>>.Disassemble(d1);
            Assert.IsNotNull(ops);
            Assert.IsTrue(ops.CanEmit);

            var e1 = ops.EmitAll();

            string instrs;
            var r1 = e1.CreateDelegate(out instrs);

            for (var i = 0; i < 100; i++)
            {
                for (var j = 0; j < 100; j++)
                {
                    Assert.AreEqual(d1(i, j), r1(i, j));
                }
            }

            Assert.AreEqual("ldc.i4.0\r\nstloc.0\r\n--BeginExceptionBlock--\r\n--BeginExceptionBlock--\r\nldarg.0\r\nldarg.1\r\ndiv\r\nstloc.0\r\nldloc.0\r\nldc.i4.1\r\nadd\r\nstloc.0\r\nleave.s _label17\r\n\r\n__autolabel0:\r\n--BeginCatchBlock(System.Exception)--\r\npop\r\nldc.i4.m1\r\nstloc.0\r\nleave.s _label17\r\n\r\n__autolabel1:\r\n--EndCatchBlock--\r\n--EndExceptionBlock--\r\n\r\n_label17:\r\nleave.s _label24\r\n\r\n__autolabel2:\r\n--BeginFinallyBlock--\r\nldloc.0\r\nldc.i4.2\r\nmul\r\nstloc.0\r\n--EndFinallyBlock--\r\n--EndExceptionBlock--\r\n\r\n_label24:\r\nldloc.0\r\nconv.r8\r\nret\r\n", instrs);
        }

        [TestMethod]
        public void ComplicatedTryCatchFinally()
        {
            Func<int, int, string> d1 =
                (a, x) =>
                {
                    string lateRet = "";
                    try
                    {

                        var c = Math.Pow(a, 3);
                        try
                        {
                            var b = a * a - a;

                            return (b / (a - 1)).ToString();
                        }
                        catch (Exception e)
                        {
                            throw new Exception("foo - " + e.Message);
                        }

                    }
                    catch (Exception f)
                    {
                        lateRet = f.Message + f.Message;
                    }
                    finally
                    {
                        if (x % 2 == 0)
                        {
                            try
                            {
                                lateRet += lateRet;
                            }
                            catch(Exception) { }
                        }
                    }

                    return lateRet;
                };

            var ops = Sigil.Disassembler<Func<int, int, string>>.Disassemble(d1);
            Assert.IsNotNull(ops);
            Assert.IsTrue(ops.CanEmit);

            var e1 = ops.EmitAll();

            string instrs;
            var r1 = e1.CreateDelegate(out instrs);

            for (var i = 0; i < 100; i++)
            {
                for (var j = 0; j < 100; j++)
                {
                    Assert.AreEqual(d1(i, j), r1(i, j));
                }
            }

            Assert.AreEqual("ldstr ''\r\nstloc.0\r\n--BeginExceptionBlock--\r\n--BeginExceptionBlock--\r\nldarg.0\r\nconv.r8\r\nldc.r8 3\r\ncall Double Pow(Double, Double)\r\npop\r\n--BeginExceptionBlock--\r\nldarg.0\r\nldarg.0\r\nmul\r\nldarg.0\r\nsub\r\nstloc.1\r\nldloc.1\r\nldarg.0\r\nldc.i4.1\r\nsub\r\ndiv\r\nstloc.s 5\r\nldloca.s 5\r\ncall System.String ToString()\r\nstloc.s 4\r\nleave.s _label114\r\n\r\n__autolabel0:\r\n--BeginCatchBlock(System.Exception)--\r\nstloc.2\r\nldstr 'foo - '\r\nldloc.2\r\ncallvirt System.String get_Message()\r\ncall System.String Concat(System.String, System.String)\r\nnewobj Void .ctor(System.String)\r\nthrow\r\n--EndCatchBlock--\r\n--EndExceptionBlock--\r\n--BeginCatchBlock(System.Exception)--\r\nstloc.3\r\nldloc.3\r\ncallvirt System.String get_Message()\r\nldloc.3\r\ncallvirt System.String get_Message()\r\ncall System.String Concat(System.String, System.String)\r\nstloc.0\r\nleave.s _label91\r\n\r\n__autolabel1:\r\n--EndCatchBlock--\r\n--EndExceptionBlock--\r\n\r\n_label91:\r\nleave.s _label112\r\n\r\n__autolabel2:\r\n--BeginFinallyBlock--\r\nldarg.1\r\nldc.i4.2\r\nrem\r\nbrtrue.s _label111\r\n--BeginExceptionBlock--\r\nldloc.0\r\nldloc.0\r\ncall System.String Concat(System.String, System.String)\r\nstloc.0\r\nleave.s _label111\r\n\r\n__autolabel3:\r\n--BeginCatchBlock(System.Exception)--\r\npop\r\nleave.s _label111\r\n\r\n__autolabel4:\r\n--EndCatchBlock--\r\n--EndExceptionBlock--\r\n\r\n_label111:\r\n--EndFinallyBlock--\r\n--EndExceptionBlock--\r\n\r\n_label112:\r\nldloc.0\r\nret\r\n\r\n_label114:\r\nldloc.s 4\r\nret\r\n", instrs);
        }

        [TestMethod]
        public void LoadLength()
        {
            Func<int[], int, int> d1 =
                (a, ix) =>
                {
                    ix %= a.Length;

                    var b = a[ix];

                    a[ix] = b - 1;

                    return b;
                };

            var ops = Sigil.Disassembler<Func<int[], int, int>>.Disassemble(d1);
            Assert.IsNotNull(ops);
            Assert.IsTrue(ops.CanEmit);

            var e1 = ops.EmitAll();

            string instrs;
            var r1 = e1.CreateDelegate(out instrs);

            var a1 = new[] { 1, 2, 3, 4, 5, 6 };
            var a2 = new[] { 1, 2, 3, 4, 5, 6 };

            for (var i = 0; i < 100; i++)
            {
                Assert.AreEqual(d1(a1, i), r1(a2, i));
                Assert.IsTrue(a1.SequenceEqual(a2));
            }

            Assert.AreEqual("ldarg.1\r\nldarg.0\r\nldlen\r\nconv.i4\r\nrem\r\nstarg.s 1\r\nldarg.0\r\nldarg.1\r\nldelem.i4\r\nstloc.0\r\nldarg.0\r\nldarg.1\r\nldloc.0\r\nldc.i4.1\r\nsub\r\nstelem.i4\r\nldloc.0\r\nret\r\n", instrs);
        }

        [TestMethod]
        public void LoadAndStoreElement()
        {
            Func<string[], int, string> d1 =
                (a, ix) =>
                {
                    ix %= a.Length;

                    var b = a[ix];

                    if (b.Length > 0)
                    {
                        a[ix] = b.Substring(0, b.Length - 1);
                    }
                    else
                    {
                        a[ix] = "hello world";
                    }

                    return b;
                };

            var ops = Sigil.Disassembler<Func<string[], int, string>>.Disassemble(d1);
            Assert.IsNotNull(ops);
            Assert.IsTrue(ops.CanEmit);

            var e1 = ops.EmitAll();

            string instrs;
            var r1 = e1.CreateDelegate(out instrs);

            var a1 = new[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten" };
            var a2 = new[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten" };

            for (var i = 0; i < 100; i++)
            {
                Assert.AreEqual(d1(a1, i), r1(a2, i));
                Assert.IsTrue(a1.SequenceEqual(a2));
            }

            Assert.AreEqual("ldarg.1\r\nldarg.0\r\nldlen\r\nconv.i4\r\nrem\r\nstarg.s 1\r\nldarg.0\r\nldarg.1\r\nldelem.ref\r\nstloc.0\r\nldloc.0\r\ncallvirt Int32 get_Length()\r\nldc.i4.0\r\nble.s _label40\r\nldarg.0\r\nldarg.1\r\nldloc.0\r\nldc.i4.0\r\nldloc.0\r\ncallvirt Int32 get_Length()\r\nldc.i4.1\r\nsub\r\ncallvirt System.String Substring(Int32, Int32)\r\nstelem.ref\r\nbr.s _label48\r\n\r\n_label40:\r\nldarg.0\r\nldarg.1\r\nldstr 'hello world'\r\nstelem.ref\r\n\r\n_label48:\r\nldloc.0\r\nret\r\n", instrs);
        }

        delegate string LoadAndStoreIndictDel(ref string arr, int at);

        [TestMethod]
        public void LoadAndStoreIndict()
        {
            LoadAndStoreIndictDel d1 =
                delegate(ref string str, int ix)
                {
                    var copy = str;

                    for (var i = 0; i < ix; i++)
                    {
                        str = str + copy;
                    }

                    return str;
                };

            var ops = Sigil.Disassembler<LoadAndStoreIndictDel>.Disassemble(d1);
            Assert.IsNotNull(ops);
            Assert.IsTrue(ops.CanEmit);

            var e1 = ops.EmitAll();

            string instrs;
            var r1 = e1.CreateDelegate(out instrs);

            var a1 = "ab";
            var a2 = "ab";

            for (var i = 0; i < 10; i++)
            {
                Assert.AreEqual(d1(ref a1, i), r1(ref a2, i));
                Assert.AreEqual(a1, a2);
            }

            Assert.AreEqual("ldarg.0\r\nldind.ref\r\nstloc.0\r\nldc.i4.0\r\nstloc.1\r\nbr.s _label21\r\n\r\n_label7:\r\nldarg.0\r\nldarg.0\r\nldind.ref\r\nldloc.0\r\ncall System.String Concat(System.String, System.String)\r\nstind.ref\r\nldloc.1\r\nldc.i4.1\r\nadd\r\nstloc.1\r\n\r\n_label21:\r\nldloc.1\r\nldarg.1\r\nblt.s _label7\r\nldarg.0\r\nldind.ref\r\nret\r\n", instrs);
        }

        class ComplexClass
        {
            public int Key { get; set; }
            public string Value { get; set; }
            public long NotUsed { get; set; }
        }

        [TestMethod]
        public void Complex()
        {
            var rand = new Random();
            var bs = new byte[16];
            rand.NextBytes(bs);

            var a = 
                new ComplexClass
                {
                    Key = rand.Next(),
                    Value = Convert.ToBase64String(bs)
                };

            Action closeOver =
                () =>
                {
                    var cs = new byte[16];
                    rand.NextBytes(cs);

                    a.Key = rand.Next();
                    a.Value = Convert.ToBase64String(cs);
                };

            var ops = Sigil.Disassembler<Action>.Disassemble(closeOver);
            var usage = ops.Usage;

            var propAccess = 
                usage
                    .Where(w =>
                        (w.ProducesResult.OpCode == OpCodes.Call || w.ProducesResult.OpCode == OpCodes.Callvirt) &&
                        ((MethodInfo)w.ProducesResult.Parameters.ElementAt(0)).DeclaringType == typeof(ComplexClass)
                    ).ToList();

            Assert.AreEqual(2, propAccess.Count);
            Assert.IsFalse(ops.CanEmit);
        }

        [TestMethod]
        public void UsedMethods()
        {
            Func<string, int> del =
                str =>
                {
                    var i = int.Parse(str);
                    return (int)Math.Pow(2, i);
                };

            var ops = Sigil.Disassembler<Func<string, int>>.Disassemble(del);

            var calls = ops.Where(o => o.IsOpCode && new[] { OpCodes.Call, OpCodes.Callvirt }.Contains(o.OpCode)).ToList();
            var methods = calls.Select(c => c.Parameters.ElementAt(0)).Cast<MethodInfo>().ToList();

            Assert.IsTrue(methods.Any(m => m == typeof(int).GetMethod("Parse", new[] { typeof(string) })));
            Assert.IsTrue(methods.Any(m => m == typeof(Math).GetMethod("Pow", new[] { typeof(double), typeof(double) })));
            Assert.AreEqual(2, methods.Count);
            Assert.IsTrue(ops.CanEmit);
        }
    }
}
