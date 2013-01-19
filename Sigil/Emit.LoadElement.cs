using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public void LoadElement()
        {
            var top = Stack.Top(2);

            if (top == null)
            {
                throw new SigilException("LoadElement expects two values on the stack", Stack);
            }

            var index = top[0];
            var array = top[1];

            if (index != TypeOnStack.Get<int>() && index != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilException("LoadElement expects an int or native int on the top of the stack, found " + index, Stack);
            }

            if (array.IsReference || array.IsPointer || !array.Type.IsArray)
            {
                throw new SigilException("LoadElement expects an array as the second element on the stack, found " + array, Stack);
            }

            if (array.Type.GetArrayRank() != 1)
            {
                throw new SigilException("LoadElement expects a 1-dimensional array, found " + array, Stack);
            }

            OpCode? instr = null;
            var elemType = array.Type.GetElementType();

            if (elemType.IsPointer)
            {
                instr = OpCodes.Ldelem_I;
            }

            if (!elemType.IsValueType)
            {
                instr = OpCodes.Ldelem_Ref;
            }

            if (elemType == typeof(sbyte))
            {
                instr = OpCodes.Ldelem_I1;
            }

            if (elemType == typeof(byte))
            {
                instr = OpCodes.Ldelem_U1;
            }

            if (elemType == typeof(short))
            {
                instr = OpCodes.Ldelem_I2;
            }

            if (elemType == typeof(ushort))
            {
                instr = OpCodes.Ldelem_U2;
            }

            if (elemType == typeof(int))
            {
                instr = OpCodes.Ldelem_I4;
            }

            if (elemType == typeof(uint))
            {
                instr = OpCodes.Ldelem_U4;
            }

            if (elemType == typeof(long) || elemType == typeof(ulong))
            {
                instr = OpCodes.Ldelem_I8;
            }

            if (elemType == typeof(float))
            {
                instr = OpCodes.Ldelem_R4;
            }

            if(elemType == typeof(double))
            {
                instr = OpCodes.Ldelem_R8;
            }

            if (!instr.HasValue)
            {
                throw new Exception("Couldn't infer proper Ldelem* opcode from " + elemType);
            }

            UpdateState(instr.Value, TypeOnStack.Get(elemType), pop: 2);
        }
    }
}
