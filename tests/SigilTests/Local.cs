using Sigil;
using System;
using System.Collections.Generic;
using Xunit;

namespace SigilTests
{
    public partial class Local
    {
        [Fact]
        public void ReuseLabels()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            e1.DeclareLocal<int>("a");
            e1.DeclareLocal<int>("b");

            e1.LoadLocal("a");
            e1.LoadLocal("b");

            using (e1.DeclareLocal<int>("c"))
            {
                e1.LoadLocal("c");
            }

            using (e1.DeclareLocal<int>("d"))
            {
                e1.LoadLocal("d");
            }

            e1.StoreLocal("a");
            e1.StoreLocal("a");
            e1.StoreLocal("b");
            e1.StoreLocal("b");

            var ex = Assert.Throws<SigilVerificationException>(() => e1.Pop());
            var debug = ex.GetDebugInfo();
            Assert.Equal("Stack\r\n=====\r\n--empty--\r\n\r\nInstructions\r\n============\r\nldloc.0 // System.Int32 a\r\nldloc.1 // System.Int32 b\r\nldloc.2 // System.Int32 c\r\nldc.i4.0\r\nstloc.2 // System.Int32 d\r\nldloc.2 // System.Int32 d\r\nstloc.0 // System.Int32 a\r\nstloc.0 // System.Int32 a\r\nstloc.1 // System.Int32 b\r\nstloc.1 // System.Int32 b\r\n", debug);
        }

        [Fact]
        public void Instructions()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");

            for (var c = 'A'; c <= 'Z'; c++)
            {
                var loc = e1.DeclareLocal<int>("" + c);
                e1.LoadConstant((int)c);
                e1.StoreLocal(loc);
                e1.LoadLocal(loc);
                e1.Pop();
            }

            e1.Return();

            e1.CreateDelegate();

            string instrs = e1.Instructions();

