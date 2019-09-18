using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using Xunit;

namespace SigilTests
{
    public partial class LoadLocal
    {
        [Fact]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes, "E1");

            var foo = e1.DeclareLocal<int>("foo");

            e1.LoadLocal(foo);
            e1.LoadConstant(3);
            e1.Add();
            e1.Return();

            var del = e1.CreateDelegate<Func<int>>();

            Assert.Equal(3, del());
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
                e1.LoadLocal(l);
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
