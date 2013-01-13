using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        private bool Invalidated;

        private ILGenerator IL;
        private Type ReturnType;
        private Type[] ParameterTypes;
        private DynamicMethod DynMethod;

        private StackState Stack;

        private List<Tuple<OpCode, StackState>> InstructionStream;

        private HashSet<EmitLocal> UnusedLocals;

        private Emit(DynamicMethod dynMethod)
        {
            DynMethod = dynMethod;

            IL = DynMethod.GetILGenerator();

            ReturnType = DynMethod.ReturnType;
            ParameterTypes = DynMethod.GetParameters().Select(p => p.ParameterType).ToArray();

            Stack = new StackState();
            InstructionStream = new List<Tuple<OpCode, StackState>>();
            UnusedLocals = new HashSet<EmitLocal>();
        }

        public DelegateType CreateDelegate()
        {
            Validate();

            var ret = (DelegateType)(object)DynMethod.CreateDelegate(typeof(DelegateType));

            Invalidated = true;

            return ret;
        }

        private void UpdateStackAndInstrStream(OpCode instr, TypeOnStack addToStack, int pop)
        {
            if (Invalidated)
            {
                throw new SigilException("Cannot modify Emit after a delegate has been generated from it", Stack);
            }

            if (pop > 0)
            {
                Stack = Stack.Pop(pop);
            }

            if (addToStack != null)
            {
                Stack = Stack.Push(addToStack);
            }

            InstructionStream.Add(Tuple.Create(instr, Stack));
        }

        private void UpdateState(OpCode instr, TypeOnStack addToStack = null, int pop = 0)
        {
            UpdateStackAndInstrStream(instr, addToStack, pop);

            IL.Emit(instr);
        }

        private void UpdateState(OpCode instr, int param, TypeOnStack addToStack = null, int pop = 0)
        {
            UpdateStackAndInstrStream(instr, addToStack, pop);

            IL.Emit(instr, param);
        }

        private void UpdateState(OpCode instr, long param, TypeOnStack addToStack = null, int pop = 0)
        {
            UpdateStackAndInstrStream(instr, addToStack, pop);

            IL.Emit(instr, param);
        }

        private void UpdateState(OpCode instr, float param, TypeOnStack addToStack = null, int pop = 0)
        {
            UpdateStackAndInstrStream(instr, addToStack, pop);

            IL.Emit(instr, param);
        }

        private void UpdateState(OpCode instr, double param, TypeOnStack addToStack = null, int pop = 0)
        {
            UpdateStackAndInstrStream(instr, addToStack, pop);

            IL.Emit(instr, param);
        }

        private void UpdateState(OpCode instr, LocalBuilder param, TypeOnStack addToStack = null, int pop = 0)
        {
            UpdateStackAndInstrStream(instr, addToStack, pop);

            IL.Emit(instr, param);
        }

        public static Emit<DelegateType> NewDynamicMethod(string name)
        {
            var delType = typeof(DelegateType);

            var baseTypes = new HashSet<Type>();
            baseTypes.Add(delType);
            var bType = delType.BaseType;
            while (bType != null)
            {
                baseTypes.Add(bType);
                bType = bType.BaseType;
            }

            if (!baseTypes.Contains(typeof(Delegate)))
            {
                throw new ArgumentException("DelegateType must be a delegate, found " + delType.FullName);
            }

            var invoke = delType.GetMethod("Invoke");
            var returnType = invoke.ReturnType;
            var parameterTypes = invoke.GetParameters().Select(s => s.ParameterType).ToArray();

            var dynMethod = new DynamicMethod(name, returnType, parameterTypes);

            return new Emit<DelegateType>(dynMethod);
        }
    }
}
