using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Takes a destination pointer, a source pointer as arguments.  Pops both off the stack.
        /// 
        /// Copies the given value type from the source to the destination.
        /// </summary>
        public Emit<DelegateType> CopyObject<ValueType>()
            where ValueType : struct
        {
            return CopyObject(typeof(ValueType));
        }

        /// <summary>
        /// Takes a destination pointer, a source pointer as arguments.  Pops both off the stack.
        /// 
        /// Copies the given value type from the source to the destination.
        /// </summary>
        public Emit<DelegateType> CopyObject(Type valueType)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException("valueType");
            }

            if (!valueType.IsValueType)
            {
                throw new ArgumentException("CopyObject expects a ValueType; found " + valueType);
            }

            var onStack = Stack.Top(2);

            if (onStack == null)
            {
                FailStackUnderflow(2);
            }

            var dest = onStack[1];
            var source = onStack[0];

            if (!source.IsPointer && !source.IsReference && source != TypeOnStack.Get<NativeIntType>())
            {
                throw new SigilVerificationException("CopyObject expects the source value to be a pointer, reference, or native int; found " + source, IL.Instructions(LocalsByIndex), Stack, 0);
            }

            if (!dest.IsPointer && !dest.IsReference && dest != TypeOnStack.Get<NativeIntType>())
            {
                throw new SigilVerificationException("CopyObject expects the destination value to be a pointer, reference, or native int; found " + dest, IL.Instructions(LocalsByIndex), Stack, 1);
            }

            if (source != dest)
            {
                throw new SigilVerificationException("CopyObject expects the source and destination types to match; found " + source + " and " + dest, IL.Instructions(LocalsByIndex), Stack, 0, 1);
            }

            var transitions =
                new[] 
                {
                    new StackTransition(new [] { typeof(NativeIntType), typeof(NativeIntType) }, Type.EmptyTypes),
                    new StackTransition(new [] { valueType.MakePointerType(), typeof(NativeIntType) }, Type.EmptyTypes),
                    new StackTransition(new [] { valueType.MakeByRefType(), typeof(NativeIntType) }, Type.EmptyTypes),
                    new StackTransition(new [] { typeof(NativeIntType), valueType.MakePointerType() }, Type.EmptyTypes),
                    new StackTransition(new [] { valueType.MakePointerType(), valueType.MakePointerType() }, Type.EmptyTypes),
                    new StackTransition(new [] { valueType.MakeByRefType(), valueType.MakePointerType() }, Type.EmptyTypes),
                    new StackTransition(new [] { typeof(NativeIntType), valueType.MakeByRefType() }, Type.EmptyTypes),
                    new StackTransition(new [] { valueType.MakePointerType(), valueType.MakeByRefType() }, Type.EmptyTypes),
                    new StackTransition(new [] { valueType.MakeByRefType(), valueType.MakeByRefType() }, Type.EmptyTypes),
                };

            UpdateState(OpCodes.Cpobj, valueType, transitions.Wrap("CopyObject"), pop: 2);

            return this;
        }
    }
}
