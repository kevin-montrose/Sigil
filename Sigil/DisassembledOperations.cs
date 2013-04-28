using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Sigil
{
    /// <summary>
    /// Represents a decompiled delegate.
    /// 
    /// The operations of the decompiled delegate can be inspected, and it can be replayed to a new Emit.
    /// </summary>
    public sealed class DecompiledOperations<DelegateType>
    {
        public int Count { get { return Operations.Count; } }

        public IEnumerable<Parameter> Parameters { get; private set; }
        public IEnumerable<Local> Locals { get; private set; }

        private List<Operation> Operations;
        public Operation this[int index]
        {
            get
            {
                if (index < 0 || index >= Operations.Count)
                {
                    if (Operations.Count == 0)
                    {
                        throw new IndexOutOfRangeException("DecompiledOperations is empty");
                    }

                    throw new IndexOutOfRangeException("Expected index between 0 and " + (Operations.Count - 1) + ", inclusive; found " + index);
                }

                return Operations[index];
            }
        }

        internal DecompiledOperations(List<Operation> ops, IEnumerable<Parameter> ps, IEnumerable<Local> locs)
        {
            Operations = ops;
            Parameters = ps;
            Locals = locs;
        }

        public void ContinueEmitFrom(Emit<DelegateType> emit, int from, int length)
        {
            if (from < 0 || from > Operations.Count)
            {
                throw new InvalidOperationException("from must be between 0 and " + Operations.Count + ", inclusive; found " + from);
            }

            if (length < 0)
            {
                throw new InvalidOperationException("length must be non-negative; found " + length);
            }

            if (from + length > Operations.Count)
            {
                throw new InvalidOperationException("from + length must be less than " + Operations.Count + "; found " + (from + length));
            }

            for (var i = 0; i <= length; i++)
            {
                this[from + length].Apply(emit);
            }
        }

        public void ContinueEmitAllFrom(Emit<DelegateType> emit, int from)
        {
            if (from < 0 || from > Operations.Count)
            {
                throw new InvalidOperationException("from must be between 0 and " + Operations.Count + ", inclusive; found " + from);
            }

            ContinueEmitFrom(emit, from, this.Count - from);
        }

        public Emit<DelegateType> EmitFrom(int from, int length, string name = null, ModuleBuilder module = null, ValidationOptions validationOptions = ValidationOptions.All)
        {
            if (from < 0 || from > Operations.Count)
            {
                throw new InvalidOperationException("from must be between 0 and " + Operations.Count + ", inclusive; found " + from);
            }

            if (length < 0)
            {
                throw new InvalidOperationException("length must be non-negative; found " + length);
            }

            if (from + length > Operations.Count)
            {
                throw new InvalidOperationException("from + length must be less than " + Operations.Count + "; found " + (from + length));
            }

            var e1 = Emit<DelegateType>.NewDynamicMethod(name, module, validationOptions);

            for (var i = 0; i < length; i++)
            {
                this[from + i].Apply(e1);
            }

            return e1;
        }

        public Emit<DelegateType> Emit(int length, string name = null, ModuleBuilder module = null, ValidationOptions validationOptions = ValidationOptions.All)
        {
            if(length < 0 || length > Operations.Count)
            {
                throw new InvalidOperationException("length must be between 0 and "+Operations.Count+", inclusive; found "+length);
            }

            return EmitFrom(0, length, name, module, validationOptions);
        }

        public Emit<DelegateType> EmitAll(string name = null, ModuleBuilder module = null, ValidationOptions validationOptions = ValidationOptions.All)
        {
            return Emit(this.Count, name, module, validationOptions);
        }

        public Emit<DelegateType> EmitAllFrom(int from, string name = null, ModuleBuilder module = null, ValidationOptions validationOptions = ValidationOptions.All)
        {
            return EmitFrom(from, Count - from, name, module, validationOptions);
        }
    }
}
