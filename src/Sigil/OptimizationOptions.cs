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
        /// Choose optimal branch instructions.
        /// </summary>
        EnableBranchPatching = 1,

        /// <summary>
        /// Elide CastClass and IsInstance instructions which are no-ops, such as casting a System.String to a System.Object.
        /// </summary>
        EnableTrivialCastEliding = 2,

        /// <summary>
        /// Perform all optimizations.
        /// </summary>
        All = EnableBranchPatching | EnableTrivialCastEliding
    }
}