using System;
using System.Reflection;

namespace Sigil.Impl
{
    /// <summary>
    /// Contains helper methods to shim over the difference between different Type APIs in
    /// different frameworks
    /// </summary>
    internal static class TypeHelpers
    {
#if COREFX
        public static readonly Type[] EmptyTypes = new Type[0];
        public static Type GetBaseType(Type type)
        {
            return type.GetTypeInfo().BaseType;
        }
        public static bool IsValueType(Type type)
        {
            return type.GetTypeInfo().IsValueType;
        }
        public static bool ContainsGenericParameters(Type type)
        {
            return type.GetTypeInfo().ContainsGenericParameters;
        }
        public static bool IsGenericType(Type type)
        {
            return type.GetTypeInfo().IsGenericType;
        }
        public static bool IsEnum(Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }
        public static bool IsPrimitive(Type type)
        {
            return type.GetTypeInfo().IsPrimitive;
        }
        public static MethodInfo GetMethod(Type type, string name, Type[] parameterTypes)
        {
            return type.GetRuntimeMethod(name, parameterTypes);
        }
        public static bool IsInterface(Type type)
        {
            return type.GetTypeInfo().IsInterface;
        }
        public static Type AsType(System.Reflection.Emit.TypeBuilder type)
        {
            return type == null ? null : type.AsType();
        }
        public static Module GetModule(Type type)
        {
            return type.GetTypeInfo().Module;
        }
        public static bool IsAssignableFrom(Type x, Type y)
        {
            return x.GetTypeInfo().IsAssignableFrom(y.GetTypeInfo());
        }
#else
        public static readonly Type[] EmptyTypes = Type.EmptyTypes;
        public static Type GetBaseType(Type type)
        {
            return type.BaseType;
        }
        public static bool IsValueType(Type type)
        {
            return type.IsValueType;
        }
        public static bool ContainsGenericParameters(Type type)
        {
            return type.ContainsGenericParameters;
        }
        public static bool IsGenericType(Type type)
        {
            return type.IsGenericType;
        }
        public static bool IsEnum(Type type)
        {
            return type.IsEnum;
        }
        public static bool IsPrimitive(Type type)
        {
            return type.IsPrimitive;
        }
        public static bool IsInterface(Type type)
        {
            return type.IsInterface;
        }
        public static MethodInfo GetMethod(Type type, string name, Type[] parameterTypes)
        {
            return type.GetMethod(name, parameterTypes);
        }
        public static Type AsType(System.Reflection.Emit.TypeBuilder type)
        {
            return type;
        }
        public static Module GetModule(Type type)
        {
            return type.Module;
        }
        public static bool IsAssignableFrom(Type x, Type y)
        {
            return x.IsAssignableFrom(y);
        }
#endif
    }
}
