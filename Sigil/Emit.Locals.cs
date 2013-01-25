using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public Local DeclareLocal<LocalType>()
        {
            return DeclareLocal<LocalType>("_" + Guid.NewGuid().ToString().Replace("-", ""));
        }

        public Local DeclareLocal<LocalType>(string name)
        {
            return DeclareLocal(typeof(LocalType), name);
        }

        public Local DeclareLocal(Type type)
        {
            return DeclareLocal(type, "_" + Guid.NewGuid().ToString().Replace("-", ""));
        }

        public Local DeclareLocal(Type type, string name)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            var local = IL.DeclareLocal(type);

            var localIndex = NextLocalIndex;
            NextLocalIndex++;

            var ret = new Local(this, localIndex, type, local, name);

            UnusedLocals.Add(ret);

            return ret;
        }
    }
}
