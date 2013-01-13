using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Push a constant integer onto the stack.
        /// </summary>
        public void LoadConstant(int i)
        {
            Stack = Stack.Push(TypeOnStack.Get<int>());

            switch (i)
            {
                case -1: IL.Emit(OpCodes.Ldc_I4_M1); return;
                case 0: IL.Emit(OpCodes.Ldc_I4_0); return;
                case 1: IL.Emit(OpCodes.Ldc_I4_1); return;
                case 2: IL.Emit(OpCodes.Ldc_I4_2); return;
                case 3: IL.Emit(OpCodes.Ldc_I4_3); return;
                case 4: IL.Emit(OpCodes.Ldc_I4_4); return;
                case 5: IL.Emit(OpCodes.Ldc_I4_5); return;
                case 6: IL.Emit(OpCodes.Ldc_I4_6); return;
                case 7: IL.Emit(OpCodes.Ldc_I4_7); return;
                case 8: IL.Emit(OpCodes.Ldc_I4_8); return;
            }

            if (i >= 0 && i <= byte.MaxValue)
            {
                IL.Emit(OpCodes.Ldc_I4_S, i);
                return;
            }

            IL.Emit(OpCodes.Ldc_I4, i);
        }

        public void LoadConstant(long l)
        {
            Stack = Stack.Push(TypeOnStack.Get<long>());

            IL.Emit(OpCodes.Ldc_I8, l);
        }

        public void LoadConstant(float f)
        {
            Stack = Stack.Push(TypeOnStack.Get<StackFloat>());

            IL.Emit(OpCodes.Ldc_R4, f);
        }

        public void LoadConstant(double d)
        {
            Stack = Stack.Push(TypeOnStack.Get<StackFloat>());

            IL.Emit(OpCodes.Ldc_R8, d);
        }
    }
}
