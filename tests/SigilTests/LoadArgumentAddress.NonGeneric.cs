using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SigilTests
{
    public partial class LoadArgumentAddress
    {
        [Fact]
        public void AllNonGeneric()
        {
            var retType = typeof(LotsOfParams).GetMethod("Invoke").ReturnType;
            var paramTypes = typeof(LotsOfParams).GetMethod("Invoke").GetParameters().Select(p => p.ParameterType).ToArray();

            var e1 = Emit.NewDynamicMethod(retType, paramTypes);

            for (ushort i = 0; i < 260; i++)
            {
                e1.LoadArgumentAddress(i);
                e1.LoadIndirect<int>();
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
        }
    }
}
