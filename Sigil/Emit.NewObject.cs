using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        #region Generic NewObject Constructor Finder Helpers

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public void NewObject<ReferenceType>()
        {
            NewObject(typeof(ReferenceType));
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public void NewObject<ReferenceType, ParameterType1>()
        {
            NewObject(typeof(ReferenceType), typeof(ParameterType1));
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public void NewObject<ReferenceType, ParameterType1, ParameterType2>()
        {
            NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2));
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public void NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3>()
        {
            NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3));
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public void NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4>()
        {
            NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4));
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public void NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5>()
        {
            NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5));
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public void NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6>()
        {
            NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6));
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public void NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7>()
        {
            NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7));
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public void NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8>()
        {
            NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8));
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public void NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9>()
        {
            NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9));
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public void NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10>()
        {
            NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10));
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public void NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11>()
        {
            NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11));
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public void NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12>()
        {
            NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12));
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public void NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13>()
        {
            NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12), typeof(ParameterType13));
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public void NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14>()
        {
            NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12), typeof(ParameterType13), typeof(ParameterType14));
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public void NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14, ParameterType15>()
        {
            NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12), typeof(ParameterType13), typeof(ParameterType14), typeof(ParameterType15));
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public void NewObject<ReferenceType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14, ParameterType15, ParameterType16>()
        {
            NewObject(typeof(ReferenceType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12), typeof(ParameterType13), typeof(ParameterType14), typeof(ParameterType15), typeof(ParameterType16));
        }

        #endregion

        public void NewObject(Type type, params Type[] parameterTypes)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (parameterTypes == null)
            {
                throw new ArgumentNullException("parameterTypes");
            }

            var cons = type.GetConstructor(parameterTypes);
            if (cons == null)
            {
                throw new SigilException("Type must have a parameterless constructor", Stack);
            }

            NewObject(cons);
        }

        public void NewObject(ConstructorInfo constructor)
        {
            if (constructor == null)
            {
                throw new ArgumentNullException("constructor");
            }

            if (constructor.DeclaringType.IsValueType)
            {
                throw new SigilException("Cannot NewObject a ValueType", Stack);
            }

            var expectedParams = constructor.GetParameters().Select(p => TypeOnStack.Get(p.ParameterType)).ToList();

            var onStack = Stack.Top(expectedParams.Count);

            if (onStack == null)
            {
                throw new SigilException("Expected " + expectedParams.Count + " parameters to be on the stack", Stack);
            }

            // Parameters come off the Stack in reverse order
            var onStackR = onStack.Reverse().ToList();

            for (var i = 0; i < expectedParams.Count; i++)
            {
                var shouldBe = expectedParams[i];
                var actuallyIs = onStackR[i];

                if (!shouldBe.IsAssignableFrom(actuallyIs))
                {
                    throw new SigilException("Parameter #" + (i + 1) + " to " + constructor + " should be " + shouldBe + ", but found " + actuallyIs, Stack);
                }
            }

            var makesType = TypeOnStack.Get(constructor.DeclaringType);

            UpdateState(OpCodes.Newobj, constructor, makesType, pop: expectedParams.Count);
        }
    }
}
