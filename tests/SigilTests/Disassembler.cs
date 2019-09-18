// These only work in Release because the IL generated differs in Debug, so exact
// matches aren't gonna work
#if !DEBUG
using System;
using System.Reflection.Emit;
using System.Linq;
using System.Reflection;
using Xunit;

namespace SigilTests
{
    public class Disassembler
    {
        [Fact]
        public void Simple()
        {
            Func<int, int, int> d1 = (a, b) => a + b;

            var ops = Sigil.Disassembler<Func<int, int, int>>.Disassemble(d1);

            Assert.NotNull(ops);

            var firstOpIs0 = OpCodes.Ldarg_0 == ops[0].OpCode;

            if (firstOpIs0)
            {
                Assert.Equal(OpCodes.Ldarg_0, ops[0].OpCode);
                Assert.Equal(OpCodes.Ldarg_1, ops[1].OpCode);
                Assert.Equal(OpCodes.Add, ops[2].OpCode);
                Assert.Equal(OpCodes.Ret, ops[3].OpCode);
            }
            else
            {
                Assert.Equal(OpCodes.Ldarg_1, ops[0].OpCode);
                Assert.Equal(OpCodes.Ldarg_2, ops[1].OpCode);
                Assert.Equal(OpCodes.Add, ops[2].OpCode);
                Assert.Equal(OpCodes.Ret, ops[3].OpCode);
            }

            Assert.True(ops.CanEmit);

            var recompiled = ops.EmitAll();
            Assert.NotNull(recompiled);

            var r1 = recompiled.CreateDelegate(out string instrs);
            Assert.NotNull(r1);

            for (var i = 0; i < 200; i++)
            {
                for (var j = 0; j < 200; j++)
                {
                    Assert.Equal(d1(i, j), r1(i, j));
                }
            }

            const string Pre4_6 = "ldarg.0\r\nldarg.1\r\nadd\r\nret\r\n";
            const string Post4_6 = "ldarg.1\r\nldarg.2\r\nadd\r\nret\r\n";

            var isValid = Pre4_6 == instrs || Post4_6 == instrs;

            Assert.True(isValid);
        }

        private class _Volatile
        {
            public volatile int Foo;
        }

        [Fact]
        public void Volatile()
        {
            Action<_Volatile, int> d1 = (a, b) => { var x = a.Foo; x += b; a.Foo = b; };

            var ops = Sigil.Disassembler<Action<_Volatile, int>>.Disassemble(d1);

            var loadField = ops[1];
            Assert.Equal(OpCodes.Ldfld, loadField.OpCode);
            Assert.Equal(typeof(_Volatile).GetField("Foo"), loadField.Parameters.ElementAt(0));
            Assert.Equal(true, loadField.Parameters.ElementAt(1));
            Assert.True(ops.CanEmit);

            var e1 = ops.EmitAll();
            var r1 = e1.CreateDelegate(out string instrs);

            var v1 = new _Volatile();
            var v2 = new _Volatile();

            for (var i = 0; i < 100; i++)
            {
                d1(v1, i);
                r1(v2, i);

                Assert.Equal(v1.Foo, v2.Foo);
            }

            const string expected = @"ldarg.1
volatile.ldfld Int32 Foo
stloc.0
ldloc.0
ldarg.2
add
stloc.0
ldarg.1
ldarg.2
volatile.stfld Int32 Foo
ret
";

            Assert.Equal(expected, instrs, ignoreLineEndingDifferences: true);
        }

        [Fact]
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
            Assert.NotNull(ops);
            Assert.True(ops.CanEmit);

            var e1 = ops.EmitAll();

            var r1 = e1.CreateDelegate(out string instrs);

            for (var i = 0; i < 100; i++)
            {
                for (var j = 0; j < 100; j++)
                {
                    Assert.Equal(d1(i, j), r1(i, j));
                }
            }

            const string expected = @"ldarg.1
brtrue.s _label5
ldc.i4.m1
ret

_label5:
ldarg.2
brtrue.s _label10
ldc.i4.m1
ret

_label10:
ldarg.1
ldarg.2
mul
stloc.0
ldloc.0
ldc.i4.2
rem
brtrue.s _label21
ldc.i4.1
ret

_label21:
ldc.i4.0
ret
";

            Assert.Equal(expected, instrs, ignoreLineEndingDifferences: true);
        }

        [Fact]
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
            Assert.NotNull(ops);
            Assert.True(ops.CanEmit);

            var e1 = ops.EmitAll();

            var r1 = e1.CreateDelegate(out string instrs);

            for (var i = 0; i < 100; i++)
            {
                for (var j = 0; j < 100; j++)
                {
                    Assert.Equal(d1(i, j), r1(i, j));
                }
            }

