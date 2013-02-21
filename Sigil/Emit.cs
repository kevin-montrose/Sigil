using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Sigil
{
    internal delegate void LocalReusableDelegate(Local local);

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
            var asm = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Sigil.Emit.DynamicAssembly"), AssemblyBuilderAccess.Run);
            Module = asm.DefineDynamicModule("DynamicModule");
        }

        private bool Invalidated;

        private BufferedILGenerator IL;
        private TypeOnStack ReturnType;
        private Type[] ParameterTypes;
        private CallingConventions CallingConventions;
        
        private StackState Stack;

        private List<Tuple<OpCode, StackState>> InstructionStream;

        private ushort NextLocalIndex = 0;

        private Dictionary<int, Local> LocalsByIndex;

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

        private ConstructorBuilder ConstrBuilder;
        private bool ConstructorBuilt;

        /// <summary>
        /// Returns true if this Emit can make use of unverifiable instructions.
        /// </summary>
        public bool AllowsUnverifiableCIL { get; private set; }

        /// <summary>
        /// Returns the maxmimum number of items on the stack for the IL stream created with the current emit.
        /// 
        /// This is not the maximum that *can be placed*, but the maximum that actually are.
        /// </summary>
        public int MaxStackSize { get; private set; }

        private bool RequireTypeAssertion;

        private List<Local> FreedLocals { get; set; }

        private Dictionary<string, Local> CurrentLocals;

        /// <summary>
        /// Lookup for the locals currently in scope by name.
        /// 
        /// Locals go out of scope when released (by calling Dispose() directly, or via using) and go into scope
        /// immediately after a DeclareLocal()
        /// </summary>
        public LocalLookup Locals { get; private set; }

        private Dictionary<string, Label> CurrentLabels;

        /// <summary>
        /// Lookup for declared labels by name.
        /// </summary>
        public LabelLookup Labels { get; private set; }

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

            LocalsByIndex = new Dictionary<int, Local>();

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

            FreedLocals = new List<Local>();

            CurrentLocals = new Dictionary<string, Local>();
            Locals = new LocalLookup(CurrentLocals);

            CurrentLabels = new Dictionary<string, Label>();
            Labels = new LabelLookup(CurrentLabels);
        }

        /// <summary>
        /// Returns the information currently on the stack, ignoring the top "skip" items; the types are returned
        /// top-to-bottom, making this directly usable from MarkLabel.
        /// </summary>
        public IEnumerable<Type> GetStack(int skip = 0)
        {
            return Stack.GetTypes(skip);
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

        /// <summary>
        /// Returns a string representation of the CIL opcodes written to this Emit to date.
        /// 
        /// This method is meant for debugging purposes only.
        /// </summary>
        public string Instructions()
        {
            var ret = new StringBuilder();

            foreach (var line in IL.Instructions(LocalsByIndex))
            {
                ret.AppendLine(line);
            }

            return ret.ToString().Trim();
        }

        /// <summary>
        /// Returns the current instruction offset (effectively, the length of the CIL stream to date).
        /// 
        /// This does not necessarily increase monotonically, as rewrites can cause it to shrink.
        /// 
        /// Likewise the effect of any given call is not guaranteed to be the same under all circumstance, as current and future
        /// state may influence opcode choice.
        /// 
        /// This method is meant for debugging purposes only.
        /// </summary>
        public int ILOffset()
        {
            return IL.ByteDistance(0, IL.Index);
        }

        private void Seal(OptimizationOptions optimizationOptions)
        {
            InjectTailCall();
            InjectReadOnly();

            if ((optimizationOptions & OptimizationOptions.EnableBranchPatching) != 0)
            {
                PatchBranches();
            }

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
        public DelegateType CreateDelegate(out string instructions, OptimizationOptions optimizationOptions = OptimizationOptions.All)
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

            Seal(optimizationOptions);

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
        public DelegateType CreateDelegate(OptimizationOptions optimizationOptions = OptimizationOptions.All)
        {
            string ignored;
            return CreateDelegate(out ignored, optimizationOptions);
        }

        /// <summary>
        /// Writes the CIL stream out to the MethodBuilder used to create this Emit.
        /// 
        /// Validation that cannot be run until a method is finished is run, and various instructions
        /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
        /// 
        /// Once this method is called the Emit may no longer be modified.
        /// 
        /// Returns a MethodBuilder, which can be used to define overrides or for further inspection.
        /// 
        /// `instructions` will be set to a representation of the instructions making up the returned method.
        /// Note that this string is typically *not* enough to regenerate the method, it is available for
        /// debugging purposes only.  Consumers may find it useful to log the instruction stream in case
        /// the returned method fails validation (indicative of a bug in Sigil) or
        /// behaves unexpectedly (indicative of a logic bug in the consumer code).
        /// </summary>
        public MethodBuilder CreateMethod(out string instructions, OptimizationOptions optimizationOptions = OptimizationOptions.All)
        {
            if (MtdBuilder == null)
            {
                throw new InvalidOperationException("Emit was not created to build a method, thus CreateMethod cannot be called");
            }

            if (MethodBuilt)
            {
                instructions = null;
                return MtdBuilder;
            }

            Seal(optimizationOptions);

            MethodBuilt = true;

            var il = MtdBuilder.GetILGenerator();
            instructions = IL.UnBuffer(il);

            AutoNamer.Release(this);

            return MtdBuilder;
        }

        /// <summary>
        /// Writes the CIL stream out to the MethodBuilder used to create this Emit.
        /// 
        /// Validation that cannot be run until a method is finished is run, and various instructions
        /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
        /// 
        /// Once this method is called the Emit may no longer be modified.
        /// 
        /// Returns a MethodBuilder, which can be used to define overrides or for further inspection.
        /// </summary>
        public MethodBuilder CreateMethod(OptimizationOptions optimizationOptions = OptimizationOptions.All)
        {
            string ignored;
            return CreateMethod(out ignored, optimizationOptions);
        }

        /// <summary>
        /// Writes the CIL stream out to the ConstructorBuilder used to create this Emit.
        /// 
        /// Validation that cannot be run until a method is finished is run, and various instructions
        /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
        /// 
        /// Once this method is called the Emit may no longer be modified.
        /// 
        /// Returns a ConstructorBuilder, which can be used to define overrides or for further inspection.
        /// 
        /// `instructions` will be set to a representation of the instructions making up the returned constructor.
        /// Note that this string is typically *not* enough to regenerate the constructor, it is available for
        /// debugging purposes only.  Consumers may find it useful to log the instruction stream in case
        /// the returned constructor fails validation (indicative of a bug in Sigil) or
        /// behaves unexpectedly (indicative of a logic bug in the consumer code).
        /// </summary>
        public ConstructorBuilder CreateConstructor(out string instructions, OptimizationOptions optimizationOptions = OptimizationOptions.All)
        {
            if (ConstrBuilder == null)
            {
                throw new InvalidOperationException("Emit was not created to build a constructor, thus CreateConstructor cannot be called");
            }

            if (ConstructorBuilt)
            {
                instructions = null;
                return ConstrBuilder;
            }

            ConstructorBuilt = true;

            Seal(optimizationOptions);

            var il = ConstrBuilder.GetILGenerator();
            instructions = IL.UnBuffer(il);

            AutoNamer.Release(this);

            return ConstrBuilder;
        }

        /// <summary>
        /// Writes the CIL stream out to the ConstructorBuilder used to create this Emit.
        /// 
        /// Validation that cannot be run until a method is finished is run, and various instructions
        /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
        /// 
        /// Once this method is called the Emit may no longer be modified.
        /// 
        /// Returns a ConstructorBuilder, which can be used to define overrides or for further inspection.
        /// </summary>
        public ConstructorBuilder CreateConstructor(OptimizationOptions optimizationOptions = OptimizationOptions.All)
        {
            string ignored;
            return CreateConstructor(out ignored, optimizationOptions);
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

        private static bool AllowsUnverifiableCode(Module m)
        {
            return Attribute.IsDefined(m, typeof(System.Security.UnverifiableCodeAttribute));
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
        /// Creates a new Emit, optionally using the provided name and owner for the inner DynamicMethod.
        /// 
        /// If name is not defined, a sane default is generated.
        /// 
        /// If owner is not defined, a module with the same trust as the executing assembly is used instead.
        /// </summary>
        public static Emit<DelegateType> NewDynamicMethod(Type owner, string name = null)
        {
            if (owner == null) return NewDynamicMethod(name: name, module: null);
            name = name ?? AutoNamer.Next("_DynamicMethod");

            CheckIsDelegate<DelegateType>();

            var delType = typeof(DelegateType);

            var invoke = delType.GetMethod("Invoke");
            var returnType = invoke.ReturnType;
            var parameterTypes = invoke.GetParameters().Select(s => s.ParameterType).ToArray();

            var dynMethod = new DynamicMethod(name, returnType, parameterTypes, owner, skipVisibility: true);

            var ret = new Emit<DelegateType>(dynMethod.CallingConvention, returnType, parameterTypes, AllowsUnverifiableCode(owner.Module));
            ret.DynMethod = dynMethod;

            return ret;
        }

        private static void CheckAttributesAndConventions(MethodAttributes attributes, CallingConventions callingConvention)
        {
            if ((attributes & ~(MethodAttributes.Abstract | MethodAttributes.Assembly | MethodAttributes.CheckAccessOnOverride | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.FamORAssem | MethodAttributes.Final | MethodAttributes.HasSecurity | MethodAttributes.HideBySig | MethodAttributes.MemberAccessMask | MethodAttributes.NewSlot | MethodAttributes.PinvokeImpl | MethodAttributes.Private | MethodAttributes.PrivateScope | MethodAttributes.Public | MethodAttributes.RequireSecObject | MethodAttributes.ReuseSlot | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.Static | MethodAttributes.UnmanagedExport | MethodAttributes.Virtual | MethodAttributes.VtableLayoutMask)) != 0)
            {
                throw new ArgumentException("Unrecognized flag in attributes");
            }

            if ((callingConvention & ~(CallingConventions.Any | CallingConventions.ExplicitThis | CallingConventions.HasThis | CallingConventions.Standard | CallingConventions.VarArgs)) != 0)
            {
                throw new ArgumentException("Unrecognized flag in callingConvention");
            }

            if (HasFlag(attributes, MethodAttributes.Static) && HasFlag(callingConvention, CallingConventions.HasThis))
            {
                throw new ArgumentException("Static methods cannot have a this reference");
            }
        }
        static bool HasFlag(MethodAttributes value, MethodAttributes flag)
        {
            return (value & flag) != 0;
        }
        static bool HasFlag(CallingConventions value, CallingConventions flag)
        {
            return (value & flag) != 0;
        }

        /// <summary>
        /// Creates a new Emit, suitable for building a method on the given TypeBuilder.
        /// 
        /// The DelegateType and MethodBuilder must agree on return types, parameter types, and parameter counts.
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

            CheckAttributesAndConventions(attributes, callingConvention);

            CheckIsDelegate<DelegateType>();

            var delType = typeof(DelegateType);

            var invoke = delType.GetMethod("Invoke");
            var returnType = invoke.ReturnType;
            var parameterTypes = invoke.GetParameters().Select(s => s.ParameterType).ToArray();

            var methodBuilder = type.DefineMethod(name, attributes, callingConvention, returnType, parameterTypes);

            if (HasFlag(callingConvention, CallingConventions.HasThis))
            {
                // Shove `this` in front, can't require it because it doesn't exist yet!
                var pList = new List<Type>(parameterTypes);
                pList.Insert(0, type);

                parameterTypes = pList.ToArray();
            }

            var ret = new Emit<DelegateType>(callingConvention, returnType, parameterTypes, allowUnverifiableCode);
            ret.MtdBuilder = methodBuilder;

            return ret;
        }

        /// <summary>
        /// Convenience method for creating static methods.
        /// 
        /// Equivalent to calling to BuildMethod, but with MethodAttributes.Static set and CallingConventions.Standard.
        /// </summary>
        public static Emit<DelegateType> BuildStaticMethod(TypeBuilder type, string name, MethodAttributes attributes, bool allowUnverifiableCode = false)
        {
            return BuildMethod(type, name, attributes | MethodAttributes.Static, CallingConventions.Standard, allowUnverifiableCode);
        }

        /// <summary>
        /// Convenience method for creating instance methods.
        /// 
        /// Equivalent to calling to BuildMethod, but with CallingConventions.HasThis.
        /// </summary>
        public static Emit<DelegateType> BuildInstanceMethod(TypeBuilder type, string name, MethodAttributes attributes, bool allowUnverifiableCode = false)
        {
            return BuildMethod(type, name, attributes, CallingConventions.HasThis, allowUnverifiableCode);
        }

        /// <summary>
        /// Creates a new Emit, suitable for building a constructo on the given TypeBuilder.
        /// 
        /// The DelegateType and TypeBuilder must agree on parameter types and parameter counts.
        /// 
        /// If you intend to use unveriable code, you must set allowUnverifiableCode to true.
        /// </summary>
        public static Emit<DelegateType> BuildConstructor(TypeBuilder type, MethodAttributes attributes, CallingConventions callingConvention = CallingConventions.HasThis, bool allowUnverifiableCode = false)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            CheckAttributesAndConventions(attributes, callingConvention);

            if (!HasFlag(callingConvention, CallingConventions.HasThis))
            {
                throw new ArgumentException("Constructors always have a this reference");
            }

            CheckIsDelegate<DelegateType>();

            var delType = typeof(DelegateType);

            var invoke = delType.GetMethod("Invoke");
            var returnType = invoke.ReturnType;
            var parameterTypes = invoke.GetParameters().Select(s => s.ParameterType).ToArray();

            if (returnType != typeof(void))
            {
                throw new ArgumentException("DelegateType used must return void");
            }

            var constructorBuilder = type.DefineConstructor(attributes, callingConvention, parameterTypes);

            // Constructors always have a `this`
            var pList = new List<Type>(parameterTypes);
            pList.Insert(0, type);

            parameterTypes = pList.ToArray();

            var ret = new Emit<DelegateType>(callingConvention, typeof(void), parameterTypes, allowUnverifiableCode);
            ret.ConstrBuilder = constructorBuilder;

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

            if (RequireTypeAssertion)
            {
                throw new InvalidOperationException("Immediately after a Branch or Leave, the only legal operation is to mark a label with a type assertion");
            }

            if (pop > 0)
            {
                Stack = Stack.Pop(instr, pop, firstParamIsThis);
            }

            if (addToStack != null)
            {
                Stack = Stack.Push(addToStack);
            }

            MaxStackSize = Math.Max(MaxStackSize, Stack.Count());

            InstructionStream.Add(Tuple.Create(instr, Stack));
        }

        private void UpdateState(OpCode instr, TypeOnStack addToStack = null, int pop = 0)
        {
            UpdateStackAndInstrStream(instr, addToStack, pop);

            IL.Emit(instr);
        }

        private void UpdateState(OpCode instr, byte param, TypeOnStack addToStack = null, int pop = 0)
        {
            UpdateStackAndInstrStream(instr, addToStack, pop);

            IL.Emit(instr, param);
        }

        private void UpdateState(OpCode instr, short param, TypeOnStack addToStack = null, int pop = 0)
        {
            UpdateStackAndInstrStream(instr, addToStack, pop);

            IL.Emit(instr, param);
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

        private void UpdateState(OpCode instr, Local param, TypeOnStack addToStack = null, int pop = 0)
        {
            UpdateStackAndInstrStream(instr, addToStack, pop);

            IL.Emit(instr, param);
        }

        private void UpdateState(OpCode instr, Label param, out BufferedILGenerator.UpdateOpCodeDelegate update, TypeOnStack addToStack = null, int pop = 0)
        {
            UpdateStackAndInstrStream(instr, addToStack, pop);

            IL.Emit(instr, param, out update);
        }

        private void UpdateState(OpCode instr, Label[] param, out BufferedILGenerator.UpdateOpCodeDelegate update, TypeOnStack addToStack = null, int pop = 0)
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
