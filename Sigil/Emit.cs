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
        private CallingConventions CallingConventions;
        
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

        private EmitShorthand<DelegateType> Shorthand;

        // These can only ever be set if we're building a DynamicMethod
        private DelegateType CreatedDelegate;
        private DynamicMethod DynMethod;

        // These can only ever be set if we're building a MethodBuilder
        private MethodBuilder MtdBuilder;
        private bool MethodBuilt;

        /// <summary>
        /// Returns true if this Emit can make use of unverifiable instructions.
        /// </summary>
        public bool AllowsUnverifiableCIL { get; private set; }

        //private Emit(DynamicMethod dynMethod, bool allowUnverifiable)
        private Emit(CallingConventions callConvention, Type returnType, Type[] parameterTypes, bool allowUnverifiable)
        {
            CallingConventions = callConvention;

            AllowsUnverifiableCIL = allowUnverifiable;

            ReturnType = TypeOnStack.Get(returnType);
            ParameterTypes = 
                parameterTypes
                    .Select(
                        type =>
                        {
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

            Shorthand = new EmitShorthand<DelegateType>(this);
        }

        /// <summary>
        /// Returns a proxy for this Emit that exposes method names that more closely
        /// match the fields on System.Reflection.Emit.OpCodes.
        /// 
        /// IF you're well versed in ILGenerator, the shorthand version may be easier to use.
        /// </summary>
        public EmitShorthand<DelegateType> AsShorthand()
        {
            return Shorthand;
        }

        private void Seal()
        {
            InjectTailCall();
            InjectReadOnly();
            PatchBranches();

            Validate();

            Invalidated = true;
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
        /// the returned delegate fails validation (indicative of a bug in Sigil) or
        /// behaves unexpectedly (indicative of a logic bug in the consumer code).
        /// </summary>
        public DelegateType CreateDelegate(out string instructions)
        {
            if (DynMethod == null)
            {
                throw new InvalidOperationException("Emit was not created to build a DynamicMethod, thus CreateDelegate cannot be called");
            }

            if (CreatedDelegate != null)
            {
                instructions = null;
                return CreatedDelegate;
            }

            Seal();

            var il = DynMethod.GetILGenerator();
            instructions = IL.UnBuffer(il);

            CreatedDelegate = (DelegateType)(object)DynMethod.CreateDelegate(typeof(DelegateType));

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
        /// Writes the CIL stream out to the MethodBuilder used to create this Emit.
        /// 
        /// Validation that cannot be run until a method is finished is run, and various instructions
        /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
        /// 
        /// Once this method is called the Emit may no longer be modified.
        /// 
        /// `instructions` will be set to a representation of the instructions making up the returned delegate.
        /// Note that this string is typically *not* enough to regenerate the delegate, it is available for
        /// debugging purposes only.  Consumers may find it useful to log the instruction stream in case
        /// the returned delegate fails validation (indicative of a bug in Sigil) or
        /// behaves unexpectedly (indicative of a logic bug in the consumer code).
        /// </summary>
        public void CreateMethod(out string instructions)
        {
            if (MtdBuilder == null)
            {
                throw new InvalidOperationException("Emit was not created to build a method, thus CreateMethod cannot be called");
            }

            if (MethodBuilt)
            {
                instructions = null;
                return;
            }

            MethodBuilt = true;

            var il = MtdBuilder.GetILGenerator();
            instructions = IL.UnBuffer(il);

            AutoNamer.Release(this);
        }

        /// <summary>
        /// Writes the CIL stream out to the MethodBuilder used to create this Emit.
        /// 
        /// Validation that cannot be run until a method is finished is run, and various instructions
        /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
        /// 
        /// Once this method is called the Emit may no longer be modified.
        /// </summary>
        public void CreateMethod()
        {
            string ignored;
            CreateMethod(out ignored);
        }

        private static void CheckIsDelegate<CheckDelegateType>()
        {
            var delType = typeof(CheckDelegateType);

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
        }

        private static bool AllowsUnverifiableCode(ModuleBuilder m)
        {
            var canaryMethod = new DynamicMethod("__Canary" + Guid.NewGuid(), typeof(void), Type.EmptyTypes, m);
            var il = canaryMethod.GetILGenerator();
            il.Emit(OpCodes.Ldc_I4, 1024);
            il.Emit(OpCodes.Localloc);
            il.Emit(OpCodes.Pop);
            il.Emit(OpCodes.Ret);

            var d1 = (Action)canaryMethod.CreateDelegate(typeof(Action));

            try
            {
                d1();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a new Emit, optionally using the provided name and module for the inner DynamicMethod.
        /// 
        /// If name is not defined, a sane default is generated.
        /// 
        /// If module is not defined, a module with the same trust as the executing assembly is used instead.
        /// </summary>
        public static Emit<DelegateType> NewDynamicMethod(string name = null, ModuleBuilder module = null)
        {
            module = module ?? Module;

            name = name ?? AutoNamer.Next("_DynamicMethod");

            CheckIsDelegate<DelegateType>();

            var delType = typeof(DelegateType);

            var invoke = delType.GetMethod("Invoke");
            var returnType = invoke.ReturnType;
            var parameterTypes = invoke.GetParameters().Select(s => s.ParameterType).ToArray();

            var dynMethod = new DynamicMethod(name, returnType, parameterTypes, module, skipVisibility: true);

            var ret = new Emit<DelegateType>(dynMethod.CallingConvention, returnType, parameterTypes, AllowsUnverifiableCode(module));
            ret.DynMethod = dynMethod;

            return ret;
        }

        /// <summary>
        /// Creates a new Emit, suitable for building a method on the given MethodBuilder.
        /// 
        /// The DelegateType and MethodBuilder must agree on return types, parameter types (including `this`), and parameter counts.
        /// 
        /// If you intend to use unveriable code, you must set allowUnverifiableCode to true.
        /// </summary>
        public static Emit<DelegateType> BuildMethod(TypeBuilder type, string name, MethodAttributes attributes, CallingConventions callingConvention, bool allowUnverifiableCode = false)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if ((attributes & ~(MethodAttributes.Abstract | MethodAttributes.Assembly | MethodAttributes.CheckAccessOnOverride | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.FamORAssem | MethodAttributes.Final | MethodAttributes.HasSecurity | MethodAttributes.HideBySig | MethodAttributes.MemberAccessMask | MethodAttributes.NewSlot | MethodAttributes.PinvokeImpl | MethodAttributes.Private | MethodAttributes.PrivateScope | MethodAttributes.Public | MethodAttributes.RequireSecObject | MethodAttributes.ReuseSlot | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.Static | MethodAttributes.UnmanagedExport | MethodAttributes.Virtual | MethodAttributes.VtableLayoutMask)) != 0)
            {
                throw new ArgumentException("Unrecognized flag in attributes");
            }

            if ((callingConvention & ~(CallingConventions.Any | CallingConventions.ExplicitThis | CallingConventions.HasThis | CallingConventions.Standard | CallingConventions.VarArgs)) != 0)
            {
                throw new ArgumentException("Unrecognized flag in callingConvention");
            }

            if (attributes.HasFlag(MethodAttributes.Static) && callingConvention.HasFlag(CallingConventions.HasThis))
            {
                throw new ArgumentException("Static methods cannot have a this reference");
            }

            CheckIsDelegate<DelegateType>();

            var delType = typeof(DelegateType);

            var invoke = delType.GetMethod("Invoke");
            var returnType = invoke.ReturnType;
            var parameterTypes = invoke.GetParameters().Select(s => s.ParameterType).ToArray();

            var methodBuilder = type.DefineMethod(name, attributes, callingConvention, returnType, parameterTypes);

            if (callingConvention.HasFlag(CallingConventions.HasThis))
            {
                // Shove `this` in front, can't require it because it doesn't exist yet!
                parameterTypes = new Type[] { type }.Union(parameterTypes).ToArray();
            }

            var ret = new Emit<DelegateType>(callingConvention, returnType, parameterTypes, allowUnverifiableCode);
            ret.MtdBuilder = methodBuilder;

            return ret;
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
