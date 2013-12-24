using Sigil.Impl;
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
    public sealed class DisassembledOperations<DelegateType> : IEnumerable<Operation<DelegateType>>
    {
        /// <summary>
        /// The total number of operations that were decompiled.
        /// </summary>
        public int Count { get { return Operations.Count; } }

        /// <summary>
        /// The parameters the decompiled delegate takes.
        /// </summary>
        public IEnumerable<Parameter> Parameters { get; private set; }

        /// <summary>
        /// The locals the decompiled delegate declared and uses.
        /// </summary>
        public IEnumerable<Local> Locals { get; private set; }

        /// <summary>
        /// The labels the decompile delegate uses.
        /// </summary>
        public IEnumerable<Label> Labels { get; private set; }

        private object UsageLock = new object();
        private volatile IEnumerable<OperationResultUsage<DelegateType>> _Usage;
        /// <summary>
        /// Traces where values produced by certain operations are used.
        /// 
        /// This is roughly equivalent to having built the disassembled delegate via Sigil originally,
        /// and saving the results of TraceOperationResultUsage().
        /// </summary>
        public IEnumerable<OperationResultUsage<DelegateType>> Usage 
        {
            get
            {
                if (_Usage != null) return _Usage;

                lock (UsageLock)
                {
                    if (_Usage != null) return _Usage;

                    var e1 = EmitFrom(0, this.Count);
                    _Usage = e1.TraceOperationResultUsage();

                    return _Usage;
                }
            }
        }

        /// <summary>
        /// Returns true if a call to EmitAll will succeed.
        /// 
        /// This property will be false if the delegate that was disassembled closed over it's environment,
        /// thereby adding an implicit `this` that cannot be represented (and thus cannot be returned).
        /// </summary>
        public bool CanEmit { get; private set; }

        private List<Operation<DelegateType>> Operations;
        /// <summary>
        /// Returns the operation that would be emitted at the given index.
        /// </summary>
        public Operation<DelegateType> this[int index]
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

        internal DisassembledOperations(
            List<Operation<DelegateType>> ops, 
            IEnumerable<Parameter> ps, 
            IEnumerable<Local> locs,
            IEnumerable<Label> labels,
            bool canEmit)
        {
            Operations = ops;
            Parameters = ps;

            Locals = locs;
            Labels = labels;

            CanEmit = canEmit;

            foreach (var loc in Locals)
            {
                loc.SetOwner(this);
            }

            foreach(var lab in Labels)
            {
                lab.SetOwner(this);
            }
        }

        private void Apply(int i, Emit<DelegateType> emit)
        {
            if (i == 0)
            {
                foreach (var l in Locals)
                {
                    emit.DeclareLocal(l.LocalType, l.Name);
                }

                foreach (var l in Labels)
                {
                    emit.DefineLabel(l.Name);
                }
            }

            this[i].Apply(emit);
        }

        private Emit<DelegateType> EmitFrom(int from, int length, string name = null, ModuleBuilder module = null)
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

            var e1 =
                Emit<DelegateType>.DisassemblerDynamicMethod(
                    LinqAlternative.Select(Parameters, p => p.ParameterType).ToArray(),
                    name,
                    module
                );

            for (var i = 0; i < length; i++)
            {
                Apply(from + i, e1);
            }

            return e1;
        }

        private Emit<DelegateType> Emit(int length, string name = null, ModuleBuilder module = null)
        {
            if (!CanEmit)
            {
                throw new InvalidOperationException("Cannot emit this DisassembledOperations object, check CanEmit before calling any Emit methods");
            }

            if(length < 0 || length > Operations.Count)
            {
                throw new InvalidOperationException("length must be between 0 and "+Operations.Count+", inclusive; found "+length);
            }

            return EmitFrom(0, length, name, module);
        }

        /// <summary>
        /// Emits the disassembled instructions into a new Emit.
        /// </summary>
        public Emit<DelegateType> EmitAll(string name = null, ModuleBuilder module = null)
        {
            return Emit(this.Count, name, module);
        }

        /// <summary>
        /// Returns an enumerator which steps over the Operations that are in this DisassembledOperations.
        /// </summary>
        public IEnumerator<Operation<DelegateType>> GetEnumerator()
        {
            return Operations.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator which steps over the Operations that are in this DisassembledOperations.
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)Operations).GetEnumerator();
        }
    }
}
