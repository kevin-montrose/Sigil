using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pushes a pointer to the given local onto the stack.
        /// 
        /// To create a local, use DeclareLocal.
        /// </summary>
        public Emit<DelegateType> LoadLocalAddress(Local local)
        {
            if (local == null)
            {
                throw new ArgumentNullException("local");
            }

            if (((IOwned)local).Owner != this)
            {
                if (((IOwned)local).Owner is DisassembledOperations<DelegateType>)
                {
                    return LoadLocalAddress(local.Name);
                }

                FailOwnership(local);
            }

            UnusedLocals.Remove(local);

            var type = local.StackType.Type;
            
            var ptrType = type.MakePointerType();

            if (local.Index >= byte.MinValue && local.Index <= byte.MaxValue)
            {
                byte asByte;
                unchecked
                {
                    asByte = (byte)local.Index;
                }

                UpdateState(OpCodes.Ldloca_S, asByte, Wrap(StackTransition.Push(ptrType), "LoadLocalAddress"));
                return this;
            }

            short asShort;
            unchecked
            {
                asShort = (short)local.Index;
            }

            UpdateState(OpCodes.Ldloca, asShort, Wrap(StackTransition.Push(ptrType), "LoadLocalAddress"));

            return this;
        }

        /// <summary>
        /// Pushes a pointer to the local with the given name onto the stack.
        /// </summary>
        public Emit<DelegateType> LoadLocalAddress(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            return LoadLocalAddress(Locals[name]);
        }
    }
}
