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
            switch (i)
            {
                case -1: IL.Emit(OpCodes.Ldc_I4_M1); UpdateState(OpCodes.Ldc_I4_M1, TypeOnStack.Get<int>()); return;
                case 0: IL.Emit(OpCodes.Ldc_I4_0); UpdateState(OpCodes.Ldc_I4_0, TypeOnStack.Get<int>()); return;
                case 1: IL.Emit(OpCodes.Ldc_I4_1); UpdateState(OpCodes.Ldc_I4_1, TypeOnStack.Get<int>()); return;
                case 2: IL.Emit(OpCodes.Ldc_I4_2); UpdateState(OpCodes.Ldc_I4_2, TypeOnStack.Get<int>()); return;
                case 3: IL.Emit(OpCodes.Ldc_I4_3); UpdateState(OpCodes.Ldc_I4_3, TypeOnStack.Get<int>()); return;
                case 4: IL.Emit(OpCodes.Ldc_I4_4); UpdateState(OpCodes.Ldc_I4_4, TypeOnStack.Get<int>()); return;
                case 5: IL.Emit(OpCodes.Ldc_I4_5); UpdateState(OpCodes.Ldc_I4_5, TypeOnStack.Get<int>()); return;
                case 6: IL.Emit(OpCodes.Ldc_I4_6); UpdateState(OpCodes.Ldc_I4_6, TypeOnStack.Get<int>()); return;
                case 7: IL.Emit(OpCodes.Ldc_I4_7); UpdateState(OpCodes.Ldc_I4_7, TypeOnStack.Get<int>()); return;
                case 8: IL.Emit(OpCodes.Ldc_I4_8); UpdateState(OpCodes.Ldc_I4_8, TypeOnStack.Get<int>()); return;
            }

            if (i >= 0 && i <= byte.MaxValue)
            {
                UpdateState(OpCodes.Ldc_I4_S, TypeOnStack.Get<int>());
                IL.Emit(OpCodes.Ldc_I4_S, i);
                return;
            }

            UpdateState(OpCodes.Ldc_I4, TypeOnStack.Get<int>());
            IL.Emit(OpCodes.Ldc_I4, i);
        }

        public void LoadConstant(long l)
        {
            UpdateState(OpCodes.Ldc_I8, TypeOnStack.Get<long>());

            IL.Emit(OpCodes.Ldc_I8, l);
        }

        public void LoadConstant(float f)
        {
            UpdateState(OpCodes.Ldc_R4, TypeOnStack.Get<StackFloat>());

            IL.Emit(OpCodes.Ldc_R4, f);
        }

        public void LoadConstant(double d)
        {
            UpdateState(OpCodes.Ldc_R8, TypeOnStack.Get<StackFloat>());

            IL.Emit(OpCodes.Ldc_R8, d);
        }
    }
}
