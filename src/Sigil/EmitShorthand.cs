using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Sigil
{
    /// <summary>
    /// <para>A version of Emit with shorter named versions of it's methods.</para>
    /// <para>Method names map more or less to OpCodes fields.</para>
    /// </summary>
    public class EmitShorthand<DelegateType>
    {
        private readonly Emit<DelegateType> InnerEmit;

        /// <summary>
        /// Returns true if this Emit can make use of unverifiable instructions.
        /// </summary>
        public bool AllowsUnverifiableCIL { get { return InnerEmit.AllowsUnverifiableCIL; } }

        /// <summary>
        /// <para>Returns the maxmimum number of items on the stack for the IL stream created with the current emit.</para>
        /// <para>This is not the maximum that *can be placed*, but the maximum that actually are.</para>
        /// </summary>
        public int MaxStackSize { get { return InnerEmit.MaxStackSize; } }

        /// <summary>
        /// <para>Lookup for the locals currently in scope by name.</para>
        /// <para>
        /// Locals go out of scope when released (by calling Dispose() directly, or via using) and go into scope
        /// immediately after a DeclareLocal()
        /// </para>
        /// </summary>
        public LocalLookup Locals { get { return InnerEmit.Locals; } }

        /// <summary>
        /// Lookup for declared labels by name.
        /// </summary>
        public LabelLookup Labels { get { return InnerEmit.Labels; } }

        internal EmitShorthand(Emit<DelegateType> inner)
        {
            InnerEmit = inner;
        }

        /// <summary>
        /// Returns the original Emit instance that AsShorthand() was called on.
        /// </summary>
        public Emit<DelegateType> AsLonghand()
        {
            return InnerEmit;
        }

        /// <summary>
        /// <para>Returns a string representation of the CIL opcodes written to this Emit to date.</para>
        /// <para>This method is meant for debugging purposes only.</para>
        /// </summary>
        public string Instructions()
        {
            return InnerEmit.Instructions();
        }

        /// <summary cref="M:Sigil.Emit`1.DeclareLocal``1(System.String, System.Boolean)" />
        public Local DeclareLocal<Type>(string name = null, bool initializeReused = true)
        {
            return DeclareLocal(typeof(Type), name, initializeReused);
        }

        /// <summary cref="M:Sigil.Emit`1.DeclareLocal``1(Sigil.Local, System.String, System.Boolean)" />
        public EmitShorthand<DelegateType> DeclareLocal<Type>(out Local local, string name = null, bool initializeReused = true)
        {
            local = DeclareLocal<Type>(name, initializeReused);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.DeclareLocal(System.Type, System.String, System.Boolean)" />
        public Local DeclareLocal(Type type, string name = null, bool initializeReused = true)
        {
            return InnerEmit.DeclareLocal(type, name, initializeReused);
        }

        /// <summary cref="M:Sigil.Emit`1.DeclareLocal(System.Type, Sigil.Local, System.String, System.Boolean)" />
        public EmitShorthand<DelegateType> DeclareLocal(Type type, out Local local, string name = null, bool initializeReused = true)
        {
            local = DeclareLocal(type, name, initializeReused);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.DefineLabel(System.String)" />
        public Label DefineLabel(string name = null)
        {
            return InnerEmit.DefineLabel(name);
        }

        /// <summary cref="M:Sigil.Emit`1.DefineLabel(System.String)" />
        public EmitShorthand<DelegateType> DefineLabel(out Label label, string name = null)
        {
            label = DefineLabel(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.MarkLabel(Sigil.Label, IEnumerable``1)" />
        public EmitShorthand<DelegateType> MarkLabel(Label label)
        {
            InnerEmit.MarkLabel(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.MarkLabel(System.String, IEnumerable``1)" />
        public EmitShorthand<DelegateType> MarkLabel(string name)
        {
            InnerEmit.MarkLabel(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.BeginExceptionBlock" />
        public ExceptionBlock BeginExceptionBlock()
        {
            return InnerEmit.BeginExceptionBlock();
        }

        /// <summary cref="M:Sigil.Emit`1.BeginExceptionBlock" />
        public EmitShorthand<DelegateType> BeginExceptionBlock(out ExceptionBlock forTry)
        {
            forTry = BeginExceptionBlock();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.BeginCatchBlock``1(Sigil.ExceptionBlock)" />
        public CatchBlock BeginCatchBlock<ExceptionType>(ExceptionBlock forTry)
        {
            return BeginCatchBlock(forTry, typeof(ExceptionType));
        }

        /// <summary cref="M:Sigil.Emit`1.BeginCatchBlock``1(Sigil.ExceptionBlock)" />
        public EmitShorthand<DelegateType> BeginCatchBlock<ExceptionType>(ExceptionBlock forTry, out CatchBlock tryCatch)
        {
            tryCatch = BeginCatchBlock<ExceptionType>(forTry);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.BeginCatchBlock(System.Type, Sigil.ExceptionBlock)" />
        public CatchBlock BeginCatchBlock(ExceptionBlock forTry, Type exceptionType)
        {
            return InnerEmit.BeginCatchBlock(forTry, exceptionType);
        }

        /// <summary cref="M:Sigil.Emit`1.BeginCatchBlock(System.Type, Sigil.ExceptionBlock)" />
        public EmitShorthand<DelegateType> BeginCatchBlock(ExceptionBlock forTry, Type exceptionType, out CatchBlock forCatch)
        {
            forCatch = BeginCatchBlock(forTry, exceptionType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.EndCatchBlock(Sigil.CatchBlock)" />
        public EmitShorthand<DelegateType> EndCatchBlock(CatchBlock forCatch)
        {
            InnerEmit.EndCatchBlock(forCatch);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.BeginFinallyBlock(Sigil.ExceptionBlock)" />
        public FinallyBlock BeginFinallyBlock(ExceptionBlock forTry)
        {
            return InnerEmit.BeginFinallyBlock(forTry);
        }

        /// <summary cref="M:Sigil.Emit`1.BeginFinallyBlock(Sigil.ExceptionBlock)" />
        public EmitShorthand<DelegateType> BeginFinallyBlock(ExceptionBlock forTry, out FinallyBlock forFinally)
        {
            forFinally = BeginFinallyBlock(forTry);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.EndFinallyBlock(Sigil.FinallyBlock)" />
        public EmitShorthand<DelegateType> EndFinallyBlock(FinallyBlock forFinally)
        {
            InnerEmit.EndFinallyBlock(forFinally);
            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.EndExceptionBlock(Sigil.ExceptionBlock)" />
        public EmitShorthand<DelegateType> EndExceptionBlock(ExceptionBlock forTry)
        {
            InnerEmit.EndExceptionBlock(forTry);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.CreateDelegate(Sigil.OptimizationOptions)" />
        public DelegateType CreateDelegate(OptimizationOptions optimizationOptions = OptimizationOptions.All)
        {
            return InnerEmit.CreateDelegate(optimizationOptions);
        }

        /// <summary cref="M:Sigil.Emit`1.CreateMethod" />
        public MethodBuilder CreateMethod()
        {
            return InnerEmit.CreateMethod();
        }

        /// <summary cref="M:Sigil.Emit`1.CreateConstructor" />
        public ConstructorBuilder CreateConstructor()
        {
            return InnerEmit.CreateConstructor();
        }

        /// <summary cref="M:Sigil.Emit`1.Add" />
        public EmitShorthand<DelegateType> Add()
        {
            InnerEmit.Add();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.AddOverflow" />
        public EmitShorthand<DelegateType> Add_Ovf()
        {
            InnerEmit.AddOverflow();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedAddOverflow" />
        public EmitShorthand<DelegateType> Add_Ovf_Un()
        {
            InnerEmit.UnsignedAddOverflow();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.And" />
        public EmitShorthand<DelegateType> And()
        {
            InnerEmit.And();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.BranchIfEqual(Sigil.Label)" />
        public EmitShorthand<DelegateType> Beq(Label label)
        {
            InnerEmit.BranchIfEqual(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.BranchIfEqual(System.String)" />
        public EmitShorthand<DelegateType> Beq(string name)
        {
            InnerEmit.BranchIfEqual(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.BranchIfGreaterOrEqual(Sigil.Label)" />
        public EmitShorthand<DelegateType> Bge(Label label)
        {
            InnerEmit.BranchIfGreaterOrEqual(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.BranchIfGreaterOrEqual(System.String)" />
        public EmitShorthand<DelegateType> Bge(string name)
        {
            InnerEmit.BranchIfGreaterOrEqual(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedBranchIfGreaterOrEqual(Sigil.Label)" />
        public EmitShorthand<DelegateType> Bge_Un(Label label)
        {
            InnerEmit.UnsignedBranchIfGreaterOrEqual(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedBranchIfGreaterOrEqual(System.String)" />
        public EmitShorthand<DelegateType> Bge_Un(string name)
        {
            InnerEmit.UnsignedBranchIfGreaterOrEqual(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.BranchIfGreater(Sigil.Label)" />
        public EmitShorthand<DelegateType> Bgt(Label label)
        {
            InnerEmit.BranchIfGreater(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.BranchIfGreater(System.String)" />
        public EmitShorthand<DelegateType> Bgt(string name)
        {
            InnerEmit.BranchIfGreater(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedBranchIfGreater(Sigil.Label)" />
        public EmitShorthand<DelegateType> Bgt_Un(Label label)
        {
            InnerEmit.UnsignedBranchIfGreater(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedBranchIfGreater(System.String)" />
        public EmitShorthand<DelegateType> Bgt_Un(string name)
        {
            InnerEmit.UnsignedBranchIfGreater(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.BranchIfLessOrEqual(Sigil.Label)" />
        public EmitShorthand<DelegateType> Ble(Label label)
        {
            InnerEmit.BranchIfLessOrEqual(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.BranchIfLessOrEqual(System.String)" />
        public EmitShorthand<DelegateType> Ble(string name)
        {
            InnerEmit.BranchIfLessOrEqual(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedBranchIfLessOrEqual(Sigil.Label)" />
        public EmitShorthand<DelegateType> Ble_Un(Label label)
        {
            InnerEmit.UnsignedBranchIfLessOrEqual(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedBranchIfLessOrEqual(System.String)" />
        public EmitShorthand<DelegateType> Ble_Un(string name)
        {
            InnerEmit.UnsignedBranchIfLessOrEqual(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.BranchIfLess(Sigil.Label)" />
        public EmitShorthand<DelegateType> Blt(Label label)
        {
            InnerEmit.BranchIfLess(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.BranchIfLess(System.String)" />
        public EmitShorthand<DelegateType> Blt(string name)
        {
            InnerEmit.BranchIfLess(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedBranchIfLess(Sigil.Label)" />
        public EmitShorthand<DelegateType> Blt_Un(Label label)
        {
            InnerEmit.UnsignedBranchIfLess(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedBranchIfLess(System.String)" />
        public EmitShorthand<DelegateType> Blt_Un(string name)
        {
            InnerEmit.UnsignedBranchIfLess(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedBranchIfNotEqual(Sigil.Label)" />
        public EmitShorthand<DelegateType> Bne_Un(Label label)
        {
            InnerEmit.UnsignedBranchIfNotEqual(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedBranchIfNotEqual(System.String)" />
        public EmitShorthand<DelegateType> Bne_Un(string name)
        {
            InnerEmit.UnsignedBranchIfNotEqual(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Box``1()" />
        public EmitShorthand<DelegateType> Box<ValueType>()
        {
            Box(typeof(ValueType));

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Box(System.Type)" />
        public EmitShorthand<DelegateType> Box(Type valueType)
        {
            InnerEmit.Box(valueType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Branch(Sigil.Label)" />
        public EmitShorthand<DelegateType> Br(Label label)
        {
            InnerEmit.Branch(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Branch(System.String)" />
        public EmitShorthand<DelegateType> Br(string name)
        {
            InnerEmit.Branch(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Break" />
        public EmitShorthand<DelegateType> Break()
        {
            InnerEmit.Break();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.BranchIfFalse(Sigil.Label)" />
        public EmitShorthand<DelegateType> Brfalse(Label label)
        {
            InnerEmit.BranchIfFalse(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.BranchIfFalse(System.String)" />
        public EmitShorthand<DelegateType> Brfalse(string name)
        {
            InnerEmit.BranchIfFalse(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.BranchIfTrue(Sigil.Label)" />
        public EmitShorthand<DelegateType> Brtrue(Label label)
        {
            InnerEmit.BranchIfTrue(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.BranchIfTrue(System.String)" />
        public EmitShorthand<DelegateType> Brtrue(string name)
        {
            InnerEmit.BranchIfTrue(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Call(System.Reflection.MethodInfo)" />
        public EmitShorthand<DelegateType> Call(MethodInfo method)
        {
            InnerEmit.Call(method);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.CallIndirect(System.Reflection.CallingConventions,System.Type,System.Type[])" />
        public EmitShorthand<DelegateType> Calli(CallingConventions callingConvention, Type returnType, params Type[] parameterTypes)
        {
            InnerEmit.CallIndirect(callingConvention, returnType, parameterTypes);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.CallVirtual(System.Reflection.MethodInfo, System.Type)" />
        public EmitShorthand<DelegateType> Callvirt(MethodInfo method, Type constrained = null)
        {
            InnerEmit.CallVirtual(method, constrained);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.CastClass``1" />
        public EmitShorthand<DelegateType> Castclass<ReferenceType>()
        {
            Castclass(typeof(ReferenceType));

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.CastClass(System.Type)" />
        public EmitShorthand<DelegateType> Castclass(Type referenceType)
        {
            InnerEmit.CastClass(referenceType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.CompareEqual" />
        public EmitShorthand<DelegateType> Ceq()
        {
            InnerEmit.CompareEqual();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.CompareGreaterThan" />
        public EmitShorthand<DelegateType> Cgt()
        {
            InnerEmit.CompareGreaterThan();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedCompareGreaterThan" />
        public EmitShorthand<DelegateType> Cgt_Un()
        {
            InnerEmit.UnsignedCompareGreaterThan();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.CheckFinite" />
        public EmitShorthand<DelegateType> Ckfinite()
        {
            InnerEmit.CheckFinite();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.CompareLessThan" />
        public EmitShorthand<DelegateType> Clt()
        {
            InnerEmit.CompareLessThan();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedCompareLessThan" />
        public EmitShorthand<DelegateType> Clt_Un()
        {
            InnerEmit.UnsignedCompareLessThan();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedConvertOverflow(System.Type)" />
        public EmitShorthand<DelegateType> Conv_Ovf_Un<PrimitiveType>()
        {
            Conv_Ovf_Un(typeof(PrimitiveType));

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedConvertOverflow(System.Type)" />
        public EmitShorthand<DelegateType> Conv_Ovf_Un(Type primitiveType)
        {
            InnerEmit.UnsignedConvertOverflow(primitiveType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedConvertToFloat" />
        public EmitShorthand<DelegateType> Conv_R_Un()
        {
            InnerEmit.UnsignedConvertToFloat();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Convert(System.Type)" />
        public EmitShorthand<DelegateType> Conv<PrimitiveType>()
        {
            Conv(typeof(PrimitiveType));

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Convert(System.Type)" />
        public EmitShorthand<DelegateType> Conv(Type primitiveType)
        {
            InnerEmit.Convert(primitiveType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.ConvertOverflow(System.Type)" />
        public EmitShorthand<DelegateType> Conv_Ovf<PrimitType>()
        {
            Conv_Ovf(typeof(PrimitType));

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.ConvertOverflow(System.Type)" />
        public EmitShorthand<DelegateType> Conv_Ovf(Type primitiveType)
        {
            InnerEmit.ConvertOverflow(primitiveType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.CopyBlock(System.Boolean, System.Nullable&lt;int&gt;)" />
        public EmitShorthand<DelegateType> Cpblk(bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.CopyBlock(isVolatile, unaligned);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.CopyObject(System.Type)" />
        public EmitShorthand<DelegateType> Cpobj<ValueType>()
        {
            Cpobj(typeof(ValueType));

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.CopyObject(System.Type)" />
        public EmitShorthand<DelegateType> Cpobj(Type valueType)
        {
            InnerEmit.CopyObject(valueType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Divide" />
        public EmitShorthand<DelegateType> Div()
        {
            InnerEmit.Divide();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedDivide" />
        public EmitShorthand<DelegateType> Div_Un()
        {
            InnerEmit.UnsignedDivide();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Duplicate" />
        public EmitShorthand<DelegateType> Dup()
        {
            InnerEmit.Duplicate();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.InitializeBlock(System.Boolean, System.Nullable&lt;int&gt;)" />
        public EmitShorthand<DelegateType> Initblk(bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.InitializeBlock(isVolatile, unaligned);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.InitializeObject(System.Type)" />
        public EmitShorthand<DelegateType> Initobj<ValueType>()
        {
            Initobj(typeof(ValueType));

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.InitializeObject(System.Type)" />
        public EmitShorthand<DelegateType> Initobj(Type valueType)
        {
            InnerEmit.InitializeObject(valueType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.IsInstance(System.Type)" />
        public EmitShorthand<DelegateType> Isinst<Type>()
        {
            Isinst(typeof(Type));

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.IsInstance(System.Type)" />
        public EmitShorthand<DelegateType> Isinst(Type type)
        {
            InnerEmit.IsInstance(type);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Jump(System.Reflection.MethodInfo)" />
        public EmitShorthand<DelegateType> Jmp(MethodInfo method)
        {
            InnerEmit.Jump(method);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadArgument(System.Int32)" />
        public EmitShorthand<DelegateType> Ldarg(ushort index)
        {
            InnerEmit.LoadArgument(index);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadArgumentAddress(System.Int32)" />
        public EmitShorthand<DelegateType> Ldarga(ushort index)
        {
            InnerEmit.LoadArgumentAddress(index);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadConstant(System.Boolean)" />
        public EmitShorthand<DelegateType> Ldc(bool b)
        {
            InnerEmit.LoadConstant(b);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadConstant(System.Single)" />
        public EmitShorthand<DelegateType> Ldc(float f)
        {
            InnerEmit.LoadConstant(f);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadConstant(System.Double)" />
        public EmitShorthand<DelegateType> Ldc(double d)
        {
            InnerEmit.LoadConstant(d);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadConstant(System.UInt32)" />
        public EmitShorthand<DelegateType> Ldc(uint u)
        {
            InnerEmit.LoadConstant(u);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadConstant(System.Int32)" />
        public EmitShorthand<DelegateType> Ldc(int i)
        {
            InnerEmit.LoadConstant(i);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadConstant(System.Int64)" />
        public EmitShorthand<DelegateType> Ldc(long l)
        {
            InnerEmit.LoadConstant(l);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadConstant(System.UInt64)" />
        public EmitShorthand<DelegateType> Ldc(ulong u)
        {
            InnerEmit.LoadConstant(u);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadElement``1" />
        public EmitShorthand<DelegateType> Ldelem<ElementType>()
        {
            return Ldelem(typeof(ElementType));
        }

        /// <summary cref="M:Sigil.Emit`1.LoadElement(System.Type)" />
        public EmitShorthand<DelegateType> Ldelem(Type elementType)
        {
            InnerEmit.LoadElement(elementType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadElementAddress``1" />
        public EmitShorthand<DelegateType> Ldelema<ElementType>()
        {
            return Ldelema(typeof(ElementType));
        }

        /// <summary cref="M:Sigil.Emit`1.LoadElementAddress(System.Type)" />
        public EmitShorthand<DelegateType> Ldelema(Type elementType)
        {
            InnerEmit.LoadElementAddress(elementType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadField(System.Reflection.FieldInfo, System.Boolean, System.Nullable&lt;int&gt;)" />
        public EmitShorthand<DelegateType> Ldfld(FieldInfo field, bool? isVolatile = null, int? unaligned = null)
        {
            InnerEmit.LoadField(field, isVolatile, unaligned);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadFieldAddress(System.Reflection.FieldInfo)" />
        public EmitShorthand<DelegateType> Ldflda(FieldInfo field)
        {
            InnerEmit.LoadFieldAddress(field);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadFunctionPointer(System.Reflection.MethodInfo)" />
        public EmitShorthand<DelegateType> Ldftn(MethodInfo method)
        {
            InnerEmit.LoadFunctionPointer(method);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadIndirect(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
        public EmitShorthand<DelegateType> Ldind<Type>(bool isVolatile = false, int? unaligned = null)
        {
            Ldind(typeof(Type), isVolatile, unaligned);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadIndirect(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
        public EmitShorthand<DelegateType> Ldind(Type type, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.LoadIndirect(type, isVolatile, unaligned);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadLength``1" />
        public EmitShorthand<DelegateType> Ldlen<ElementType>()
        {
            return Ldlen(typeof(ElementType));
        }

        /// <summary cref="M:Sigil.Emit`1.LoadLength(System.Type)" />
        public EmitShorthand<DelegateType> Ldlen(Type elementType)
        {
            InnerEmit.LoadLength(elementType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadLocal(Sigil.Local)" />
        public EmitShorthand<DelegateType> Ldloc(Local local)
        {
            InnerEmit.LoadLocal(local);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadLocal(System.String)" />
        public EmitShorthand<DelegateType> Ldloc(string name)
        {
            InnerEmit.LoadLocal(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadLocalAddress(Sigil.Local)" />
        public EmitShorthand<DelegateType> Ldloca(Local local)
        {
            InnerEmit.LoadLocalAddress(local);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadLocalAddress(System.String)" />
        public EmitShorthand<DelegateType> Ldloca(string name)
        {
            InnerEmit.LoadLocalAddress(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadNull" />
        public EmitShorthand<DelegateType> Ldnull()
        {
            InnerEmit.LoadNull();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadObject(System.Type, System.Boolen, System.Nullable&lt;int&gt;)" />
        public EmitShorthand<DelegateType> Ldobj<ValueType>(bool isVolatile = false, int? unaligned = null)
        {
            Ldobj(typeof(ValueType), isVolatile, unaligned);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadObject(System.Type, System.Boolen, System.Nullable&lt;int&gt;)" />
        public EmitShorthand<DelegateType> Ldobj(Type valueType, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.LoadObject(valueType, isVolatile, unaligned);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadConstant(System.String)" />
        public EmitShorthand<DelegateType> Ldstr(string str)
        {
            InnerEmit.LoadConstant(str);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadConstant(System.Reflection.FieldInfo)" />
        public EmitShorthand<DelegateType> Ldtoken(FieldInfo field)
        {
            InnerEmit.LoadConstant(field);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadConstant(System.Reflection.MethodInfo)" />
        public EmitShorthand<DelegateType> Ldtoken(MethodInfo method)
        {
            InnerEmit.LoadConstant(method);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadConstant(System.Type)" />
        public EmitShorthand<DelegateType> Ldtoken<Type>()
        {
            Ldtoken(typeof(Type));

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadConstant(System.Type)" />
        public EmitShorthand<DelegateType> Ldtoken(Type type)
        {
            InnerEmit.LoadConstant(type);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LoadVirtualFunctionPointer(System.Reflection.MethodInfo)" />
        public EmitShorthand<DelegateType> Ldvirtftn(MethodInfo method)
        {
            InnerEmit.LoadVirtualFunctionPointer(method);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Leave(Sigil.Label)" />
        public EmitShorthand<DelegateType> Leave(Label label)
        {
            InnerEmit.Leave(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Leave(System.String)" />
        public EmitShorthand<DelegateType> Leave(string name)
        {
            InnerEmit.Leave(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.LocalAllocate" />
        public EmitShorthand<DelegateType> Localloc()
        {
            InnerEmit.LocalAllocate();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Multiply" />
        public EmitShorthand<DelegateType> Mul()
        {
            InnerEmit.Multiply();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.MultiplyOverflow" />
        public EmitShorthand<DelegateType> Mul_Ovf()
        {
            InnerEmit.MultiplyOverflow();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedMultiplyOverflow" />
        public EmitShorthand<DelegateType> Mul_Ovf_Un()
        {
            InnerEmit.UnsignedMultiplyOverflow();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Negate" />
        public EmitShorthand<DelegateType> Neg()
        {
            InnerEmit.Negate();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.NewArray(System.Type)" />
        public EmitShorthand<DelegateType> Newarr<ElementType>()
        {
            Newarr(typeof(ElementType));

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.NewArray(System.Type)" />
        public EmitShorthand<DelegateType> Newarr(Type elementType)
        {
            InnerEmit.NewArray(elementType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.NewObject(System.Reflection.ConstructorInfo)" />
        public EmitShorthand<DelegateType> Newobj(ConstructorInfo constructor)
        {
            InnerEmit.NewObject(constructor);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Nop" />
        public EmitShorthand<DelegateType> Nop()
        {
            InnerEmit.Nop();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Not" />
        public EmitShorthand<DelegateType> Not()
        {
            InnerEmit.Not();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Or" />
        public EmitShorthand<DelegateType> Or()
        {
            InnerEmit.Or();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Pop" />
        public EmitShorthand<DelegateType> Pop()
        {
            InnerEmit.Pop();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Remainder" />
        public EmitShorthand<DelegateType> Rem()
        {
            InnerEmit.Remainder();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedRemainder" />
        public EmitShorthand<DelegateType> Rem_Un()
        {
            InnerEmit.UnsignedRemainder();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Return" />
        public EmitShorthand<DelegateType> Ret()
        {
            InnerEmit.Return();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.ReThrow" />
        public EmitShorthand<DelegateType> Rethrow()
        {
            InnerEmit.ReThrow();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.ShiftLeft" />
        public EmitShorthand<DelegateType> Shl()
        {
            InnerEmit.ShiftLeft();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.ShiftRight" />
        public EmitShorthand<DelegateType> Shr()
        {
            InnerEmit.ShiftRight();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedShiftRight" />
        public EmitShorthand<DelegateType> Shr_Un()
        {
            InnerEmit.UnsignedShiftRight();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.SizeOf(System.Type)" />
        public EmitShorthand<DelegateType> Sizeof<ValueType>()
        {
            return Sizeof(typeof(ValueType));
        }

        /// <summary cref="M:Sigil.Emit`1.SizeOf(System.Type)" />
        public EmitShorthand<DelegateType> Sizeof(Type valueType)
        {
            InnerEmit.SizeOf(valueType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.StoreArgument(System.Int32)" />
        public EmitShorthand<DelegateType> Starg(ushort index)
        {
            InnerEmit.StoreArgument(index);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.StoreElement``1" />
        public EmitShorthand<DelegateType> Stelem<ElementType>()
        {
            return Stelem(typeof(ElementType));
        }

        /// <summary cref="M:Sigil.Emit`1.StoreElement(System.Type)" />
        public EmitShorthand<DelegateType> Stelem(Type elementType)
        {
            InnerEmit.StoreElement(elementType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.StoreField(System.Reflection.FieldInfo, System.Boolean, System.Nullable&lt;int&gt;)" />
        public EmitShorthand<DelegateType> Stfld(FieldInfo field, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.StoreField(field, isVolatile, unaligned);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.StoreIndirect(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
        public EmitShorthand<DelegateType> Stind<Type>(bool isVolatile = false, int? unaligned = null)
        {
            return Stind(typeof(Type), isVolatile, unaligned);
        }

        /// <summary cref="M:Sigil.Emit`1.StoreIndirect(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
        public EmitShorthand<DelegateType> Stind(Type type, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.StoreIndirect(type, isVolatile, unaligned);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.StoreLocal(Sigil.Local)" />
        public EmitShorthand<DelegateType> Stloc(Local local)
        {
            InnerEmit.StoreLocal(local);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.StoreLocal(System.String)" />
        public EmitShorthand<DelegateType> Stloc(string name)
        {
            InnerEmit.StoreLocal(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.StoreObject(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
        public EmitShorthand<DelegateType> Stobj<ValueType>(bool isVolatile = false, int? unaligned = null)
        {
            return Stobj(typeof(ValueType), isVolatile, unaligned);
        }

        /// <summary cref="M:Sigil.Emit`1.StoreObject(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
        public EmitShorthand<DelegateType> Stobj(Type valueType, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.StoreObject(valueType, isVolatile, unaligned);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Subtract" />
        public EmitShorthand<DelegateType> Sub()
        {
            InnerEmit.Subtract();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.SubtractOverflow" />
        public EmitShorthand<DelegateType> Sub_Ovf()
        {
            InnerEmit.SubtractOverflow();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedSubtractOverflow" />
        public EmitShorthand<DelegateType> Sub_Ovf_Un()
        {
            InnerEmit.UnsignedSubtractOverflow();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Switch(Sigil.Label[])" />
        public EmitShorthand<DelegateType> Switch(params Label[] labels)
        {
            InnerEmit.Switch(labels);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Switch(System.String[])" />
        public EmitShorthand<DelegateType> Switch(params string[] names)
        {
            InnerEmit.Switch(names);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Throw" />
        public EmitShorthand<DelegateType> Throw()
        {
            InnerEmit.Throw();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Unbox(System.Type)" />
        public EmitShorthand<DelegateType> Unbox<ValueType>()
        {
            return Unbox(typeof(ValueType));
        }

        /// <summary cref="M:Sigil.Emit`1.Unbox(System.Type)" />
        public EmitShorthand<DelegateType> Unbox(Type valueType)
        {
            InnerEmit.Unbox(valueType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.UnboxAny(System.Type)" />
        public EmitShorthand<DelegateType> Unbox_Any<ValueType>()
        {
            return Unbox_Any(typeof(ValueType));
        }

        /// <summary cref="M:Sigil.Emit`1.UnboxAny(System.Type)" />
        public EmitShorthand<DelegateType> Unbox_Any(Type valueType)
        {
            InnerEmit.UnboxAny(valueType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.WriteLine(System.String)" />
        public EmitShorthand<DelegateType> WriteLine(string line, params Local[] locals)
        {
            InnerEmit.WriteLine(line, locals);

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.Xor" />
        public EmitShorthand<DelegateType> Xor()
        {
            InnerEmit.Xor();

            return this;
        }

        /// <summary cref="M:Sigil.Emit`1.TraceOperationResultUsage" />
        public IEnumerable<OperationResultUsage<DelegateType>> TraceOperationResultUsage()
        {
            return InnerEmit.TraceOperationResultUsage();
        }
    }
}
