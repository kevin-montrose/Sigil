using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using Xunit;

namespace SigilTests
{
    public partial class LoadlLocalAddress
    {
        [Fact]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes, "E1");
            var a = e1.DeclareLocal<int>("a");
            e1.LoadConstant(123);
            e1.StoreLocal(a);
            e1.LoadLocalAddress(a);
            e1.LoadIndirect<int>();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int>>();

            Assert.Equal(123, d1());
        }

        [Fact]
        public void AllNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes);

            var locals = new List<Sigil.Local>();
            int total = 0;

            for (var i = 0; i <= 256; i++)
            {
                var l = e1.DeclareLocal<int>();
                e1.LoadConstant(i);
                e1.StoreLocal(l);

                locals.Add(l);

                total += i;
            }

            foreach (var l in locals)
            {
                e1.LoadLocalAddress(l);
                e1.LoadIndirect<int>();
            }

            for (var i = 0; i <= 255; i++)
            {
                e1.Add();
            }

            e1.Return();

            var d1 = e1.CreateDelegate<Func<int>>();

            Assert.Equal(total, d1());
        }
    }
}
