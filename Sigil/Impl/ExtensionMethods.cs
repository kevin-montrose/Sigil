using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Sigil.Impl
{
    internal static class ExtensionMethods
    {
        public static bool IsTailableCall(OpCode op)
        {
            return
                op == OpCodes.Call ||
                //op == OpCodes.Calli ||
                op == OpCodes.Callvirt;
        }

        public static bool StartsWithVowel(string str)
        {
            var c = char.ToLower(str[0]);

            return "aeiou".IndexOf(c) != -1;
        }
        
        public static bool IsVolatile(FieldInfo field)
        {
            // field builder doesn't implement GetRequiredCustomModifiers
            if (field is FieldBuilder) return false;

            return Array.IndexOf(field.GetRequiredCustomModifiers(), typeof(System.Runtime.CompilerServices.IsVolatile)) >= 0;
        }

        public static bool IsPrefix(OpCode op)
        {
            return
                op == OpCodes.Tailcall ||
                op == OpCodes.Readonly ||
                op == OpCodes.Volatile ||
                op == OpCodes.Unaligned;
        }

        private static Dictionary<Type, Type> _AliasCache = new Dictionary<Type, Type>
        {
            { typeof(bool), typeof(int) },
            { typeof(sbyte), typeof(int) },
            { typeof(byte), typeof(int) },
            { typeof(short), typeof(int) },
            { typeof(ushort), typeof(int) },
            { typeof(uint), typeof(int) },
            { typeof(ulong), typeof(long) },
            { typeof(IntPtr), typeof(NativeIntType) },
            { typeof(UIntPtr), typeof(NativeIntType) }
        };

        private static Type Alias(Type t)
        {
            if (TypeHelpers.IsValueType(t) && TypeHelpers.IsEnum(t))
            {
                return Alias(Enum.GetUnderlyingType(t));
            }

            Type ret;
            if (!_AliasCache.TryGetValue(t, out ret)) ret = t;

            return ret;
        }

        public static bool IsAssignableFrom(Type type1, TypeOnStack type2)
        {
            return TypeOnStack.Get(type1).IsAssignableFrom(type2);
        }

        public static bool IsAssignableFrom(TypeOnStack type1, TypeOnStack type2)
        {
            // wildcards match *everything*
            if (type1 == TypeOnStack.Get<WildcardType>() || type2 == TypeOnStack.Get<WildcardType>()) return true;

            if (type1.IsArray && type2.IsArray)
            {
                if (type1.Type.GetArrayRank() == type2.Type.GetArrayRank())
                {
                    var t1Elem = type1.Type.GetElementType();
                    var t2Elem = type2.Type.GetElementType();

                    while (t1Elem.HasElementType) t1Elem = t1Elem.GetElementType();
                    while (t2Elem.HasElementType) t2Elem = t2Elem.GetElementType();

                    if (t1Elem == typeof(WildcardType) || t2Elem == typeof(WildcardType)) return true;
                }
            }

            if (type1.IsPointer && type2.IsPointer)
            {
                if (type1.Type.GetElementType() == typeof(WildcardType) || type2.Type.GetElementType() == typeof(WildcardType)) return true;
            }

            if (type1.IsReference && type2.IsReference)
            {
                if (type1.Type.GetElementType() == typeof(WildcardType) || type2.Type.GetElementType() == typeof(WildcardType)) return true;
            }

            // any pointer type matches, well, any pointer
            if (type1.Type == typeof(AnyPointerType) && type2.IsPointer) return true;
            if (type2.Type == typeof(AnyPointerType) && type1.IsPointer) return true;

            // likewise for any by ref
            if (type1.Type == typeof(AnyByRefType) && type2.IsReference) return true;
            if (type2.Type == typeof(AnyByRefType) && type1.IsReference) return true;

            // Native int can be convereted to any pointer type
            if (type1.IsPointer && type2 == TypeOnStack.Get<NativeIntType>()) return true;
            if (type2.IsPointer && type1 == TypeOnStack.Get<NativeIntType>()) return true;

            if ((type1.IsPointer || type1.IsReference) && !(type2.IsPointer || type2.IsReference)) return false;
            if ((type2.IsPointer || type2.IsReference) && !(type1.IsPointer || type1.IsReference)) return false;

            if (type1.IsPointer || type1.IsReference)
            {
                return type1.Type.GetElementType() == type2.Type.GetElementType();
            }

            var t1 = type1.Type;
            var t2 = type2.Type;


            // The null type can be assigned to any reference type
            if (t1 == typeof(NullType) && !TypeHelpers.IsValueType(t2)) return true;
            if (t2 == typeof(NullType) && !TypeHelpers.IsValueType(t1)) return true;

            t1 = Alias(t1);
            t2 = Alias(t2);

            return ReallyIsAssignableFrom(t1, t2);
        }

        private static LinqList<Type> GetBases(Type t)
        {
            if (TypeHelpers.IsValueType(t)) return new LinqList<Type>();

            var ret = new LinqList<Type>();
            t = TypeHelpers.GetBaseType(t);

            while (t != null)
            {
                ret.Add(t);
                t = TypeHelpers.GetBaseType(t);
            }

            return ret;
        }

        private static bool ReallyIsAssignableFrom(Type t1, Type t2)
        {
            if (t1 == t2) return true;

            if (t1 == typeof(OnlyObjectType))
            {
                if (t2 == typeof(object)) return true;

                return false;
            }

            // quick and dirty base case
            if (t1 == typeof(object) && !TypeHelpers.IsValueType(t2)) return true;

            // you have to box in this case
            if (t1 == typeof(object) && TypeHelpers.IsValueType(t2)) return false;

            var t1Bases = GetBases(t1);
            var t2Bases = GetBases(t2);

            if (t2Bases.Any(t2b => TypeOnStack.Get(t1).IsAssignableFrom(TypeOnStack.Get(t2b)))) return true;

            if (TypeHelpers.IsInterface(t1))
            {
                var t2Interfaces = (LinqArray<Type>)t2.GetInterfaces();

                return t2Interfaces.Any(t2i => TypeOnStack.Get(t1).IsAssignableFrom(TypeOnStack.Get(t2i)));
            }

            if (TypeHelpers.IsGenericType(t1) && TypeHelpers.IsGenericType(t2))
            {
                var t1Def = t1.GetGenericTypeDefinition();
                var t2Def = t2.GetGenericTypeDefinition();

                if (t1Def != t2Def) return false;

                var t1Args = t1.GetGenericArguments();
                var t2Args = t2.GetGenericArguments();

                for (var i = 0; i < t1Args.Length; i++)
                {
                    if (!TypeOnStack.Get(t1Args[i]).IsAssignableFrom(TypeOnStack.Get(t2Args[i]))) return false;
                }

                return true;
            }

            try
            {
                return t1.IsAssignableFrom(t2);
            }
            catch (NotSupportedException) 
            { 
                // Builders, some generic types, and so on don't implement this; just assume it's *no good* for now
                return false; 
            }
        }
    }
}
