using System;
using System.Reflection;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        #region Generic NewObject Constructor Finder Helpers

        /// <summary>
        /// Invokes the parameterless constructor of the given type, and pushes a reference to the new object onto the stack.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit NewObject<ReferenceType>()
        {
            InnerEmit.NewObject<ReferenceType>();
            return this;
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit NewObject<ReferenceType, ParameterType1>()
        {
            InnerEmit.NewObject<ReferenceType, ParameterType1>();
            return this;
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit NewObject<ReferenceType, ParameterType1, ParameterType2>()
        {
            InnerEmit.NewObject<ReferenceType, ParameterType1, ParameterType2>();
            return this;
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3>()
        {
            InnerEmit.NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3>();
            return this;
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4>()
        {
            InnerEmit.NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4>();
            return this;
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5>()
        {
            InnerEmit.NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5>();
            return this;
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6>()
        {
            InnerEmit.NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6>();
            return this;
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7>()
        {
            InnerEmit.NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7>();
            return this;
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8>()
        {
            InnerEmit.NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8>();
            return this;
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9>()
        {
            InnerEmit.NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9>();
            return this;
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10>()
        {
            InnerEmit.NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10>();
            return this;
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11>()
        {
            InnerEmit.NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11>();
            return this;
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12>()
        {
            InnerEmit.NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12>();
            return this;
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13>()
        {
            InnerEmit.NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13>();
            return this;
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14>()
        {
            InnerEmit.NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14>();
            return this;
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14, ParameterType15>()
        {
            InnerEmit.NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14, ParameterType15>();
            return this;
        }

        /// <summary>
        /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14, ParameterType15, ParameterType16>()
        {
            InnerEmit.NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14, ParameterType15, ParameterType16>();
            return this;
        }

        #endregion

        /// <summary>
        /// Pops parameterTypes.Length arguments from the stack, invokes the constructor on the given type that matches parameterTypes, and pushes a reference to the new object onto the stack.
        /// </summary>
        public Emit NewObject(Type type, params Type[] parameterTypes)
        {
            InnerEmit.NewObject(type, parameterTypes);
            return this;
        }

        /// <summary>
        /// Pops # of parameters to the given constructor arguments from the stack, invokes the constructor, and pushes a reference to the new object onto the stack.
        /// </summary>
        public Emit NewObject(ConstructorInfo constructor)
        {
            InnerEmit.NewObject(constructor);
            return this;
        }
    }
}
