using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class BlogPost
    {
        [Fact]
        public void Block1()
        {
            var il = Emit<Func<int>>.NewDynamicMethod("AddOneAndTwo");
            il.LoadConstant(1);

            var ex = Assert.Throws<SigilVerificationException>(() =>
            {
                // Still missing that 2!
                il.Add();
                il.Return();
                var del = il.CreateDelegate();
                del();
            });

            Assert.Equal("Add expects 2 values on the stack", ex.Message);
        }

        [Fact]
        public void Block2()
        {
            var il = Emit<Func<string, Func<string, int>, string>>.NewDynamicMethod("E1");
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

                il.CreateDelegate();
            });
            Assert.Equal("Return expected a System.String; found int", ex.Message);
        }
    }
}
