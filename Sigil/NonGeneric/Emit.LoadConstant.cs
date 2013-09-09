using System;
using System.Reflection;

namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Push a 1 onto the stack if b is true, and 0 if false.
        /// 
        /// Pushed values are int32s.
        /// </summary>
        public Emit LoadConstant(bool b)
        {
            InnerEmit.LoadConstant(b);
            return this;
        }

        /// <summary>
        /// Push a constant int32 onto the stack.
        /// </summary>
        public Emit LoadConstant(int i)
        {
            InnerEmit.LoadConstant(i);
            return this;
        }

        /// <summary>
        /// Push a constant int32 onto the stack.
        /// </summary>
        public Emit LoadConstant(uint i)
        {
            InnerEmit.LoadConstant(i);
            return this;
        }

        /// <summary>
        /// Push a constant int64 onto the stack.
        /// </summary>
        public Emit LoadConstant(long l)
        {
            InnerEmit.LoadConstant(l);
            return this;
        }

        /// <summary>
        /// Push a constant int64 onto the stack.
        /// </summary>
        public Emit LoadConstant(ulong l)
        {
            InnerEmit.LoadConstant(l);
            return this;
        }

        /// <summary>
        /// Push a constant float onto the stack.
        /// </summary>
        public Emit LoadConstant(float f)
        {
            InnerEmit.LoadConstant(f);
            return this;
        }

        /// <summary>
        /// Push a constant double onto the stack.
        /// </summary>
        public Emit LoadConstant(double d)
        {
            InnerEmit.LoadConstant(d);
            return this;
        }

        /// <summary>
        /// Push a constant string onto the stack.
        /// </summary>
        public Emit LoadConstant(string str)
        {
            InnerEmit.LoadConstant(str);
            return this;
        }

        /// <summary>
        /// Push a constant RuntimeFieldHandle onto the stack.
        /// </summary>
        public Emit LoadConstant(FieldInfo field)
        {
            InnerEmit.LoadConstant(field);
            return this;
        }

        /// <summary>
        /// Push a constant RuntimeMethodHandle onto the stack.
        /// </summary>
        public Emit LoadConstant(MethodInfo method)
        {
            InnerEmit.LoadConstant(method);
            return this;
        }

        /// <summary>
        /// Push a constant RuntimeTypeHandle onto the stack.
        /// </summary>
        public Emit LoadConstant<Type>()
        {
            InnerEmit.LoadConstant<Type>();
            return this;
        }

        /// <summary>
        /// Push a constant RuntimeTypeHandle onto the stack.
        /// </summary>
        public Emit LoadConstant(Type type)
        {
            InnerEmit.LoadConstant(type);
            return this;
        }

        /// <summary>
        /// Loads a null reference onto the stack.
        /// </summary>
        public Emit LoadNull()
        {
            InnerEmit.LoadNull();
            return this;
        }
    }
}
