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
        /// <summary>
        /// Expects a reference to an array and an index on the stack.
        /// 
        /// Pops both, and pushes the element in the array at the index onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadElement()
        {
            var top = Stack.Top(2);

            if (top == null)
            {
                throw new SigilVerificationException("LoadElement expects two values on the stack", IL, Stack);
            }

            var index = top[0];
            var array = top[1];

            if (index != TypeOnStack.Get<int>() && index != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilVerificationException("LoadElement expects an int or native int on the top of the stack, found " + index, IL, Stack);
            }

            if (array.IsReference || array.IsPointer || !array.Type.IsArray)
            {
                throw new SigilVerificationException("LoadElement expects an array as the second element on the stack, found " + array, IL, Stack);
            }

            if (array.Type.GetArrayRank() != 1)
            {
                throw new SigilVerificationException("LoadElement expects a 1-dimensional array, found " + array, IL, Stack);
            }

            OpCode? instr = null;
            var elemType = array.Type.GetElementType();

            if (elemType.IsPointer)
            {
                instr = OpCodes.Ldelem_I;
            }

            if (!elemType.IsValueType && !instr.HasValue)
            {
                instr = OpCodes.Ldelem_Ref;
            }

            if (elemType == typeof(sbyte) && !instr.HasValue)
            {
                instr = OpCodes.Ldelem_I1;
            }

            if (elemType == typeof(byte) && !instr.HasValue)
            {
                instr = OpCodes.Ldelem_U1;
            }

            if (elemType == typeof(short) && !instr.HasValue)
            {
                instr = OpCodes.Ldelem_I2;
            }

            if (elemType == typeof(ushort) && !instr.HasValue)
            {
                instr = OpCodes.Ldelem_U2;
            }

            if (elemType == typeof(int) && !instr.HasValue)
            {
                instr = OpCodes.Ldelem_I4;
            }

            if (elemType == typeof(uint) && !instr.HasValue)
            {
                instr = OpCodes.Ldelem_U4;
            }

            if ((elemType == typeof(long) || elemType == typeof(ulong)) && !instr.HasValue)
            {
                instr = OpCodes.Ldelem_I8;
            }

            if (elemType == typeof(float) && !instr.HasValue)
            {
                instr = OpCodes.Ldelem_R4;
            }

            if (elemType == typeof(double) && !instr.HasValue)
            {
                instr = OpCodes.Ldelem_R8;
            }

            if (!instr.HasValue)
            {
                UpdateState(OpCodes.Ldelem, elemType, TypeOnStack.Get(elemType), pop: 2);
                return this;
            }

            UpdateState(instr.Value, TypeOnStack.Get(elemType), pop: 2);

            return this;
        }
    }
}
