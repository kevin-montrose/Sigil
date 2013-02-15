using System;

namespace Sigil
{
    /// <summary>
    /// This type represents a "native int" on the stack.
    /// 
    /// The size of native int varies depending on the architecture an assembly is executed on.
    /// Raw pointers are often of type native int.
    /// 
    /// This type is exposed to allow for stack assertions containing native int via Emit.MarkLabel.
    /// </summary>
    public class NativeIntType
    {
        private NativeIntType() { }
    }
}
