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
        public void NewObject<ReferenceType>()
        {
            NewObject(typeof(ReferenceType));
        }

        public void NewObject(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (type.IsValueType)
            {
                throw new SigilException("Type must be a ValueType", Stack);
            }

            var cons = type.GetConstructor(Type.EmptyTypes);
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
