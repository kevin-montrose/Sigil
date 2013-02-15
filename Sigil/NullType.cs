using System;

namespace Sigil
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
    public class NullType
    {
        private NullType() { }
    }
}
