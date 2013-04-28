using System;
using System.Reflection;

namespace Sigil
{
    public sealed class Parameter
    {
        public int Position { get; private set; }
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
    }
}
