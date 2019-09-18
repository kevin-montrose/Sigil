using Sigil.Impl;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        #region Generic NewObject Constructor Finder Helpers

        /// <summary>
        /// Invokes the parameterless constructor of the given type, and pushes a reference to the new object onto the stack.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> NewObject<ReferenceType>()
        {
            return NewObject(typeof(ReferenceType));
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> NewObject<ReferenceType, ParameterType1>()
        {
            return NewObject(typeof(ReferenceType), typeof(ParameterType1));
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> NewObject<ReferenceType, ParameterType1, ParameterType2>()
        {
            return NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2));
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3>()
        {
            return NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3));
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4>()
        {
            return NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4));
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5>()
        {
            return NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5));
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6>()
        {
            return NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6));
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7>()
        {
            return NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7));
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8>()
        {
            return NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8));
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9>()
        {
            return NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9));
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10>()
        {
            return NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10));
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11>()
        {
            return NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11));
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12>()
        {
            return NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12));
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13>()
        {
            return NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12), typeof(ParameterType13));
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14>()
        {
            return NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12), typeof(ParameterType13), typeof(ParameterType14));
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14, ParameterType15>()
        {
            return NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12), typeof(ParameterType13), typeof(ParameterType14), typeof(ParameterType15));
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Emit<DelegateType> NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14, ParameterType15, ParameterType16>()
        {
            return NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12), typeof(ParameterType13), typeof(ParameterType14), typeof(ParameterType15), typeof(ParameterType16));
        }

        #endregion

        /// <summary>
        /// Pops parameterTypes.Length arguments from the stack, invokes the constructor on the given type that matches parameterTypes, and pushes a reference to the new object onto the stack.
        /// </summary>
        public Emit<DelegateType> NewObject(Type type, params Type[] parameterTypes)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (parameterTypes == null)
            {
                throw new ArgumentNullException("parameterTypes");
            }

            var allCons = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance);
            var cons =
                LinqAlternative
                    .Where(
                        allCons,
                        c =>
                            c.GetParameters().Length == parameterTypes.Length &&
                            LinqAlternative.Select(c.GetParameters(), (p, i) => p.ParameterType == parameterTypes[i]).Aggregate(true, (a, b) => a && b)
                    ).SingleOrDefault();

            if (cons == null)
            {
                throw new InvalidOperationException("Type " + type + " must have a constructor that matches parameters [" + BufferedILGenerator<DelegateType>.Join(", ", ((LinqArray<Type>)parameterTypes).AsEnumerable()) + "]");
            }

            return NewObject(cons);
        }

        /// <summary>
        /// Pops # of parameters to the given constructor arguments from the stack, invokes the constructor, and pushes a reference to the new object onto the stack.
        /// </summary>
        public Emit<DelegateType> NewObject(ConstructorInfo constructor)
        {
            if (constructor == null)
            {
                throw new ArgumentNullException("constructor");
            }

            var pts = ((LinqArray<ParameterInfo>)constructor.GetParameters()).Select(p => p.ParameterType).ToArray();

            return InnerNewObject(constructor, pts);
        }

        /// <summary>
        /// <para>Pops # of parameters from the stack, invokes the constructor, and pushes a reference to the new object onto the stack.</para>
        /// <para>
        /// This method is provided as ConstructorBuilder cannot be inspected for parameter information at runtime.  If the passed parameterTypes
        /// do not match the given constructor, the produced code will be invalid.
        /// </para>
        /// </summary>
        public Emit<DelegateType> NewObject(ConstructorBuilder constructor, Type[] parameterTypes)
        {
            if(constructor == null)
            {
                throw new ArgumentNullException("constructor");
            }

            if (parameterTypes == null)
            {
                throw new ArgumentNullException("parameterTypes");
            }

            return InnerNewObject(constructor, parameterTypes);
        }

        Emit<DelegateType> InnerNewObject(ConstructorInfo constructor, Type[] parameterTypes)
        {
            var expectedParams = ((LinqArray<Type>)parameterTypes).Select(p => TypeOnStack.Get(p)).Reverse().ToList();

            var makesType = TypeOnStack.Get(constructor.DeclaringType);

            var transitions =
                new[]
                {
                    new StackTransition(expectedParams.AsEnumerable(), new [] { makesType })
                };

            UpdateState(OpCodes.Newobj, constructor, Wrap(transitions, "NewObject"));

            return this;
        }
    }
}
