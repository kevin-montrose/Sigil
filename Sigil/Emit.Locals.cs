using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public Local CreateLocal<LocalType>()
        {
            return CreateLocal<LocalType>("_" + Guid.NewGuid().ToString().Replace("-", ""));
        }

        public Local CreateLocal<LocalType>(string name)
        {
            return CreateLocal(typeof(LocalType), name);
        }

        public Local CreateLocal(Type type)
        {
            return CreateLocal(type, "_" + Guid.NewGuid().ToString().Replace("-", ""));
        }

        public Local CreateLocal(Type type, string name)
        {
            var local = IL.DeclareLocal(type);

            var localIndex = NextLocalIndex;
            NextLocalIndex++;

            var ret = new Local(this, localIndex, type, local, name);

            UnusedLocals.Add(ret);

            return ret;
        }
    }
}
