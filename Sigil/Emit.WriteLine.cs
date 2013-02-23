using System;
using System.Linq;

using Sigil.Impl;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Emits IL that calls Console.WriteLine(string) for the given string if no locals are passed.
        /// 
        /// If any locals are passed, line is treated as a format string and local values are used in a call
        /// to Console.WriteLine(string, object[]).
        /// </summary>
        public Emit<DelegateType> WriteLine(string line, params Local[] locals)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            if (locals == null)
            {
                throw new ArgumentNullException("locals");
            }

            var unowned = locals.Cast<IOwned>().FirstOrDefault(l => l.Owner != this);
            if (unowned != null)
            {
                FailOwnership(unowned);
            }

            if (locals.Length == 0)
            {
                LoadConstant(line);
                return Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }));
            }

            LoadConstant(line);

            LoadConstant(locals.Length);
            NewArray<object>();
            
            for (var i = 0; i < locals.Length; i++)
            {
                Duplicate();
                LoadConstant(i);
                LoadLocal(locals[i]);

                if (locals[i].LocalType.IsValueType)
                {
                    Box(locals[i].LocalType);
                }

                StoreElement<object>();
            }

            return Call(typeof(Console).GetMethod("WriteLine", new [] { typeof(string), typeof(object[]) }));
        }
    }
}