using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SigilTests
{
    public partial class LoadArguments
    {
        [Fact]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(string), new [] { typeof(int) }, "E1");
            e1.LoadArgument(0);
            e1.Box<int>();
            e1.CallVirtual(typeof(object).GetMethod("ToString"));
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int, string>>();

            Assert.Equal("31415", d1(31415));
        }

        [Fact]
        public void AllNonGeneric()
        {
            var retType = typeof(LotsOfParams).GetMethod("Invoke").ReturnType;
            var paramTypes = typeof(LotsOfParams).GetMethod("Invoke").GetParameters().Select(p => p.ParameterType).ToArray();

            var e1 = Emit.NewDynamicMethod(retType, paramTypes);

            for (ushort i = 0; i < 260; i++)
            {
                e1.LoadArgument(i);
            }

            for (var i = 0; i < 259; i++)
            {
                e1.Add();
            }

            e1.Return();

            var d1 = e1.CreateDelegate<LotsOfParams>();

            var rand = new Random();
            var args = new List<int>();
            for (var i = 0; i < 260; i++)
            {
                args.Add(rand.Next(10));
            }

            var ret = (int)d1.DynamicInvoke(args.Cast<object>().ToArray());

            Assert.Equal(args.Sum(), ret);

            Assert.Equal(260, e1.MaxStackSize);
        }
    }
}
