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
        /// <summary>
        /// Push a 1 onto the stack if b is true, and 0 if false.
        /// 
        /// Pushed values are int32s.
        /// </summary>
        public Emit<DelegateType> LoadConstant(bool b)
        {
            return LoadConstant(b ? 1 : 0);
        }

        /// <summary>
        /// Push a constant int32 onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadConstant(int i)
        {
            switch (i)
            {
                case -1: UpdateState(OpCodes.Ldc_I4_M1, TypeOnStack.Get<int>()); return this;
                case 0: UpdateState(OpCodes.Ldc_I4_0, TypeOnStack.Get<int>()); return this;
                case 1: UpdateState(OpCodes.Ldc_I4_1, TypeOnStack.Get<int>()); return this;
                case 2: UpdateState(OpCodes.Ldc_I4_2, TypeOnStack.Get<int>()); return this;
                case 3: UpdateState(OpCodes.Ldc_I4_3, TypeOnStack.Get<int>()); return this;
                case 4: UpdateState(OpCodes.Ldc_I4_4, TypeOnStack.Get<int>()); return this;
                case 5: UpdateState(OpCodes.Ldc_I4_5, TypeOnStack.Get<int>()); return this;
                case 6: UpdateState(OpCodes.Ldc_I4_6, TypeOnStack.Get<int>()); return this;
                case 7: UpdateState(OpCodes.Ldc_I4_7, TypeOnStack.Get<int>()); return this;
                case 8: UpdateState(OpCodes.Ldc_I4_8, TypeOnStack.Get<int>()); return this;
            }

            if (i >= sbyte.MinValue && i <= sbyte.MaxValue)
            {
                UpdateState(OpCodes.Ldc_I4_S, i, TypeOnStack.Get<int>());
                return this;
            }

            UpdateState(OpCodes.Ldc_I4, i, TypeOnStack.Get<int>());

            return this;
        }

        /// <summary>
        /// Push a constant int32 onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadConstant(uint i)
        {
            switch (i)
            {
                case uint.MaxValue: UpdateState(OpCodes.Ldc_I4_M1, TypeOnStack.Get<int>()); return this;
                case 0: UpdateState(OpCodes.Ldc_I4_0, TypeOnStack.Get<int>()); return this;
                case 1: UpdateState(OpCodes.Ldc_I4_1, TypeOnStack.Get<int>()); return this;
                case 2: UpdateState(OpCodes.Ldc_I4_2, TypeOnStack.Get<int>()); return this;
                case 3: UpdateState(OpCodes.Ldc_I4_3, TypeOnStack.Get<int>()); return this;
                case 4: UpdateState(OpCodes.Ldc_I4_4, TypeOnStack.Get<int>()); return this;
                case 5: UpdateState(OpCodes.Ldc_I4_5, TypeOnStack.Get<int>()); return this;
                case 6: UpdateState(OpCodes.Ldc_I4_6, TypeOnStack.Get<int>()); return this;
                case 7: UpdateState(OpCodes.Ldc_I4_7, TypeOnStack.Get<int>()); return this;
                case 8: UpdateState(OpCodes.Ldc_I4_8, TypeOnStack.Get<int>()); return this;
            }

            if (i <= sbyte.MaxValue)
            {
                UpdateState(OpCodes.Ldc_I4_S, i, TypeOnStack.Get<int>());
                return this;
            }

            UpdateState(OpCodes.Ldc_I4, i, TypeOnStack.Get<int>());

            return this;
        }

        /// <summary>
        /// Push a constant int64 onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadConstant(long l)
        {
            UpdateState(OpCodes.Ldc_I8, l, TypeOnStack.Get<long>());

            return this;
        }

        /// <summary>
        /// Push a constant int64 onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadConstant(ulong l)
        {
            UpdateState(OpCodes.Ldc_I8, l, TypeOnStack.Get<long>());

            return this;
        }

        /// <summary>
        /// Push a constant float onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadConstant(float f)
        {
            UpdateState(OpCodes.Ldc_R4, f, TypeOnStack.Get<float>());

            return this;
        }

        /// <summary>
        /// Push a constant double onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadConstant(double d)
        {
            UpdateState(OpCodes.Ldc_R8, d, TypeOnStack.Get<double>());

            return this;
        }

        /// <summary>
        /// Push a constant string onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadConstant(string str)
        {
            UpdateState(OpCodes.Ldstr, str, TypeOnStack.Get<string>());

            return this;
        }

        /// <summary>
        /// Push a constant RuntimeFieldHandle onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadConstant(FieldInfo field)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }

            UpdateState(OpCodes.Ldtoken, field, TypeOnStack.Get<RuntimeFieldHandle>());

            return this;
        }

        /// <summary>
        /// Push a constant RuntimeMethodHandle onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadConstant(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            UpdateState(OpCodes.Ldtoken, method, TypeOnStack.Get<RuntimeMethodHandle>());

            return this;
        }

        /// <summary>
        /// Push a constant RuntimeTypeHandle onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadConstant<Type>()
        {
            return LoadConstant(typeof(Type));
        }

        /// <summary>
        /// Push a constant RuntimeTypeHandle onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadConstant(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            UpdateState(OpCodes.Ldtoken, type, TypeOnStack.Get<RuntimeTypeHandle>());

            return this;
        }

        /// <summary>
        /// Loads a null reference onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadNull()
        {
            UpdateState(OpCodes.Ldnull, TypeOnStack.Get<NullType>());

            return this;
        }
    }
}
