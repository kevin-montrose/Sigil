using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigil.Impl
{
    internal static class ExtensionMethods
    {
        public static bool IsAssignableFrom(this Type type1, TypeOnStack type2)
        {
            if (type1.IsPointer)
            {
                if (!type2.IsPointer) return false;

                return type1.IsAssignableFrom(type2.Type.MakePointerType());
            }

            if (type1.IsByRef)
            {
                if (!type2.IsReference) return false;

                return type1.IsAssignableFrom(type2.Type.MakeByRefType());
            }

            return type1.IsAssignableFrom(type2.Type);
        }

        public static bool IsAssignableFrom(this TypeOnStack type1, TypeOnStack type2)
        {
            if (type1.IsPointer != type2.IsPointer) return false;
            if (type1.IsReference != type2.IsReference) return false;

            return type1.Type.IsAssignableFrom(type2.Type);
        }
    }
}
