using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    public static class TypeHelpers
    {
        public static MethodInfo GetMethod(this Type type, string name)
        {
            MethodInfo found = null;
            foreach(var method in type.GetRuntimeMethods().Where(x => x.Name == name).Take(2))
            {
                if(found != null) throw new AmbiguousMatchException(name);
                found = method;
            }
            return found;
        }
        public static MethodInfo GetMethod(this Type type, string name, Type[] parameterTypes)
        {
            return type.GetRuntimeMethod(name, parameterTypes);
        }
        static bool IsMatch(ParameterInfo[] declared, Type[] expected)
        {
            if (expected == null) expected = Type.EmptyTypes;
            if (declared.Length != expected.Length) return false;
            for (int i = 0; i < expected.Length; i++)
            {
                if (declared[i].ParameterType != expected[i]) return false;
            }
            return true;
        }
        public static Type MakeGenericType(this Type type, params System.Reflection.Emit.TypeBuilder[] args)
        {
            Type[] t = new Type[args.Length];
            for (int i = 0; i < args.Length; i++)
                t[i] = args[i].AsType();
            return type.MakeGenericType(t);
        }
        public static ConstructorInfo GetConstructor(this Type type, params Type[] parameterTypes)
        {
            if (parameterTypes == null) parameterTypes = Type.EmptyTypes;
            foreach(var ctor in type.GetTypeInfo().DeclaredConstructors)
            {
                var args = ctor.GetParameters();
                if (IsMatch(args, parameterTypes)) return ctor;
            }
            return null;
        }
        public static Type CreateType(this System.Reflection.Emit.TypeBuilder type)
        {
            return type.CreateTypeInfo().AsType();
        }
        public static FieldInfo GetField(this Type type, string name)
        {
            return type.GetTypeInfo().GetDeclaredField(name);
        }
        public static PropertyInfo GetProperty(this Type type, string name)
        {
            return type.GetTypeInfo().GetDeclaredProperty(name);
        }
        public static MethodInfo GetGetMethod(this PropertyInfo property)
        {
            return property.GetMethod;
        }
    }
    public static class Assert
    {
        public static void AreEqual<T>(T x, T y)
        {
            Xunit.Assert.Equal<T>(x, y);
        }
        public static void AreEqual<T>(IEnumerable<T> x, IEnumerable<T> y)
        {
            Xunit.Assert.Equal<T>(x, y);
        }
        public static void AreNotEqual<T>(T x, T y)
        {
            Xunit.Assert.NotEqual<T>(x, y);
        }
        public static void AreNotEqual<T>(IEnumerable<T> x, IEnumerable<T> y)
        {
            Xunit.Assert.NotEqual<T>(x, y);
        }
        public static void IsFalse(bool value)
        {
            Xunit.Assert.False(value);
        }
        public static void IsTrue(bool value)
        {
            Xunit.Assert.True(value);
        }
        public static void Fail()
        {
            Xunit.Assert.Equal("pass", "fail");
        }
        public static void Fail(string message)
        {
            string expected = "pass";
            if (expected == message) expected = "passed"; // unlikely, but!
            Xunit.Assert.Equal(expected, message);
        }
        public static void IsNull(object @object)
        {
            Xunit.Assert.Null(@object);
        }
        public static void IsNotNull(object @object)
        {
            Xunit.Assert.NotNull(@object);
        }
        public static void IsInstanceOfType(object @object, Type type)
        {
            Xunit.Assert.IsType(type, @object);
        }
    }
}
namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    class TestClassAttribute : Attribute { }
#if WTFDNXTEST
    // disables all the tests except an example of the 4 that are foobar
    class ActualTestMethodAttribute : Xunit.FactAttribute { }
    class TestMethodAttribute : Attribute { }
#else
    class TestMethodAttribute : Xunit.FactAttribute { }
#endif

}

namespace System.Diagnostics.CodeAnalysis
{
    class ExcludeFromCodeCoverageAttribute : Attribute { }
}