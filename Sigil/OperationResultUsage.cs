using Sigil.Impl;
using System.Collections.Generic;

namespace Sigil
{
    /// <summary>
    /// Represents an IL operation, and the subsequent operations that may use it's result.
    /// </summary>
    public sealed class OperationResultUsage
    {
        /// <summary>
        /// The operation that is producing a result.
        /// </summary>
        public Operation ProducesResult { get; private set; }

        /// <summary>
        /// The operations that may use the result produced by the ProducesResult operation.
        /// </summary>
        public IEnumerable<Operation> ResultUsedBy { get; private set; }

        internal OperationResultUsage(Operation producer, IEnumerable<Operation> users)
        {
            ProducesResult = producer;
            ResultUsedBy = LinqAlternative.ToList(users).AsEnumerable();
        }

        /// <summary>
        /// Returns a string representation of this OperationResultUsage.
        /// </summary>
        public override string ToString()
        {
            var users = string.Join(", ", LinqAlternative.Select(ResultUsedBy, r => r.ToString()).OrderBy(_ => _).ToArray());

            if (users.Length == 0)
            {
                return "(" + ProducesResult + ") result is unused";
            }

            return "(" + ProducesResult + ") result is used by (" + users + ")";
        }
    }
}
