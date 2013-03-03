using System;

namespace Sigil
{
    /// <summary>
    /// Sigil performs validation on IL as it is emitted.
    /// 
    /// This can sometimes be costly, this enumeration is used to indicate what kind of validation should be
    /// performed.
    /// 
    /// By default, Sigil performs all available validation.  It is recommended that all validation be enabled
    /// when debugging code.
    /// 
    /// Note that all validation will still be performed when a delegate, method, or constructor is created.
    /// </summary>
    [Flags]
    public enum ValidationOptions
    {
        /// <summary>
        /// Defers all validation that can be deferred
        /// </summary>
        None = 0,

        /// <summary>
        /// Validates all control flow whenever a new control transfer or label is introduced
        /// </summary>
        ControlFlowImmediately = 1,

        /// <summary>
        /// Performs all validation immediately.
        /// 
        /// This is the default
        /// </summary>
        All = ControlFlowImmediately
    }
}
