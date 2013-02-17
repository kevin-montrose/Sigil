using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Sigil.Impl
{
    internal class TypeOnStack
    {
        private class TypeCache
        {
            private static Dictionary<Type, TypeOnStack> Cache = new Dictionary<Type, TypeOnStack>();

            public static TypeOnStack Get(Type t)
            {
                lock (Cache)
                {
                    TypeOnStack ret;
                    if (!Cache.TryGetValue(t, out ret))
                    {
                        ret =
                            new TypeOnStack
                            {
                                Type = t
                            };

                        Cache[t] = ret;
                    }

                    return ret;
                }
            }
        }

        public static bool operator ==(TypeOnStack a, TypeOnStack b)
        {
            if (object.ReferenceEquals(a, b)) return true;
            if (object.ReferenceEquals(a, null)) return false;
            if (object.ReferenceEquals(b, null)) return false;

            // There's not exact map of NativeInt in .NET; but once it's on the stack IntPtr can be manipulated similarly
            if (a.Type == typeof(NativeIntType) && b.Type == typeof(IntPtr) || b.Type == typeof(NativeIntType) && a.Type == typeof(IntPtr))
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

        public bool IsReference { get { return Type.IsByRef; } }
        public bool IsPointer { get { return Type.IsPointer; } }

        public bool HasAttachedMethodInfo { get; private set; }
        public CallingConventions CallingConvention { get; private set; }
        public Type InstanceType { get; private set; }
        public Type ReturnType { get; private set; }
        public Type[] ParameterTypes { get; private set; }

        public bool IsMarkable { get { return UsedBy != null; } }

        private List<Tuple<OpCode, int, bool>> UsedBy { get; set; }

        private List<TypeOnStack> ReplacedWithTypes { get; set; }

        /// <summary>
        /// When a type is replaced (without an opcode) on the stack, call this to keep
        /// any mark tracking working.
        /// </summary>
        public void ReplacedWith(TypeOnStack newType)
        {
            if (!IsMarkable) return;

            ReplacedWithTypes.Add(newType);
        }

        /// <summary>
        /// Call to indicate that something on the stack was used
        /// as the #{index}'d (starting at 0) parameter to the {code} 
        /// opcode.
        /// </summary>
        public void Mark(OpCode code, int index, bool isThis)
        {
            if (UsedBy == null) return;

            UsedBy.Add(Tuple.Create(code, index, isThis));
        }

        /// <summary>
        /// Returns the # of times this value was used as the given #{index}'d parameter to the {code} instruction.
        /// </summary>
        public int CountMarks(OpCode code, int index, bool isThis)
        {
            if (UsedBy == null) throw new Exception(this + " is not markable");

            var val = Tuple.Create(code, index, isThis);

            return UsedBy.Count(c => c.Equals(val)) + ReplacedWithTypes.Sum(t => t.CountMarks(code, index, isThis));
        }

        /// <summary>
        /// Returns the total number of times this value was marked.
        /// </summary>
        public int CountMarks()
        {
            if (UsedBy == null) throw new Exception(this + " is not markable");

            return UsedBy.Count + ReplacedWithTypes.Sum(t => t.CountMarks());
        }

        public static TypeOnStack Get<T>()
        {
            return Get(typeof(T));
        }

        public static TypeOnStack Get(Type type, bool makeMarkable = false)
        {
            if (type.ContainsGenericParameters)
            {
                throw new InvalidOperationException("Sigil does not currently support generic types; found " + type);
            }

            var ret = TypeCache.Get(type);

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
                    ParameterTypes = ret.ParameterTypes,
                    ReturnType = ret.ReturnType,
                    Type = ret.Type,
                    UsedBy = new List<Tuple<OpCode, int, bool>>(),
                    ReplacedWithTypes = new List<TypeOnStack>()
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
                            Type = typeof(NativeIntType),
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

            if (Type == typeof(NativeIntType)) ret = "native int";
            if (Type == typeof(NullType)) ret = "null";
            if (Type == typeof(int)) ret = "int";
            if (Type == typeof(long)) ret = "long";
            if (Type == typeof(float)) ret = "float";
            if (Type == typeof(double)) ret = "double";
            if (Type == typeof(AnyPointerType)) ret = "pointer";

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
}