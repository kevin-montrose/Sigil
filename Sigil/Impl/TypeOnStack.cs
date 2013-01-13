using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigil.Impl
{
    internal class TypeOnStack
    {
        private class TypeCache<Type>
        {
            private static TypeOnStack Value;

            static TypeCache()
            {
                Value =
                    new TypeOnStack
                    {
                        Type = typeof(Type),
                        IsReference = false,
                        IsPointer = false
                    };
            }

            public static TypeOnStack Get() { return Value; }
        }

        private class ReferenceTypeCache<Type>
        {
            private static TypeOnStack Value;

            static ReferenceTypeCache()
            {
                Value =
                    new TypeOnStack
                    {
                        Type = typeof(Type),
                        IsReference = true,
                        IsPointer = false
                    };
            }

            public static TypeOnStack Get() { return Value; }
        }

        private class PointerTypeCache<Type>
        {
            private static TypeOnStack Value;

            static PointerTypeCache()
            {
                Value =
                    new TypeOnStack
                    {
                        Type = typeof(Type),
                        IsReference = true,
                        IsPointer = false
                    };
            }

            public static TypeOnStack Get() { return Value; }
        }

        public Type Type { get; set; }
        public bool IsReference { get; set; }
        public bool IsPointer { get; set; }

        public static TypeOnStack Get<T>()
        {
            return TypeCache<T>.Get();
        }

        public static TypeOnStack GetReference<T>()
        {
            return ReferenceTypeCache<T>.Get();
        }

        public static TypeOnStack GetPointer<T>()
        {
            return PointerTypeCache<T>.Get();
        }
    }

    // Stand in for native int type
    internal class NativeInt { }

    // Stand in for "float" as defined on the stack; no distinction between float/double
    internal class StackFloat { }
}
