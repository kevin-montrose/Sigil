using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigil.Impl
{
    internal interface IOwned
    {
        object Owner { get; }
    }
}