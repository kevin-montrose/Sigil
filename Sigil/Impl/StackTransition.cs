using System;
using System.Collections.Generic;

namespace Sigil.Impl
{
    internal delegate void VerificationCallback(LinqStack<LinqList<TypeOnStack>> currentStack, bool isBaseless);

    internal class StackTransition
    {
        public int PoppedCount { get { return PoppedFromStack.Length; } }

        // on the stack, first item is on the top of the stack
        public TypeOnStack[] PoppedFromStack { get; private set; }

        // pushed onto the stack, first item is first pushed (ends up lowest on the stack)
        public TypeOnStack[] PushedToStack { get; private set; }

        public int? StackSizeMustBe { get; private set; }

        public bool IsDuplicate { get; private set; }

        public VerificationCallback Before { get; private set; }

        public StackTransition(IEnumerable<Type> popped, IEnumerable<Type> pushed, VerificationCallback before = null)
            : this
            (
                LinqEnumerable<Type>.For(popped).Select(s => TypeOnStack.Get(s)).AsEnumerable(),
                LinqEnumerable<Type>.For(pushed).Select(s => TypeOnStack.Get(s)).AsEnumerable(),
                before
            )
        { }

        public StackTransition(int sizeMustBe)
            : this(new TypeOnStack[0], new TypeOnStack[0], null)
        {
            StackSizeMustBe = sizeMustBe;
        }

        public StackTransition(bool isDuplicate)
            : this(new TypeOnStack[0], new[] { TypeOnStack.Get<WildcardType>() }, null)
        {
            IsDuplicate = isDuplicate;
        }

        public StackTransition(IEnumerable<TypeOnStack> popped, IEnumerable<TypeOnStack> pushed, VerificationCallback before = null)
        {
            PoppedFromStack = LinqEnumerable<TypeOnStack>.For(popped).ToArray();
            PushedToStack = LinqEnumerable<TypeOnStack>.For(pushed).ToArray();
            
            Before = before;
        }

        public override string ToString()
        {
            return "(" + string.Join(", ", ((LinqArray<TypeOnStack>)PoppedFromStack).Select(p => p.ToString()).ToArray()) + ") => (" + string.Join(", ", ((LinqArray<TypeOnStack>)PushedToStack).Select(p => p.ToString()).ToArray()) + ")";
        }

        public static StackTransition[] None() { return new[] { new StackTransition(TypeHelpers.EmptyTypes, TypeHelpers.EmptyTypes) }; }
        public static StackTransition[] Push<PushType>() { return Push(typeof(PushType)); }
        public static StackTransition[] Push(Type pushType) { return Push(TypeOnStack.Get(pushType)); }
        public static StackTransition[] Push(TypeOnStack pushType) { return new[] { new StackTransition(new TypeOnStack[0], new[] { pushType }) }; }

        public static StackTransition[] Pop<PopType>() { return Pop(typeof(PopType)); }
        public static StackTransition[] Pop(Type popType) { return Pop(TypeOnStack.Get(popType)); }
        public static StackTransition[] Pop(TypeOnStack popType) { return new[] { new StackTransition(new[] { popType }, new TypeOnStack[0]) }; }
    }
}
