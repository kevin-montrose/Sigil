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
        /// Pops a value, an index, and a reference to an array off the stack.  Places the given value into the given array at the given index.
        /// </summary>
        public Emit<DelegateType> StoreElement()
        {
            var onStack = Stack.Top(3);

            if (onStack == null)
            {
                throw new SigilVerificationException("StoreElement expects three parameters to be on the stack", IL, Stack);
            }

            var value = onStack[0];
            var index = onStack[1];
            var arr = onStack[2];

            if (arr.IsPointer || arr.IsReference || !arr.Type.IsArray || arr.Type.GetArrayRank() != 1)
            {
                throw new SigilVerificationException("StoreElement expects a rank one array, found " + arr, IL, Stack);
            }

            if (index != TypeOnStack.Get<int>() && index != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilVerificationException("StoreElement expects an index of type int or native int, found " + index, IL, Stack);
            }

            var elemType = arr.Type.GetElementType();

            if (!elemType.IsAssignableFrom(value))
            {
                throw new SigilVerificationException("StoreElement expects a value assignable to " + elemType + ", found " + value, IL, Stack);
            }

            OpCode? instr = null;

            if (elemType.IsPointer)
            {
                instr = OpCodes.Stelem_I;
            }

            if (!elemType.IsValueType && !instr.HasValue)
            {
                instr = OpCodes.Stelem_Ref;
            }

            if ((elemType == typeof(sbyte) || elemType == typeof(byte))  && !instr.HasValue)
            {
                instr = OpCodes.Stelem_I1;
            }

            if ((elemType == typeof(short) || elemType == typeof(ushort))  && !instr.HasValue)
            {
                instr = OpCodes.Stelem_I2;
            }

            if ((elemType == typeof(int) || elemType == typeof(uint))  && !instr.HasValue)
            {
                instr = OpCodes.Stelem_I4;
            }

            if ((elemType == typeof(long) || elemType == typeof(ulong))  && !instr.HasValue)
            {
                instr = OpCodes.Stelem_I8;
            }

            if (elemType == typeof(float) && !instr.HasValue)
            {
                instr = OpCodes.Stelem_R4;
            }

            if (elemType == typeof(double) && !instr.HasValue)
            {
                instr = OpCodes.Stelem_R8;
            }

            if (!instr.HasValue)
            {
                UpdateState(OpCodes.Stelem, elemType, pop:3);
                return this;
            }

            UpdateState(instr.Value, pop: 3);

            return this;
        }
    }
}