            const string expected = @"--BeginExceptionBlock--
ldarg.1
ldarg.2
div
stloc.0
ldloca.s 0
call System.String ToString()
stloc.1
leave.s _label24

__autolabel0:
--BeginCatchBlock(System.Exception)--
stloc.2
ldloc.2
callvirt System.String get_Message()
stloc.1
leave.s _label24

__autolabel1:
--EndCatchBlock--
--EndExceptionBlock--

_label24:
ldloc.1
ret
";

            Assert.Equal(expected, instrs, ignoreLineEndingDifferences: true);
        }

        [Fact]
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
            Assert.NotNull(ops);
            Assert.True(ops.CanEmit);

            var e1 = ops.EmitAll();

            var r1 = e1.CreateDelegate(out string instrs);

            for (var i = 1; i < 100; i++)
            {
                for (var j = 1; j < 100; j++)
                {
                    Assert.Equal(d1(i, j), r1(i, j));
                }
            }

            const string Pre4_6 = "ldc.i4.0\r\nstloc.0\r\n--BeginExceptionBlock--\r\nldarg.0\r\nldarg.1\r\ndiv\r\nstloc.0\r\nldloc.0\r\nldc.i4.1\r\nadd\r\nstloc.0\r\nleave.s _label17\r\n\r\n__autolabel0:\r\n--BeginFinallyBlock--\r\nldloc.0\r\nldc.i4.2\r\nmul\r\nstloc.0\r\n--EndFinallyBlock--\r\n--EndExceptionBlock--\r\n\r\n_label17:\r\nldloc.0\r\nconv.r8\r\nret\r\n";
            const string Post4_6 = "ldc.i4.0\r\nstloc.0\r\n--BeginExceptionBlock--\r\nldarg.1\r\nldarg.2\r\ndiv\r\nstloc.0\r\nldloc.0\r\nldc.i4.1\r\nadd\r\nstloc.0\r\nleave.s _label17\r\n\r\n__autolabel0:\r\n--BeginFinallyBlock--\r\nldloc.0\r\nldc.i4.2\r\nmul\r\nstloc.0\r\n--EndFinallyBlock--\r\n--EndExceptionBlock--\r\n\r\n_label17:\r\nldloc.0\r\nconv.r8\r\nret\r\n";

            var isValid = instrs == Pre4_6 || instrs == Post4_6;

            Assert.True(isValid, instrs);
        }

        [Fact]
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
            Assert.NotNull(ops);
            Assert.True(ops.CanEmit);

            var e1 = ops.EmitAll();

            var r1 = e1.CreateDelegate(out string instrs);

            for (var i = 0; i < 100; i++)
            {
                for (var j = 0; j < 100; j++)
                {
                    Assert.Equal(d1(i, j), r1(i, j));
                }
            }

            const string Pre4_6 = "ldc.i4.0\r\nstloc.0\r\n--BeginExceptionBlock--\r\n--BeginExceptionBlock--\r\nldarg.0\r\nldarg.1\r\ndiv\r\nstloc.0\r\nldloc.0\r\nldc.i4.1\r\nadd\r\nstloc.0\r\nleave.s _label17\r\n\r\n__autolabel0:\r\n--BeginCatchBlock(System.Exception)--\r\npop\r\nldc.i4.m1\r\nstloc.0\r\nleave.s _label17\r\n\r\n__autolabel1:\r\n--EndCatchBlock--\r\n--EndExceptionBlock--\r\n\r\n_label17:\r\nleave.s _label24\r\n\r\n__autolabel2:\r\n--BeginFinallyBlock--\r\nldloc.0\r\nldc.i4.2\r\nmul\r\nstloc.0\r\n--EndFinallyBlock--\r\n--EndExceptionBlock--\r\n\r\n_label24:\r\nldloc.0\r\nconv.r8\r\nret\r\n";
            const string Post4_6 = "ldc.i4.0\r\nstloc.0\r\n--BeginExceptionBlock--\r\n--BeginExceptionBlock--\r\nldarg.1\r\nldarg.2\r\ndiv\r\nstloc.0\r\nldloc.0\r\nldc.i4.1\r\nadd\r\nstloc.0\r\nleave.s _label22\r\n\r\n__autolabel0:\r\n--BeginCatchBlock(System.Exception)--\r\npop\r\nldc.i4.m1\r\nstloc.0\r\nleave.s _label22\r\n\r\n__autolabel1:\r\n--EndCatchBlock--\r\n--EndExceptionBlock--\r\n--BeginFinallyBlock--\r\nldloc.0\r\nldc.i4.2\r\nmul\r\nstloc.0\r\n--EndFinallyBlock--\r\n--EndExceptionBlock--\r\n\r\n_label22:\r\nldloc.0\r\nconv.r8\r\nret\r\n";

            var isValid = instrs == Pre4_6 || instrs == Post4_6;

            Assert.True(isValid, instrs);
        }

        [Fact]
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
            Assert.NotNull(ops);
            Assert.True(ops.CanEmit);

            var e1 = ops.EmitAll();

            var r1 = e1.CreateDelegate(out string instrs);

            for (var i = 0; i < 100; i++)
            {
                for (var j = 0; j < 100; j++)
                {
                    Assert.Equal(d1(i, j), r1(i, j));
                }
            }

            const string expected = @"ldstr ''
