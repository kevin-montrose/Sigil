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
    /// <summary>
    /// Helper for CIL generation that fails as soon as a sequence of instructions
    /// can be shown to be invalid.
    /// </summary>
    /// <typeparam name="DelegateType">The type of delegate being built</typeparam>
    public partial class Emit<DelegateType>
    {
        private static readonly ModuleBuilder Module;

        static Emit()
        {
            var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Sigil.Emit.DynamicAssembly"), AssemblyBuilderAccess.Run);
            Module = asm.DefineDynamicModule("DynamicModule");
        }

        private bool Invalidated;

        private BufferedILGenerator IL;
        private TypeOnStack ReturnType;
        private Type[] ParameterTypes;
        private DynamicMethod DynMethod;

        private StackState Stack;

        private List<Tuple<OpCode, StackState>> InstructionStream;

        private int NextLocalIndex = 0;

        private HashSet<Local> UnusedLocals;
        private HashSet<Label> UnusedLabels;
        private HashSet<Label> UnmarkedLabels;

        private Dictionary<StackState, Tuple<Label, int>> Branches;
        private Dictionary<Label, Tuple<StackState, int>> Marks;

        private Dictionary<int, Tuple<Label, BufferedILGenerator.UpdateOpCodeDelegate, OpCode>> BranchPatches;

        private Stack<ExceptionBlock> CurrentExceptionBlock;

        private Dictionary<ExceptionBlock, Tuple<int, int>> TryBlocks;
        private Dictionary<CatchBlock, Tuple<int, int>> CatchBlocks;
        private Dictionary<FinallyBlock, Tuple<int, int>> FinallyBlocks;

        private List<Tuple<int, TypeOnStack>> ReadonlyPatches;

        private DelegateType CreatedDelegate;

        private Emit(DynamicMethod dynMethod)
        {
            DynMethod = dynMethod;

            ReturnType = TypeOnStack.Get(DynMethod.ReturnType);
            ParameterTypes = 
                DynMethod
                    .GetParameters()
                    .Select(
                        p =>
                        {
                            var type = p.ParameterType;

                            // All 32-bit ints on the stack
                            if(type == typeof(byte) || type == typeof(sbyte) || type == typeof(short) || type == typeof(ushort) || type == typeof(uint))
                            {
                                type = typeof(int);
                            }

                            // Just a 64-bit int on the stack
                            if(type == typeof(ulong))
                            {
                                type = typeof(long);
                            }

                            return type;
                        }
                    ).ToArray();

            IL = new BufferedILGenerator(typeof(DelegateType));

            Stack = new StackState();
            InstructionStream = new List<Tuple<OpCode, StackState>>();
            UnusedLocals = new HashSet<Local>();
            UnusedLabels = new HashSet<Label>();
            UnmarkedLabels = new HashSet<Label>();

            Branches = new Dictionary<StackState, Tuple<Label, int>>();
            Marks = new Dictionary<Label, Tuple<StackState, int>>();

            BranchPatches = new Dictionary<int, Tuple<Label, BufferedILGenerator.UpdateOpCodeDelegate, OpCode>>();

            CurrentExceptionBlock = new Stack<ExceptionBlock>();

            TryBlocks = new Dictionary<ExceptionBlock, Tuple<int, int>>();
            CatchBlocks = new Dictionary<CatchBlock, Tuple<int, int>>();
            FinallyBlocks = new Dictionary<FinallyBlock, Tuple<int, int>>();

            ReadonlyPatches = new List<Tuple<int, TypeOnStack>>();
        }

        /// <summary>
        /// Returns a proxy for this Emit that exposes method names that more closely
        /// match the fields on System.Reflection.Emit.OpCodes.
        /// 
        /// IF you're well versed in ILGenerator, the shorthand version may be easier to use.
        /// </summary>
        public EmitShorthand<DelegateType> AsShorthand()
        {
            return new EmitShorthand<DelegateType>(this);
        }

        /// <summary>
        /// Converts the CIL stream into a delegate.
        /// 
        /// Validation that cannot be run until a method is finished is run, and various instructions
        /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
        /// 
        /// Once this method is called the Emit may no longer be modified.
        /// 
        /// `instructions` will be set to a representation of the instructions making up the returned delegate.
        /// Note that this string is typically *not* enough to regenerate the delegate, it is available for
        /// debugging purposes only.  Consumers may find it useful to log the instruction stream in case
        /// a future failure in the returned delegate fails validation (indicative of a bug in Sigil) or
        /// behaves unexpectedly (indicative of a logic bug in the consumer code).
        /// </summary>
        public DelegateType CreateDelegate(out string instructions)
        {
            if (CreatedDelegate != null)
            {
                instructions = null;
                return CreatedDelegate;
            }

            InjectTailCall();
            InjectReadOnly();
            PatchBranches();

            Validate();

            var il = DynMethod.GetILGenerator();
            instructions = IL.UnBuffer(il);

            CreatedDelegate = (DelegateType)(object)DynMethod.CreateDelegate(typeof(DelegateType));

            Invalidated = true;

            AutoNamer.Release(this);

            return CreatedDelegate;
        }

        /// <summary>
        /// Converts the CIL stream into a delegate.
        /// 
        /// Validation that cannot be run until a method is finished is run, and various instructions
        /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
        /// 
        /// Once this method is called the Emit may no longer be modified.
        /// </summary>
        public DelegateType CreateDelegate()
        {
            string ignored;
            return CreateDelegate(out ignored);
        }

        /// <summary>
        /// Creates a new Emit, using the provided name for the inner DynamicMethod.
        /// </summary>
        public static Emit<DelegateType> NewDynamicMethod(string name = null)
        {
            name = name ?? AutoNamer.Next("_DynamicMethod");

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

            var dynMethod = new DynamicMethod(name, returnType, parameterTypes, Module, skipVisibility: true);

            return new Emit<DelegateType>(dynMethod);
        }

        private void InsertInstruction(int index, OpCode instr)
        {
            IL.Insert(index, instr);

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

            foreach (var kv in TryBlocks.Where(kv => kv.Value.Item1 >= index).ToList())
            {
                TryBlocks[kv.Key] = Tuple.Create(kv.Value.Item1 + 1, kv.Value.Item2);
            }
            foreach (var kv in TryBlocks.Where(kv => kv.Value.Item2 >= index).ToList())
            {
                TryBlocks[kv.Key] = Tuple.Create(kv.Value.Item1, kv.Value.Item2 + 1);
            }

            foreach (var kv in CatchBlocks.Where(kv => kv.Value.Item1 >= index).ToList())
            {
                CatchBlocks[kv.Key] = Tuple.Create(kv.Value.Item1 + 1, kv.Value.Item2);
            }
            foreach (var kv in CatchBlocks.Where(kv => kv.Value.Item2 >= index).ToList())
            {
                CatchBlocks[kv.Key] = Tuple.Create(kv.Value.Item1, kv.Value.Item2 + 1);
            }

            foreach (var kv in FinallyBlocks.Where(kv => kv.Value.Item1 >= index).ToList())
            {
                FinallyBlocks[kv.Key] = Tuple.Create(kv.Value.Item1 + 1, kv.Value.Item2);
            }
            foreach (var kv in FinallyBlocks.Where(kv => kv.Value.Item2 >= index).ToList())
            {
                FinallyBlocks[kv.Key] = Tuple.Create(kv.Value.Item1, kv.Value.Item2 + 1);
            }

            foreach (var elem in ReadonlyPatches.ToList())
            {
                if (elem.Item1 >= index)
                {
                    var update = Tuple.Create(elem.Item1 + 1, elem.Item2);

                    ReadonlyPatches.Remove(elem);
                    ReadonlyPatches.Add(elem);
                }
            }
        }

        private void UpdateStackAndInstrStream(OpCode instr, TypeOnStack addToStack, int pop, bool firstParamIsThis = false)
        {
            if (Invalidated)
            {
                throw new InvalidOperationException("Cannot modify Emit after a delegate has been generated from it");
            }

            if (pop > 0)
            {
                Stack = Stack.Pop(instr, pop, firstParamIsThis);
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

        private void UpdateState(OpCode instr, uint param, TypeOnStack addToStack = null, int pop = 0)
        {
            UpdateStackAndInstrStream(instr, addToStack, pop);

            IL.Emit(instr, param);
        }

        private void UpdateState(OpCode instr, long param, TypeOnStack addToStack = null, int pop = 0)
        {
            UpdateStackAndInstrStream(instr, addToStack, pop);

            IL.Emit(instr, param);
        }

        private void UpdateState(OpCode instr, ulong param, TypeOnStack addToStack = null, int pop = 0)
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

        private void UpdateState(OpCode instr, BufferedILGenerator.DefineLabelDelegate[] param, out BufferedILGenerator.UpdateOpCodeDelegate update, TypeOnStack addToStack = null, int pop = 0)
        {
            UpdateStackAndInstrStream(instr, addToStack, pop);

            IL.Emit(instr, param, out update);
        }

        private void UpdateState(OpCode instr, MethodInfo method, TypeOnStack addToStack = null, int pop = 0, bool firstParamIsThis = false)
        {
            UpdateStackAndInstrStream(instr, addToStack, pop, firstParamIsThis);

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

        private void UpdateState(OpCode instr, CallingConventions callConventions, Type returnType, Type[] parameterTypes, int pop = 0)
        {
            UpdateStackAndInstrStream(instr, returnType != typeof(void) ? TypeOnStack.Get(returnType) : null, pop);

            IL.Emit(instr, callConventions, returnType, parameterTypes);
        }
    }
}
