using System;

namespace Sigil
{
    /// <summary>
    /// Sigil can perform optimizations to the emitted IL. This enum tells Sigil which optimizations to perform.
    /// </summary>
    [Flags]
    public enum OptimizationOptions
    {
        /// <summary>
        /// Perform no optional optimizations.
        /// </summary>
        None = 0,

        /// <summary>
        /// Tells Sigil to choose optimal branch instructions.
        /// </summary>
        EnableBranchPatching = 1 << 0,

        /// <summary>
        /// Perform all optimizations.
        /// </summary>
        All = ~0
    }
}