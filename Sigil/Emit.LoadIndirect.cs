using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pops a pointer from the stack and pushes the value (of the given type) at that address onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadIndirect<Type>(bool isVolatile = false, int? unaligned = null)
        {
            return LoadIndirect(typeof(Type), isVolatile, unaligned);
        }

        /// <summary>
        /// Pops a pointer from the stack and pushes the value (of the given type) at that address onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadIndirect(Type type, bool isVolatile = false, int? unaligned = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (unaligned.HasValue && (unaligned != 1 && unaligned != 2 && unaligned != 4))
            {
                throw new ArgumentException("unaligned must be null, 1, 2, or 4");
            }

            var onStack = Stack.Top();

            if (onStack == null)
            {
                throw new SigilVerificationException("LoadIndirect expects a value on the stack, but it was empty", IL, Stack);
            }

            var ptr = onStack[0];

            if (!ptr.IsPointer && !ptr.IsReference && ptr != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilVerificationException("LoadIndirect expects a pointer, reference, or native int on the stack, found " + ptr, IL, Stack);
            }

            if (ptr.IsPointer || ptr.IsReference)
            {
                if (!type.IsAssignableFrom(TypeOnStack.Get(ptr.Type)))
                {
                    throw new SigilVerificationException("LoadIndirect expected a pointer or reference to type " + type + ", but found " + ptr, IL, Stack);
                }
            }

            OpCode? instr = null;

            if (type.IsPointer)
            {
                instr = OpCodes.Ldind_I;
            }

            if (!type.IsValueType && !instr.HasValue)
            {
                instr = OpCodes.Ldind_Ref;
            }

            if (type == typeof(sbyte) && !instr.HasValue)
            {
                instr = OpCodes.Ldind_I1;
            }

            if (type == typeof(byte) && !instr.HasValue)
            {
                instr = OpCodes.Ldind_U1;
            }

            if (type == typeof(short) && !instr.HasValue)
            {
                instr = OpCodes.Ldind_I2;
            }

            if (type == typeof(ushort) && !instr.HasValue)
            {
                instr = OpCodes.Ldind_U2;
            }

            if (type == typeof(int) && !instr.HasValue)
            {
                instr = OpCodes.Ldind_I4;
            }

            if (type == typeof(uint) && !instr.HasValue)
            {
                instr = OpCodes.Ldind_U4;
            }

            if ((type == typeof(long) || type == typeof(ulong)) && !instr.HasValue)
            {
                instr = OpCodes.Ldind_I8;
            }

            if (type == typeof(float) && !instr.HasValue)
            {
                instr = OpCodes.Ldind_R4;
            }

            if (type == typeof(double) && !instr.HasValue)
            {
                instr = OpCodes.Ldind_R8;
            }

            if (!instr.HasValue)
            {
                throw new InvalidOperationException("LoadIndirect cannot be used with " + type + ", LoadObject may be more appropriate");
            }

            if (isVolatile)
            {
                UpdateState(OpCodes.Volatile);
            }

            if (unaligned.HasValue)
            {
                UpdateState(OpCodes.Unaligned, unaligned.Value);
            }

            UpdateState(instr.Value, TypeOnStack.Get(type), pop: 1);

            return this;
        }
    }
}
