using System;
using System.Collections.Generic;
using System.Linq;

namespace Sigil.Impl
{
    internal class StackTransition
    {
        public int PoppedCount { get { return PoppedFromStack.Count(); } }

        // on the stack, first item is on the top of the stack
        public IEnumerable<TypeOnStack> PoppedFromStack { get; private set; }

        // pushed onto the stack, first item is first pushed (ends up lowest on the stack)
        public IEnumerable<TypeOnStack> PushedToStack { get; private set; }

        public int? StackSizeMustBe { get; private set; }

        public bool IsDuplicate { get; private set; }

        public StackTransition(IEnumerable<Type> popped, IEnumerable<Type> pushed)
            : this
            (
                popped.Select(s => TypeOnStack.Get(s)),
                pushed.Select(s => TypeOnStack.Get(s))
            )
        { }

        public StackTransition(int sizeMustBe)
            : this(new TypeOnStack[0], new TypeOnStack[0])
        {
            StackSizeMustBe = sizeMustBe;
        }

        public StackTransition(bool isDuplicate)
            : this(new TypeOnStack[0], new[] { TypeOnStack.Get<WildcardType>() })
        {
            IsDuplicate = isDuplicate;
        }

        public StackTransition(IEnumerable<TypeOnStack> popped, IEnumerable<TypeOnStack> pushed)
        {
            PoppedFromStack = popped.ToList().AsReadOnly();
            PushedToStack = pushed.ToList().AsReadOnly();
        }

        public override string ToString()
        {
            return "(" + string.Join(", ", PoppedFromStack.Select(p => p.ToString()).ToArray()) + ") => (" + string.Join(", ", PushedToStack.Select(p => p.ToString()).ToArray()) + ")";
        }

        public static StackTransition[] None() { return new[] { new StackTransition(Type.EmptyTypes, Type.EmptyTypes) }; }
        public static StackTransition[] Push<PushType>() { return Push(typeof(PushType)); }
        public static StackTransition[] Push(Type pushType) { return Push(TypeOnStack.Get(pushType)); }
        public static StackTransition[] Push(TypeOnStack pushType) { return new[] { new StackTransition(new TypeOnStack[0], new[] { pushType }) }; }

        public static StackTransition[] Pop<PopType>() { return Pop(typeof(PopType)); }
        public static StackTransition[] Pop(Type popType) { return Pop(TypeOnStack.Get(popType)); }
        public static StackTransition[] Pop(TypeOnStack popType) { return new[] { new StackTransition(new[] { popType }, new TypeOnStack[0]) }; }
    }
}
