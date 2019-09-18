using System;
using System.Reflection;

namespace Sigil
{
    /// <summary>
    /// Represents a parameter to a decompiled delegate.
    /// </summary>
    public sealed class Parameter
    {
        /// <summary>
        /// The index of the parameter.
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// The type of the parameter.
        /// </summary>
        public Type ParameterType { get; private set; }

        internal Parameter(int pos, Type type)
        {
            Position = pos;
            ParameterType = type;
        }

        internal static Parameter For(ParameterInfo p)
        {
            return new Parameter(p.Position, p.ParameterType);
        }

        /// <summary>
        /// Returns a string representation of this Parameter.
        /// </summary>
        public override string ToString()
        {
            return "(" + ParameterType + ") at " + Position;
        }
    }
}
