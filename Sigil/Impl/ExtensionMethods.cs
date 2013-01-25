using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigil.Impl
{
    internal static class ExtensionMethods
    {
        private static Type AliasInts(Type t)
        {
            if (t == typeof(bool) || t == typeof(sbyte) || t == typeof(byte) || t == typeof(short) || t == typeof(ushort) || t == typeof(uint))
            {
                // Nothing smaller than In32 exists in CLR land
                return typeof(int);
            }

            return t;
        }

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

            var t1 = type1;
            var t2 = type2.Type;

            t1 = AliasInts(t1);
            t2 = AliasInts(t2);

            return t1.IsAssignableFrom(t2);
        }

        public static bool IsAssignableFrom(this TypeOnStack type1, TypeOnStack type2)
        {
            if (type1.IsPointer != type2.IsPointer) return false;
            if (type1.IsReference != type2.IsReference) return false;

            var t1 = type1.Type;
            var t2 = type2.Type;

            t1 = AliasInts(t1);
            t2 = AliasInts(t2);

            return t1.IsAssignableFrom(t2);
        }

        public static bool IsAssignableFrom(this TypeOnStack type1, Type type2)
        {
            return type1.IsAssignableFrom(TypeOnStack.Get(type2));
        }
    }
}
