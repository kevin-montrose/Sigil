using System;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Emits IL that calls Console.WriteLine(string) for the given string.
        /// </summary>
        public Emit<DelegateType> WriteLine(string line)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            LoadConstant(line);
            Call(typeof (Console).GetMethod("WriteLine", new [] { typeof(string) }));
            return this;
        } 
    }
}