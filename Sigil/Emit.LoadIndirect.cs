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
        public void LoadIndirect<Type>()
        {
            LoadIndirect(typeof(Type));
        }

        public void LoadIndirect(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            var onStack = Stack.Top();

            if (onStack == null)
            {
                throw new SigilException("LoadIndirect expects a value on the stack, but it was empty", Stack);
            }

            var ptr = onStack[0];

            if (!ptr.IsPointer && !ptr.IsReference && ptr != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilException("LoadIndirect expects a pointer, reference, or native int on the stack, found " + ptr, Stack);
            }

            if (ptr.IsPointer || ptr.IsReference)
            {
                if (type != ptr.Type)
                {
                    throw new SigilException("LoadIndirect expected a pointer or reference to type " + type + ", but found " + ptr, Stack);
                }
            }

            OpCode? instr = null;

            if (type.IsPointer)
            {
                instr = OpCodes.Ldind_I;
            }

            if (!type.IsValueType)
            {
                instr = OpCodes.Ldind_Ref;
            }

            if (type == typeof(sbyte))
            {
                instr = OpCodes.Ldind_I1;
            }

            if (type == typeof(byte))
            {
                instr = OpCodes.Ldind_U1;
            }

            if (type == typeof(short))
            {
                instr = OpCodes.Ldind_I2;
            }

            if (type == typeof(ushort))
            {
                instr = OpCodes.Ldind_U2;
            }

            if (type == typeof(int))
            {
                instr = OpCodes.Ldind_I4;
            }

            if (type == typeof(uint))
            {
                instr = OpCodes.Ldind_U4;
            }

            if (type == typeof(long) || type == typeof(ulong))
            {
                instr = OpCodes.Ldind_I8;
            }

            if (type == typeof(float))
            {
                instr = OpCodes.Ldind_R4;
            }

            if (type == typeof(double))
            {
                instr = OpCodes.Ldind_R8;
            }

            if (!instr.HasValue)
            {
                throw new Exception("Couldn't infer proper Ldind* opcode from " + type);
            }

            UpdateState(instr.Value, TypeOnStack.Get(type), pop: 1);
        }
    }
}
