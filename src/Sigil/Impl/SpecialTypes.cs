
namespace Sigil.Impl
{
    /// <summary>
    /// <para>This type represents a provably null value on the stack.</para>
    /// <para>Nulls typically arrive on the stack via LoadNull.</para>
    /// <para>Null can be assigned to any reference type safely, without the need for a CastClass.</para>
    /// <para>This type is exposed to allow for stack assertions containing null via Emit.MarkLabel.</para>
    /// </summary>
    internal sealed class NullType
    {
        private NullType() { }
    }

    /// <summary>
    /// <para>This type represents a "native int" on the stack.</para>
    /// <para>
    /// The size of native int varies depending on the architecture an assembly is executed on.
    /// Raw pointers are often of type native int.
    /// </para>
    /// <para>This type is exposed to allow for stack assertions containing native int via Emit.MarkLabel.</para>
    /// </summary>
    internal sealed class NativeIntType { }

    // Represents a type that *could be* anything
    internal sealed class WildcardType { }

    // Represents *any* pointer
    internal sealed class AnyPointerType { }

    // Represents *any* & type
    internal sealed class AnyByRefType { }

    // Something that's *only* assignable from object
    internal sealed class OnlyObjectType { }

    // Something that means "pop the entire damn stack" when encountered by the verifier
    internal sealed class PopAllType { }

    // Something that represents a * type that cannot stand on it's own, but be inferred from where it's used
    internal sealed class SamePointerType { }

    internal sealed class SameByRefType { }
}
