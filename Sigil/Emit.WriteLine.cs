using System;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public Emit<DelegateType> WriteLine(string line)
        {
            LoadConstant(line);
            Call(typeof (Console).GetMethod("WriteLine", new [] { typeof(string) }));
            return this;
        } 
    }
}