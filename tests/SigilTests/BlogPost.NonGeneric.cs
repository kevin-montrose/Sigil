using Sigil;
using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class BlogPost
    {
        [Fact]
        public void Block1NonGeneric()
        {
            var il = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes, "AddOneAndTwo");
            il.LoadConstant(1);

            var ex = Assert.Throws<SigilVerificationException>(() =>
            {
                // Still missing that 2!
                il.Add();
                il.Return();
                var del = il.CreateDelegate<Func<int>>();
                del();
            });

            Assert.Equal("Add expects 2 values on the stack", ex.Message);
        }

        [Fact]
        public void Block2NonGeneric()
        {
            var il = Emit.NewDynamicMethod(typeof(string), new[] { typeof(string), typeof(Func<string, int>) }, "E1");
            var invoke = typeof(Func<string, int>).GetMethod("Invoke");
            var notNull = il.DefineLabel("not_null");

            var ex = Assert.Throws<SigilVerificationException>(() =>
            {
                il.LoadArgument(0);
                il.LoadNull();
                il.UnsignedBranchIfNotEqual(notNull);
                il.LoadNull();
                il.Return();

                il.MarkLabel(notNull);
                il.LoadArgument(1);
                il.LoadArgument(0);
                il.CallVirtual(invoke);
                il.Return();

                il.CreateDelegate<Func<string, Func<string, int>, string>>();
            });
            Assert.Equal("Return expected a System.String; found int", ex.Message);
        }
    }
}
