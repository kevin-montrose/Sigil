using Sigil.NonGeneric;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SigilTests
{
    public partial class StoreArgument
    {
        [Fact]
        public void AllNonGeneric()
        {
            var returnType = typeof(LotsOfParams).GetMethod("Invoke").ReturnType;
            var paramTypes = typeof(LotsOfParams).GetMethod("Invoke").GetParameters().Select(p => p.ParameterType).ToArray();

            var e1 = Emit.NewDynamicMethod(returnType, paramTypes);

            var args = new List<int>();

            for (ushort i = 0; i < 260; i++)
            {
                e1.LoadConstant(i);
                e1.StoreArgument(i);

                args.Add(i);
            }

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

            var x = (int)d1.DynamicInvoke(args.Cast<object>().ToArray());

            Assert.Equal(args.Sum(), x);
        }
    }
}
