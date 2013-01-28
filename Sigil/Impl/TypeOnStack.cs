using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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
                        IsReference = false,
                        IsPointer = true
                    };
            }

            public static TypeOnStack Get() { return Value; }
        }

        public static bool operator ==(TypeOnStack a, TypeOnStack b)
        {
            if (object.ReferenceEquals(a, b)) return true;
            if (object.ReferenceEquals(a, null)) return false;
            if (object.ReferenceEquals(b, null)) return false;

            // There's not exact map of NativeInt in .NET; but once it's on the stack IntPtr can be manipulated similarly
            if (a.Type == typeof(NativeInt) && b.Type == typeof(IntPtr) || b.Type == typeof(NativeInt) && a.Type == typeof(IntPtr))
            {
                return a.IsPointer == b.IsPointer && a.IsReference == b.IsReference;
            }

            return
                a.Type == b.Type &&
                a.IsPointer == b.IsPointer &&
                a.IsReference == b.IsReference;
        }

        public static bool operator !=(TypeOnStack a, TypeOnStack b)
        {
            return !(a == b);
        }

        private static readonly Dictionary<Tuple<CallingConventions, Type, Type, Type[]>, TypeOnStack> KnownFunctionPointerCache = new Dictionary<Tuple<CallingConventions, Type, Type, Type[]>, TypeOnStack>();

        public Type Type { get; private set; }
        public bool IsReference { get; private set; }
        public bool IsPointer { get; private set; }

        public bool HasAttachedMethodInfo { get; private set; }
        public CallingConventions CallingConvention { get; private set; }
        public Type InstanceType { get; private set; }
        public Type ReturnType { get; private set; }
        public Type[] ParameterTypes { get; private set; }

        private List<Tuple<OpCode, int>> UsedBy { get; set; }

        /// <summary>
        /// Call to indicate that something on the stack was used
        /// as the #{index}'d (starting at 0) parameter to the {code} 
        /// opcode.
        /// </summary>
        public void Mark(OpCode code, int index)
        {
            if (UsedBy == null) return;

            UsedBy.Add(Tuple.Create(code, index));
        }

        /// <summary>
        /// Returns the # of times this value was used as the given #{index}'d parameter to the {code} instruction.
        /// </summary>
        public int CountMarks(OpCode code, int index)
        {
            if (UsedBy == null) throw new Exception(this + " is not markable");

            var val = Tuple.Create(code, index);

            return UsedBy.Count(c => c.Equals(val));
        }

        /// <summary>
        /// Returns the total number of times this value was marked.
        /// </summary>
        public int CountMarks()
        {
            if (UsedBy == null) throw new Exception(this + " is not markable");

            return UsedBy.Count;
        }

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

        public static TypeOnStack Get(Type type, bool makeMarkable = false)
        {
            TypeOnStack ret;

            if (type.IsPointer)
            {
                var nonPointer = type.GetElementType();

                var get = typeof(TypeOnStack).GetMethod("GetPointer").MakeGenericMethod(nonPointer);

                ret = (TypeOnStack)get.Invoke(null, new object[0]);
            }
            else
            {

                if (type.IsByRef)
                {
                    var nonRef = type.GetElementType();

                    var get = typeof(TypeOnStack).GetMethod("GetReference").MakeGenericMethod(nonRef);

                    ret = (TypeOnStack)get.Invoke(null, new object[0]);
                }
                else
                {

                    var getM = typeof(TypeOnStack).GetMethods().Single(w => w.Name == "Get" && w.IsGenericMethod).MakeGenericMethod(type);

                    ret = (TypeOnStack)getM.Invoke(null, new object[0]);
                }
            }

            if (!makeMarkable)
            {
                return ret;
            }

            return
                new TypeOnStack
                {
                    CallingConvention = ret.CallingConvention,
                    HasAttachedMethodInfo = ret.HasAttachedMethodInfo,
                    InstanceType = ret.InstanceType,
                    IsPointer = ret.IsPointer,
                    IsReference = ret.IsReference,
                    ParameterTypes = ret.ParameterTypes,
                    ReturnType = ret.ReturnType,
                    Type = ret.Type,
                    UsedBy = new List<Tuple<OpCode,int>>()
                };
        }

        public static TypeOnStack GetKnownFunctionPointer(CallingConventions conv, Type instanceType, Type returnType, Type[] parameterTypes)
        {
            var key = Tuple.Create(conv, instanceType, returnType, parameterTypes);

            TypeOnStack ret;

            lock (KnownFunctionPointerCache)
            {
                if (!KnownFunctionPointerCache.TryGetValue(key, out ret))
                {
                    ret =
                        new TypeOnStack
                        {
                            HasAttachedMethodInfo = true,
                            Type = typeof(NativeInt),
                            CallingConvention = conv,
                            InstanceType = instanceType,
                            ReturnType = returnType,
                            ParameterTypes = parameterTypes
                        };

                    KnownFunctionPointerCache[key] = ret;
                }
            }

            return ret;
        }

        public override string ToString()
        {
            var ret = Type.FullName;

            if (Type == typeof(NativeInt)) ret = "native int";

            if (IsPointer) ret += "*";
            if (IsReference) ret += "&";

            return ret;
        }

        public override bool Equals(object obj)
        {
            var other = obj as TypeOnStack;
            return this == other;
        }

        public override int GetHashCode()
        {
            return
                (int)(
                    Type.GetHashCode() ^
                    (IsPointer ? 0x0000FFFF : 0) ^
                    (IsReference ? 0xFFFF0000 : 0)
                );
        }
    }

    // Stand in for native int type
    internal class NativeInt { }
}