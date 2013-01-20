using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;
using System.Reflection;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        private bool Invalidated;

        private BufferedILGenerator IL;
        private Type ReturnType;
        private Type[] ParameterTypes;
        private DynamicMethod DynMethod;

        private StackState Stack;

        private List<Tuple<OpCode, StackState>> InstructionStream;

        private int NextLocalIndex = 0;

        private HashSet<EmitLocal> UnusedLocals;
        private HashSet<EmitLabel> UnusedLabels;
        private HashSet<EmitLabel> UnmarkedLabels;

        private Dictionary<StackState, Tuple<EmitLabel, int>> Branches;
        private Dictionary<EmitLabel, Tuple<StackState, int>> Marks;

        private Dictionary<int, Tuple<EmitLabel, BufferedILGenerator.UpdateOpCodeDelegate, OpCode>> BranchPatches;

        private Dictionary<EmitExceptionBlock, Tuple<int, int>> TryBlocks;
        private Dictionary<EmitCatchBlock, Tuple<int, int>> CatchBlocks;

        private DelegateType CreatedDelegate;

        private Emit(DynamicMethod dynMethod)
        {
            DynMethod = dynMethod;

            ReturnType = DynMethod.ReturnType;
            ParameterTypes = DynMethod.GetParameters().Select(p => p.ParameterType).ToArray();

            IL = new BufferedILGenerator(typeof(DelegateType));

            Stack = new StackState();
            InstructionStream = new List<Tuple<OpCode, StackState>>();
            UnusedLocals = new HashSet<EmitLocal>();
            UnusedLabels = new HashSet<EmitLabel>();
            UnmarkedLabels = new HashSet<EmitLabel>();

            Branches = new Dictionary<StackState, Tuple<EmitLabel, int>>();
            Marks = new Dictionary<EmitLabel, Tuple<StackState, int>>();

            BranchPatches = new Dictionary<int, Tuple<EmitLabel, BufferedILGenerator.UpdateOpCodeDelegate, OpCode>>();

            TryBlocks = new Dictionary<EmitExceptionBlock, Tuple<int, int>>();
            CatchBlocks = new Dictionary<EmitCatchBlock, Tuple<int, int>>();
        }

        public DelegateType CreateDelegate()
        {
            if (CreatedDelegate != null) return CreatedDelegate;

            InjectTailCall();
            PatchBranches();

            Validate();

            var il = DynMethod.GetILGenerator();
            IL.UnBuffer(il);

            CreatedDelegate = (DelegateType)(object)DynMethod.CreateDelegate(typeof(DelegateType));

            Invalidated = true;

            return CreatedDelegate;
        }

        private void InsertInstruction(int index, OpCode instr)
        {
            IL.Insert(index, OpCodes.Tailcall);

            // We need to update our state to account for the new insertion
            foreach (var kv in Branches.Where(w => w.Value.Item2 >= index).ToList())
            {
                Branches[kv.Key] = Tuple.Create(kv.Value.Item1, kv.Value.Item2 + 1);
            }

            foreach (var kv in Marks.Where(w => w.Value.Item2 >= index).ToList())
            {
                Marks[kv.Key] = Tuple.Create(kv.Value.Item1, kv.Value.Item2 + 1);
            }

            var needUpdateKeys = BranchPatches.Keys.Where(k => k >= index).ToList();

            foreach (var key in needUpdateKeys)
            {
                var cur = BranchPatches[key];
                BranchPatches.Remove(key);
                BranchPatches[key + 1] = cur;
            }
        }

        private void UpdateStackAndInstrStream(OpCode instr, TypeOnStack addToStack, int pop)
        {
            if (Invalidated)
            {
                throw new InvalidOperationException("Cannot modify Emit after a delegate has been generated from it");
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

        private void UpdateState(OpCode instr, BufferedILGenerator.DeclareLocallDelegate param, TypeOnStack addToStack = null, int pop = 0)
        {
            UpdateStackAndInstrStream(instr, addToStack, pop);

            IL.Emit(instr, param);
        }

        private void UpdateState(OpCode instr, BufferedILGenerator.DefineLabelDelegate param, out BufferedILGenerator.UpdateOpCodeDelegate update, TypeOnStack addToStack = null, int pop = 0)
        {
            UpdateStackAndInstrStream(instr, addToStack, pop);

            IL.Emit(instr, param, out update);
        }

        private void UpdateState(OpCode instr, MethodInfo method, TypeOnStack addToStack = null, int pop = 0)
        {
            UpdateStackAndInstrStream(instr, addToStack, pop);

            IL.Emit(instr, method);
        }

        private void UpdateState(OpCode instr, ConstructorInfo cons, TypeOnStack addToStack = null, int pop = 0)
        {
            UpdateStackAndInstrStream(instr, addToStack, pop);

            IL.Emit(instr, cons);
        }

        private void UpdateState(OpCode instr, Type type, TypeOnStack addToStack = null, int pop = 0)
        {
            UpdateStackAndInstrStream(instr, addToStack, pop);

            IL.Emit(instr, type);
        }

        private void UpdateState(OpCode instr, FieldInfo field, TypeOnStack addToStack = null, int pop = 0)
        {
            UpdateStackAndInstrStream(instr, addToStack, pop);

            IL.Emit(instr, field);
        }

        private void UpdateState(OpCode instr, string str, TypeOnStack addToStack = null, int pop = 0)
        {
            UpdateStackAndInstrStream(instr, addToStack, pop);

            IL.Emit(instr, str);
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

            var dynMethod = new DynamicMethod(name, returnType, parameterTypes, restrictedSkipVisibility: true);

            return new Emit<DelegateType>(dynMethod);
        }
    }
}
