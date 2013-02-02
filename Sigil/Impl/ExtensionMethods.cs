using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigil.Impl
{
    internal static class ExtensionMethods
    {
        private static Type Alias(Type t)
        {
            if (t == typeof(bool) || t == typeof(sbyte) || t == typeof(byte) || t == typeof(short) || t == typeof(ushort) || t == typeof(uint))
            {
                // Nothing smaller than In32 exists in CLR land
                return typeof(int);
            }

            if (t == typeof(long) || t == typeof(ulong))
            {
                // long/ulong are interchangable on the stack
                return typeof(long);
            }

            if (t == typeof(IntPtr) || t == typeof(UIntPtr))
            {
                return typeof(NativeInt);
            }

            return t;
        }

        public static bool IsAssignableFrom(this Type type1, TypeOnStack type2)
        {
            return TypeOnStack.Get(type1).IsAssignableFrom(type2);
        }

        public static bool IsAssignableFrom(this TypeOnStack type1, TypeOnStack type2)
        {
            // Native int can be convereted to any pointer type
            if (type1.IsPointer && type2 == TypeOnStack.Get<NativeInt>()) return true;
            if (type2.IsPointer && type1 == TypeOnStack.Get<NativeInt>()) return true;

            if (type1.IsPointer != type2.IsPointer) return false;
            if (type1.IsReference != type2.IsReference) return false;

            var t1 = type1.Type;
            var t2 = type2.Type;

            t1 = Alias(t1);
            t2 = Alias(t2);

            return t1.IsAssignableFrom(t2);
        }

        public static bool IsAssignableFrom(this TypeOnStack type1, Type type2)
        {
            return type1.IsAssignableFrom(TypeOnStack.Get(type2));
        }
    }
}