stloc.0
--BeginExceptionBlock--
--BeginExceptionBlock--
ldarg.1
conv.r8
ldc.r8 3
call Double Pow(Double, Double)
stloc.1
--BeginExceptionBlock--
ldarg.1
ldarg.1
mul
ldarg.1
sub
stloc.2
ldloc.2
ldarg.1
ldc.i4.1
sub
div
stloc.3
ldloca.s 3
call System.String ToString()
stloc.s 4
leave.s _label116

__autolabel0:
--BeginCatchBlock(System.Exception)--
stloc.s 5
ldstr 'foo - '
ldloc.s 5
callvirt System.String get_Message()
call System.String Concat(System.String, System.String)
newobj Void .ctor(System.String)
throw

__autolabel1:
--EndCatchBlock--
--EndExceptionBlock--
--BeginCatchBlock(System.Exception)--
stloc.s 6
ldloc.s 6
callvirt System.String get_Message()
ldloc.s 6
callvirt System.String get_Message()
call System.String Concat(System.String, System.String)
stloc.0
leave.s _label114

__autolabel2:
--EndCatchBlock--
--EndExceptionBlock--
--BeginFinallyBlock--
ldarg.2
ldc.i4.2
rem
brtrue.s _label113
--BeginExceptionBlock--
ldloc.0
ldloc.0
call System.String Concat(System.String, System.String)
stloc.0
leave.s _label113

__autolabel3:
--BeginCatchBlock(System.Exception)--
pop
leave.s _label113

__autolabel4:
--EndCatchBlock--
--EndExceptionBlock--

_label113:
--EndFinallyBlock--
--EndExceptionBlock--

_label114:
ldloc.0
ret