            Assert.Equal("ldc.i4.s 65\r\nstloc.0 // System.Int32 A\r\nldloc.0 // System.Int32 A\r\npop\r\n\r\nldc.i4.s 66\r\nstloc.1 // System.Int32 B\r\nldloc.1 // System.Int32 B\r\npop\r\n\r\nldc.i4.s 67\r\nstloc.2 // System.Int32 C\r\nldloc.2 // System.Int32 C\r\npop\r\n\r\nldc.i4.s 68\r\nstloc.3 // System.Int32 D\r\nldloc.3 // System.Int32 D\r\npop\r\n\r\nldc.i4.s 69\r\nstloc.s 4 // System.Int32 E\r\nldloc.s 4 // System.Int32 E\r\npop\r\n\r\nldc.i4.s 70\r\nstloc.s 5 // System.Int32 F\r\nldloc.s 5 // System.Int32 F\r\npop\r\n\r\nldc.i4.s 71\r\nstloc.s 6 // System.Int32 G\r\nldloc.s 6 // System.Int32 G\r\npop\r\n\r\nldc.i4.s 72\r\nstloc.s 7 // System.Int32 H\r\nldloc.s 7 // System.Int32 H\r\npop\r\n\r\nldc.i4.s 73\r\nstloc.s 8 // System.Int32 I\r\nldloc.s 8 // System.Int32 I\r\npop\r\n\r\nldc.i4.s 74\r\nstloc.s 9 // System.Int32 J\r\nldloc.s 9 // System.Int32 J\r\npop\r\n\r\nldc.i4.s 75\r\nstloc.s 10 // System.Int32 K\r\nldloc.s 10 // System.Int32 K\r\npop\r\n\r\nldc.i4.s 76\r\nstloc.s 11 // System.Int32 L\r\nldloc.s 11 // System.Int32 L\r\npop\r\n\r\nldc.i4.s 77\r\nstloc.s 12 // System.Int32 M\r\nldloc.s 12 // System.Int32 M\r\npop\r\n\r\nldc.i4.s 78\r\nstloc.s 13 // System.Int32 N\r\nldloc.s 13 // System.Int32 N\r\npop\r\n\r\nldc.i4.s 79\r\nstloc.s 14 // System.Int32 O\r\nldloc.s 14 // System.Int32 O\r\npop\r\n\r\nldc.i4.s 80\r\nstloc.s 15 // System.Int32 P\r\nldloc.s 15 // System.Int32 P\r\npop\r\n\r\nldc.i4.s 81\r\nstloc.s 16 // System.Int32 Q\r\nldloc.s 16 // System.Int32 Q\r\npop\r\n\r\nldc.i4.s 82\r\nstloc.s 17 // System.Int32 R\r\nldloc.s 17 // System.Int32 R\r\npop\r\n\r\nldc.i4.s 83\r\nstloc.s 18 // System.Int32 S\r\nldloc.s 18 // System.Int32 S\r\npop\r\n\r\nldc.i4.s 84\r\nstloc.s 19 // System.Int32 T\r\nldloc.s 19 // System.Int32 T\r\npop\r\n\r\nldc.i4.s 85\r\nstloc.s 20 // System.Int32 U\r\nldloc.s 20 // System.Int32 U\r\npop\r\n\r\nldc.i4.s 86\r\nstloc.s 21 // System.Int32 V\r\nldloc.s 21 // System.Int32 V\r\npop\r\n\r\nldc.i4.s 87\r\nstloc.s 22 // System.Int32 W\r\nldloc.s 22 // System.Int32 W\r\npop\r\n\r\nldc.i4.s 88\r\nstloc.s 23 // System.Int32 X\r\nldloc.s 23 // System.Int32 X\r\npop\r\n\r\nldc.i4.s 89\r\nstloc.s 24 // System.Int32 Y\r\nldloc.s 24 // System.Int32 Y\r\npop\r\n\r\nldc.i4.s 90\r\nstloc.s 25 // System.Int32 Z\r\nldloc.s 25 // System.Int32 Z\r\npop\r\nret", instrs);
        }

        [Fact]
        public void Name()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            var local = e1.DeclareLocal<int>("local");

            Assert.Equal("System.Int32 local", local.ToString());
        }

        [Fact]
        public void Reuse()
        {
            var e1 = Emit<Func<int>>.NewDynamicMethod();

            using (var a = e1.DeclareLocal<int>("a"))
            {
                e1.LoadLocal(a);
                e1.LoadConstant(1);
                e1.Add();
            }

            using (var b = e1.DeclareLocal<int>("b"))
            {
                e1.StoreLocal(b);
                e1.LoadLocal(b);
                e1.Return();
            }

            var d1 = e1.CreateDelegate(out string instrs);

            Assert.Equal(1, d1());
            Assert.Equal("ldloc.0\r\nldc.i4.1\r\nadd\r\nldc.i4.0\r\nstloc.0\r\nstloc.0\r\nldloc.0\r\nret\r\n", instrs);
        }

        [Fact]
        public void Lookup()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            var a = e1.DeclareLocal<int>("a");

            var aRef = e1.Locals["a"];

            Assert.True(a == aRef);

            a.Dispose();

            var ex = Assert.Throws<KeyNotFoundException>(() => e1.Locals["a"]);
            Assert.Equal("No local with name 'a' found", ex.Message);
        }

        [Fact]
        public void LocalReuseInitializedToDefault()
        {
            {
                var e1 = Emit<Func<bool>>.NewDynamicMethod();
                using (var loc = e1.DeclareLocal<bool>())
                {
                    e1.LoadConstant(true);
                    e1.StoreLocal(loc);
                }

                using (var loc = e1.DeclareLocal<bool>())
                {
                    e1.LoadLocal(loc);
                    e1.Return();
                }

                var d1 = e1.CreateDelegate();

                Assert.False(d1());
            }

            {
                var e1 = Emit<Func<DateTime>>.NewDynamicMethod();
                using (var loc = e1.DeclareLocal<DateTime>())
                {
                    e1.LoadConstant(DateTime.MaxValue.Ticks);
                    e1.NewObject<DateTime, long>();
                    e1.StoreLocal(loc);
                }

                using (var loc = e1.DeclareLocal<DateTime>())
                {
                    e1.LoadLocal(loc);
                    e1.Return();
                }

                var d1 = e1.CreateDelegate();

                Assert.Equal(default, d1());
            }
        }

        [Fact]
        public void ReinitializeOptOut()
        {
            var e1 = Emit<Func<int>>.NewDynamicMethod();

            using (var a = e1.DeclareLocal<int>("a"))
            {
                e1.LoadLocal(a);
                e1.LoadConstant(1);
                e1.Add();
            }

            using (var b = e1.DeclareLocal(typeof(int), "b", initializeReused: false))
            {
                e1.StoreLocal(b);
                e1.LoadLocal(b);
                e1.Return();
            }

            var d1 = e1.CreateDelegate(out string instrs);

            Assert.Equal(1, d1());
            Assert.Equal("ldloc.0\r\nldc.i4.1\r\nadd\r\nstloc.0\r\nldloc.0\r\nret\r\n", instrs);
        }
    }
}
