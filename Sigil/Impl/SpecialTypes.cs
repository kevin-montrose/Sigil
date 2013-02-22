using System;

namespace Sigil.Impl
{
    /// <summary>
    /// This type represents a provably null value on the stack.
    /// 
    /// Nulls typically arrive on the stack via LoadNull.
    /// 
    /// Null can be assigned to any reference type safely, without the need for a CastClass.
    /// 
    /// This type is exposed to allow for stack assertions containing null via Emit.MarkLabel.
    /// </summary>
    internal class NullType
    {
        private NullType() { }
    }

    /// <summary>
    /// This type represents a "native int" on the stack.
    /// 
    /// The size of native int varies depending on the architecture an assembly is executed on.
    /// Raw pointers are often of type native int.
    /// 
    /// This type is exposed to allow for stack assertions containing native int via Emit.MarkLabel.
    /// </summary>
    internal class NativeIntType
    {
        private NativeIntType() { }
    }

    // Represents a type that *could be* anything
    internal class WildcardType { }

    // Represents *any* pointer
    internal class AnyPointerType { }

    // Something that's *only* assignable from object
    internal class OnlyObjectType { }

    // Something that means "pop the entire damn stack" when encountered by the verifier
    internal class PopAllType { }
}
