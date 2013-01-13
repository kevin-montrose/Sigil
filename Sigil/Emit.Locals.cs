using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public EmitLocal CreateLocal<LocalType>()
        {
            return CreateLocal<LocalType>("_" + Guid.NewGuid().ToString().Replace("-", ""));
        }

        public EmitLocal CreateLocal<LocalType>(string name)
        {
            return CreateLocal(typeof(LocalType), name);
        }

        public EmitLocal CreateLocal(Type type)
        {
            return CreateLocal(type, "_" + Guid.NewGuid().ToString().Replace("-", ""));
        }

        public EmitLocal CreateLocal(Type type, string name)
        {
            var local = IL.DeclareLocal(type);

            var ret = new EmitLocal(this, local, name);

            UnusedLocals.Add(ret);

            return ret;
        }
    }
}