_label116:
ldloc.s 4
ret
";

            Assert.Equal(expected, instrs, ignoreLineEndingDifferences: true);
        }

        [Fact]
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
            Assert.NotNull(ops);
            Assert.True(ops.CanEmit);

            var e1 = ops.EmitAll();

            var r1 = e1.CreateDelegate(out string instrs);

            var a1 = new[] { 1, 2, 3, 4, 5, 6 };
            var a2 = new[] { 1, 2, 3, 4, 5, 6 };

            for (var i = 0; i < 100; i++)
            {
                Assert.Equal(d1(a1, i), r1(a2, i));
                Assert.True(a1.SequenceEqual(a2));
            }

            const string Pre4_6 = "ldarg.1\r\nldarg.0\r\nldlen\r\nconv.i4\r\nrem\r\nstarg.s 1\r\nldarg.0\r\nldarg.1\r\nldelem.i4\r\nstloc.0\r\nldarg.0\r\nldarg.1\r\nldloc.0\r\nldc.i4.1\r\nsub\r\nstelem.i4\r\nldloc.0\r\nret\r\n";
            const string Post4_6 = "ldarg.2\r\nldarg.1\r\nldlen\r\nconv.i4\r\nrem\r\nstarg.s 2\r\nldarg.1\r\nldarg.2\r\nldelem.i4\r\nstloc.0\r\nldarg.1\r\nldarg.2\r\nldloc.0\r\nldc.i4.1\r\nsub\r\nstelem.i4\r\nldloc.0\r\nret\r\n";

            var isValid = instrs == Pre4_6 || instrs == Post4_6;

            Assert.True(isValid, instrs);
        }

        [Fact]
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
            Assert.NotNull(ops);
            Assert.True(ops.CanEmit);

            var e1 = ops.EmitAll();

            var r1 = e1.CreateDelegate(out string instrs);

            var a1 = new[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten" };
            var a2 = new[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten" };

            for (var i = 0; i < 100; i++)
            {
                Assert.Equal(d1(a1, i), r1(a2, i));
                Assert.True(a1.SequenceEqual(a2));
            }

            const string Pre4_6 = "ldarg.1\r\nldarg.0\r\nldlen\r\nconv.i4\r\nrem\r\nstarg.s 1\r\nldarg.0\r\nldarg.1\r\nldelem.ref\r\nstloc.0\r\nldloc.0\r\ncallvirt Int32 get_Length()\r\nldc.i4.0\r\nble.s _label40\r\nldarg.0\r\nldarg.1\r\nldloc.0\r\nldc.i4.0\r\nldloc.0\r\ncallvirt Int32 get_Length()\r\nldc.i4.1\r\nsub\r\ncallvirt System.String Substring(Int32, Int32)\r\nstelem.ref\r\nbr.s _label48\r\n\r\n_label40:\r\nldarg.0\r\nldarg.1\r\nldstr 'hello world'\r\nstelem.ref\r\n\r\n_label48:\r\nldloc.0\r\nret\r\n";
            const string Post4_6 = "ldarg.2\r\nldarg.1\r\nldlen\r\nconv.i4\r\nrem\r\nstarg.s 2\r\nldarg.1\r\nldarg.2\r\nldelem.ref\r\nstloc.0\r\nldloc.0\r\ncallvirt Int32 get_Length()\r\nldc.i4.0\r\nble.s _label40\r\nldarg.1\r\nldarg.2\r\nldloc.0\r\nldc.i4.0\r\nldloc.0\r\ncallvirt Int32 get_Length()\r\nldc.i4.1\r\nsub\r\ncallvirt System.String Substring(Int32, Int32)\r\nstelem.ref\r\nbr.s _label48\r\n\r\n_label40:\r\nldarg.1\r\nldarg.2\r\nldstr 'hello world'\r\nstelem.ref\r\n\r\n_label48:\r\nldloc.0\r\nret\r\n";

            var isValid = instrs == Pre4_6 || instrs == Post4_6;

            Assert.True(isValid, instrs);
        }

        private delegate string LoadAndStoreIndictDel(ref string arr, int at);

        [Fact]
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
            Assert.NotNull(ops);
            Assert.True(ops.CanEmit);

            var e1 = ops.EmitAll();

            var r1 = e1.CreateDelegate(out string instrs);

            var a1 = "ab";
            var a2 = "ab";

            for (var i = 0; i < 10; i++)
            {
                Assert.Equal(d1(ref a1, i), r1(ref a2, i));
                Assert.Equal(a1, a2);
            }

            const string Pre4_6 = "ldarg.0\r\nldind.ref\r\nstloc.0\r\nldc.i4.0\r\nstloc.1\r\nbr.s _label21\r\n\r\n_label7:\r\nldarg.0\r\nldarg.0\r\nldind.ref\r\nldloc.0\r\ncall System.String Concat(System.String, System.String)\r\nstind.ref\r\nldloc.1\r\nldc.i4.1\r\nadd\r\nstloc.1\r\n\r\n_label21:\r\nldloc.1\r\nldarg.1\r\nblt.s _label7\r\nldarg.0\r\nldind.ref\r\nret\r\n";
            const string Post4_6 = "ldarg.1\r\nldind.ref\r\nstloc.0\r\nldc.i4.0\r\nstloc.1\r\nbr.s _label21\r\n\r\n_label7:\r\nldarg.1\r\nldarg.1\r\nldind.ref\r\nldloc.0\r\ncall System.String Concat(System.String, System.String)\r\nstind.ref\r\nldloc.1\r\nldc.i4.1\r\nadd\r\nstloc.1\r\n\r\n_label21:\r\nldloc.1\r\nldarg.2\r\nblt.s _label7\r\nldarg.1\r\nldind.ref\r\nret\r\n";

            var isValid = instrs == Pre4_6 || instrs == Post4_6;

            Assert.True(isValid, instrs);
        }

        private class ComplexClass
        {
            public int Key { get; set; }
            public string Value { get; set; }
            public long NotUsed { get; set; }
        }

        [Fact]
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

            Assert.Equal(2, propAccess.Count);
            Assert.False(ops.CanEmit);
        }

        [Fact]
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

            Assert.Contains(methods, m => m == typeof(int).GetMethod("Parse", new[] { typeof(string) }));
            Assert.Contains(methods, m => m == typeof(Math).GetMethod("Pow", new[] { typeof(double), typeof(double) }));
            Assert.Equal(2, methods.Count);
            Assert.True(ops.CanEmit);
        }

        [Fact]
        public void SingleFloat()
        {
            Func<float> del = () => 0.5f;

            var ops = Sigil.Disassembler<Func<float>>.Disassemble(del);

            var recompiled = ops.EmitAll();
            Assert.NotNull(recompiled);

            var r1 = recompiled.CreateDelegate(out string instrs);
            Assert.NotNull(r1);
            Assert.Equal("ldc.r4 0.5\r\nret\r\n", instrs);
        }
    }
}
#endif