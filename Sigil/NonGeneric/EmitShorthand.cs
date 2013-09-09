using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Sigil.NonGeneric
{
    /// <summary>
    /// A version of Emit with shorter named versions of it's methods.
    /// 
    /// Method names map more or less to OpCodes fields.
    /// </summary>
    public class EmitShorthand
    {
        private readonly Emit InnerEmit;

        /// <summary>
        /// Returns true if this Emit can make use of unverifiable instructions.
        /// </summary>
        public bool AllowsUnverifiableCIL { get { return InnerEmit.AllowsUnverifiableCIL; } }

        /// <summary>
        /// Returns the maxmimum number of items on the stack for the IL stream created with the current emit.
        /// 
        /// This is not the maximum that *can be placed*, but the maximum that actually are.
        /// </summary>
        public int MaxStackSize { get { return InnerEmit.MaxStackSize; } }

        /// <summary>
        /// Lookup for the locals currently in scope by name.
        /// 
        /// Locals go out of scope when released (by calling Dispose() directly, or via using) and go into scope
        /// immediately after a DeclareLocal()
        /// </summary>
        public LocalLookup Locals { get { return InnerEmit.Locals; } }

        /// <summary>
        /// Lookup for declared labels by name.
        /// </summary>
        public LabelLookup Labels { get { return InnerEmit.Labels; } }

        internal EmitShorthand(Emit inner)
        {
            InnerEmit = inner;
        }

        /// <summary>
        /// Returns the original Emit instance that AsShorthand() was called on.
        /// </summary>
        public Emit AsLonghand()
        {
            return InnerEmit;
        }

        /// <summary>
        /// Returns a string representation of the CIL opcodes written to this Emit to date.
        /// 
        /// This method is meant for debugging purposes only.
        /// </summary>
        public string Instructions()
        {
            return InnerEmit.Instructions();
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.DeclareLocal``1(System.String)" />
        public Local DeclareLocal<Type>(string name = null)
        {
            return DeclareLocal(typeof(Type), name);
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.DeclareLocal``1(System.String)" />
        public EmitShorthand DeclareLocal<Type>(out Local local, string name = null)
        {
            local = DeclareLocal<Type>(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.DeclareLocal(System.Type, System.String)" />
        public Local DeclareLocal(Type type, string name = null)
        {
            return InnerEmit.DeclareLocal(type, name);
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.DeclareLocal(System.Type, System.String)" />
        public EmitShorthand DeclareLocal(Type type, out Local local, string name = null)
        {
            local = DeclareLocal(type, name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.DefineLabel(System.String)" />
        public Label DefineLabel(string name = null)
        {
            return InnerEmit.DefineLabel(name);
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.DefineLabel(System.String)" />
        public EmitShorthand DefineLabel(out Label label, string name = null)
        {
            label = DefineLabel(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.MarkLabel(Sigil.Label, IEnumerable``1)" />
        public EmitShorthand MarkLabel(Label label)
        {
            InnerEmit.MarkLabel(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.MarkLabel(System.String, IEnumerable``1)" />
        public EmitShorthand MarkLabel(string name)
        {
            InnerEmit.MarkLabel(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BeginExceptionBlock" />
        public ExceptionBlock BeginExceptionBlock()
        {
            return InnerEmit.BeginExceptionBlock();
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BeginExceptionBlock" />
        public EmitShorthand BeginExceptionBlock(out ExceptionBlock forTry)
        {
            forTry = BeginExceptionBlock();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BeginCatchBlock``1(Sigil.ExceptionBlock)" />
        public CatchBlock BeginCatchBlock<ExceptionType>(ExceptionBlock forTry)
        {
            return BeginCatchBlock(forTry, typeof(ExceptionType));
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BeginCatchBlock``1(Sigil.ExceptionBlock)" />
        public EmitShorthand BeginCatchBlock<ExceptionType>(ExceptionBlock forTry, out CatchBlock tryCatch)
        {
            tryCatch = BeginCatchBlock<ExceptionType>(forTry);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BeginCatchBlock(System.Type, Sigil.ExceptionBlock)" />
        public CatchBlock BeginCatchBlock(ExceptionBlock forTry, Type exceptionType)
        {
            return InnerEmit.BeginCatchBlock(forTry, exceptionType);
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BeginCatchBlock(System.Type, Sigil.ExceptionBlock)" />
        public EmitShorthand BeginCatchBlock(ExceptionBlock forTry, Type exceptionType, out CatchBlock forCatch)
        {
            forCatch = BeginCatchBlock(forTry, exceptionType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.EndCatchBlock(Sigil.CatchBlock)" />
        public EmitShorthand EndCatchBlock(CatchBlock forCatch)
        {
            InnerEmit.EndCatchBlock(forCatch);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BeginFinallyBlock(Sigil.ExceptionBlock)" />
        public FinallyBlock BeginFinallyBlock(ExceptionBlock forTry)
        {
            return InnerEmit.BeginFinallyBlock(forTry);
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BeginFinallyBlock(Sigil.ExceptionBlock)" />
        public EmitShorthand BeginFinallyBlock(ExceptionBlock forTry, out FinallyBlock forFinally)
        {
            forFinally = BeginFinallyBlock(forTry);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.EndFinallyBlock(Sigil.FinallyBlock)" />
        public EmitShorthand EndFinallyBlock(FinallyBlock forFinally)
        {
            InnerEmit.EndFinallyBlock(forFinally);
            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.EndExceptionBlock(Sigil.ExceptionBlock)" />
        public EmitShorthand EndExceptionBlock(ExceptionBlock forTry)
        {
            InnerEmit.EndExceptionBlock(forTry);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CreateDelegate(System.Type, Sigil.OptimizationOptions)" />
        public object CreateDelegate(Type delegateType, OptimizationOptions optimizationOptions = OptimizationOptions.All)
        {
            return InnerEmit.CreateDelegate(delegateType, optimizationOptions);
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CreateDelegate`2" />
        public DelegateType CreateDelegate<DelegateType>(OptimizationOptions optimizationOptions = OptimizationOptions.All)
        {
            return InnerEmit.CreateDelegate<DelegateType>(optimizationOptions);
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CreateMethod" />
        public MethodBuilder CreateMethod()
        {
            return InnerEmit.CreateMethod();
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CreateConstructor" />
        public ConstructorBuilder CreateConstructor()
        {
            return InnerEmit.CreateConstructor();
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Add" />
        public EmitShorthand Add()
        {
            InnerEmit.Add();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.AddOverflow" />
        public EmitShorthand Add_Ovf()
        {
            InnerEmit.AddOverflow();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedAddOverflow" />
        public EmitShorthand Add_Ovf_Un()
        {
            InnerEmit.UnsignedAddOverflow();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.And" />
        public EmitShorthand And()
        {
            InnerEmit.And();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfEqual(Sigil.Label)" />
        public EmitShorthand Beq(Label label)
        {
            InnerEmit.BranchIfEqual(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfEqual(System.String)" />
        public EmitShorthand Beq(string name)
        {
            InnerEmit.BranchIfEqual(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfGreaterOrEqual(Sigil.Label)" />
        public EmitShorthand Bge(Label label)
        {
            InnerEmit.BranchIfGreaterOrEqual(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfGreaterOrEqual(System.String)" />
        public EmitShorthand Bge(string name)
        {
            InnerEmit.BranchIfGreaterOrEqual(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedBranchIfGreaterOrEqual(Sigil.Label)" />
        public EmitShorthand Bge_Un(Label label)
        {
            InnerEmit.UnsignedBranchIfGreaterOrEqual(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedBranchIfGreaterOrEqual(System.String)" />
        public EmitShorthand Bge_Un(string name)
        {
            InnerEmit.UnsignedBranchIfGreaterOrEqual(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfGreater(Sigil.Label)" />
        public EmitShorthand Bgt(Label label)
        {
            InnerEmit.BranchIfGreater(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfGreater(System.String)" />
        public EmitShorthand Bgt(string name)
        {
            InnerEmit.BranchIfGreater(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedBranchIfGreater(Sigil.Label)" />
        public EmitShorthand Bgt_Un(Label label)
        {
            InnerEmit.UnsignedBranchIfGreater(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedBranchIfGreater(System.String)" />
        public EmitShorthand Bgt_Un(string name)
        {
            InnerEmit.UnsignedBranchIfGreater(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfLessOrEqual(Sigil.Label)" />
        public EmitShorthand Ble(Label label)
        {
            InnerEmit.BranchIfLessOrEqual(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfLessOrEqual(System.String)" />
        public EmitShorthand Ble(string name)
        {
            InnerEmit.BranchIfLessOrEqual(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedBranchIfLessOrEqual(Sigil.Label)" />
        public EmitShorthand Ble_Un(Label label)
        {
            InnerEmit.UnsignedBranchIfLessOrEqual(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedBranchIfLessOrEqual(System.String)" />
        public EmitShorthand Ble_Un(string name)
        {
            InnerEmit.UnsignedBranchIfLessOrEqual(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfLess(Sigil.Label)" />
        public EmitShorthand Blt(Label label)
        {
            InnerEmit.BranchIfLess(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfLess(System.String)" />
        public EmitShorthand Blt(string name)
        {
            InnerEmit.BranchIfLess(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedBranchIfLess(Sigil.Label)" />
        public EmitShorthand Blt_Un(Label label)
        {
            InnerEmit.UnsignedBranchIfLess(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedBranchIfLess(System.String)" />
        public EmitShorthand Blt_Un(string name)
        {
            InnerEmit.UnsignedBranchIfLess(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedBranchIfNotEqual(Sigil.Label)" />
        public EmitShorthand Bne_Un(Label label)
        {
            InnerEmit.UnsignedBranchIfNotEqual(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedBranchIfNotEqual(System.String)" />
        public EmitShorthand Bne_Un(string name)
        {
            InnerEmit.UnsignedBranchIfNotEqual(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Box``1()" />
        public EmitShorthand Box<ValueType>()
        {
            Box(typeof(ValueType));

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Box(System.Type)" />
        public EmitShorthand Box(Type valueType)
        {
            InnerEmit.Box(valueType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Branch(Sigil.Label)" />
        public EmitShorthand Br(Label label)
        {
            InnerEmit.Branch(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Branch(System.String)" />
        public EmitShorthand Br(string name)
        {
            InnerEmit.Branch(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Break" />
        public EmitShorthand Break()
        {
            InnerEmit.Break();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfFalse(Sigil.Label)" />
        public EmitShorthand Brfalse(Label label)
        {
            InnerEmit.BranchIfFalse(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfFalse(System.String)" />
        public EmitShorthand Brfalse(string name)
        {
            InnerEmit.BranchIfFalse(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfTrue(Sigil.Label)" />
        public EmitShorthand Brtrue(Label label)
        {
            InnerEmit.BranchIfTrue(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfTrue(System.String)" />
        public EmitShorthand Brtrue(string name)
        {
            InnerEmit.BranchIfTrue(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Call(System.Reflection.MethodInfo)" />
        public EmitShorthand Call(MethodInfo method)
        {
            InnerEmit.Call(method);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CallIndirect(System.Reflection.CallingConventions,System.Type,System.Type[])" />
        public EmitShorthand Calli(CallingConventions callingConvention, Type returnType, params Type[] parameterTypes)
        {
            InnerEmit.CallIndirect(callingConvention, returnType, parameterTypes);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CallVirtual(System.Reflection.MethodInfo, System.Type)" />
        public EmitShorthand Callvirt(MethodInfo method, Type constrained = null)
        {
            InnerEmit.CallVirtual(method, constrained);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CastClass``1" />
        public EmitShorthand Castclass<ReferenceType>()
        {
            Castclass(typeof(ReferenceType));

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CastClass(System.Type)" />
        public EmitShorthand Castclass(Type referenceType)
        {
            InnerEmit.CastClass(referenceType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CompareEqual" />
        public EmitShorthand Ceq()
        {
            InnerEmit.CompareEqual();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CompareGreaterThan" />
        public EmitShorthand Cgt()
        {
            InnerEmit.CompareGreaterThan();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedCompareGreaterThan" />
        public EmitShorthand Cgt_Un()
        {
            InnerEmit.UnsignedCompareGreaterThan();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CheckFinite" />
        public EmitShorthand Ckfinite()
        {
            InnerEmit.CheckFinite();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CompareLessThan" />
        public EmitShorthand Clt()
        {
            InnerEmit.CompareLessThan();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedCompareLessThan" />
        public EmitShorthand Clt_Un()
        {
            InnerEmit.UnsignedCompareLessThan();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedConvertOverflow(System.Type)" />
        public EmitShorthand Conv_Ovf_Un<PrimitiveType>()
        {
            Conv_Ovf_Un(typeof(PrimitiveType));

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedConvertOverflow(System.Type)" />
        public EmitShorthand Conv_Ovf_Un(Type primitiveType)
        {
            InnerEmit.UnsignedConvertOverflow(primitiveType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedConvertToFloat" />
        public EmitShorthand Conv_R_Un()
        {
            InnerEmit.UnsignedConvertToFloat();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Convert(System.Type)" />
        public EmitShorthand Conv<PrimitiveType>()
        {
            Conv(typeof(PrimitiveType));

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Convert(System.Type)" />
        public EmitShorthand Conv(Type primitiveType)
        {
            InnerEmit.Convert(primitiveType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.ConvertOverflow(System.Type)" />
        public EmitShorthand Conv_Ovf<PrimitType>()
        {
            Conv_Ovf(typeof(PrimitType));

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.ConvertOverflow(System.Type)" />
        public EmitShorthand Conv_Ovf(Type primitiveType)
        {
            InnerEmit.ConvertOverflow(primitiveType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CopyBlock(System.Boolean, System.Nullable&lt;int&gt;)" />
        public EmitShorthand Cpblk(bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.CopyBlock(isVolatile, unaligned);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CopyObject(System.Type)" />
        public EmitShorthand Cpobj<ValueType>()
        {
            Cpobj(typeof(ValueType));

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CopyObject(System.Type)" />
        public EmitShorthand Cpobj(Type valueType)
        {
            InnerEmit.CopyObject(valueType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Divide" />
        public EmitShorthand Div()
        {
            InnerEmit.Divide();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedDivide" />
        public EmitShorthand Div_Un()
        {
            InnerEmit.UnsignedDivide();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Duplicate" />
        public EmitShorthand Dup()
        {
            InnerEmit.Duplicate();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.InitializeBlock(System.Boolean, System.Nullable&lt;int&gt;)" />
        public EmitShorthand Initblk(bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.InitializeBlock(isVolatile, unaligned);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.InitializeObject(System.Type)" />
        public EmitShorthand Initobj<ValueType>()
        {
            Initobj(typeof(ValueType));

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.InitializeObject(System.Type)" />
        public EmitShorthand Initobj(Type valueType)
        {
            InnerEmit.InitializeObject(valueType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.IsInstance(System.Type)" />
        public EmitShorthand Isinst<Type>()
        {
            Isinst(typeof(Type));

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.IsInstance(System.Type)" />
        public EmitShorthand Isinst(Type type)
        {
            InnerEmit.IsInstance(type);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Jump(System.Reflection.MethodInfo)" />
        public EmitShorthand Jmp(MethodInfo method)
        {
            InnerEmit.Jump(method);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadArgument(System.Int32)" />
        public EmitShorthand Ldarg(ushort index)
        {
            InnerEmit.LoadArgument(index);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadArgumentAddress(System.Int32)" />
        public EmitShorthand Ldarga(ushort index)
        {
            InnerEmit.LoadArgumentAddress(index);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadConstant(System.Boolean)" />
        public EmitShorthand Ldc(bool b)
        {
            InnerEmit.LoadConstant(b);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadConstant(System.Single)" />
        public EmitShorthand Ldc(float f)
        {
            InnerEmit.LoadConstant(f);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadConstant(System.Double)" />
        public EmitShorthand Ldc(double d)
        {
            InnerEmit.LoadConstant(d);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadConstant(System.UInt32)" />
        public EmitShorthand Ldc(uint u)
        {
            InnerEmit.LoadConstant(u);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadConstant(System.Int32)" />
        public EmitShorthand Ldc(int i)
        {
            InnerEmit.LoadConstant(i);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadConstant(System.Int64)" />
        public EmitShorthand Ldc(long l)
        {
            InnerEmit.LoadConstant(l);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadConstant(System.UInt64)" />
        public EmitShorthand Ldc(ulong u)
        {
            InnerEmit.LoadConstant(u);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadElement``1" />
        public EmitShorthand Ldelem<ElementType>()
        {
            return Ldelem(typeof(ElementType));
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadElement(System.Type)" />
        public EmitShorthand Ldelem(Type elementType)
        {
            InnerEmit.LoadElement(elementType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadElementAddress``1" />
        public EmitShorthand Ldelema<ElementType>()
        {
            return Ldelema(typeof(ElementType));
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadElementAddress(System.Type)" />
        public EmitShorthand Ldelema(Type elementType)
        {
            InnerEmit.LoadElementAddress(elementType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadField(System.Reflection.FieldInfo, System.Boolean, System.Nullable&lt;int&gt;)" />
        public EmitShorthand Ldfld(FieldInfo field, bool? isVolatile = null, int? unaligned = null)
        {
            InnerEmit.LoadField(field, isVolatile, unaligned);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadFieldAddress(System.Reflection.FieldInfo)" />
        public EmitShorthand Ldflda(FieldInfo field)
        {
            InnerEmit.LoadFieldAddress(field);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadFunctionPointer(System.Reflection.MethodInfo)" />
        public EmitShorthand Ldftn(MethodInfo method)
        {
            InnerEmit.LoadFunctionPointer(method);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadIndirect(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
        public EmitShorthand Ldind<Type>(bool isVolatile = false, int? unaligned = null)
        {
            Ldind(typeof(Type), isVolatile, unaligned);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadIndirect(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
        public EmitShorthand Ldind(Type type, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.LoadIndirect(type, isVolatile, unaligned);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadLength``1" />
        public EmitShorthand Ldlen<ElementType>()
        {
            return Ldlen(typeof(ElementType));
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadLength(System.Type)" />
        public EmitShorthand Ldlen(Type elementType)
        {
            InnerEmit.LoadLength(elementType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadLocal(Sigil.Local)" />
        public EmitShorthand Ldloc(Local local)
        {
            InnerEmit.LoadLocal(local);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadLocal(System.String)" />
        public EmitShorthand Ldloc(string name)
        {
            InnerEmit.LoadLocal(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadLocalAddress(Sigil.Local)" />
        public EmitShorthand Ldloca(Local local)
        {
            InnerEmit.LoadLocalAddress(local);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadLocalAddress(System.String)" />
        public EmitShorthand Ldloca(string name)
        {
            InnerEmit.LoadLocalAddress(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadNull" />
        public EmitShorthand Ldnull()
        {
            InnerEmit.LoadNull();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadObject(System.Type, System.Boolen, System.Nullable&lt;int&gt;)" />
        public EmitShorthand Ldobj<ValueType>(bool isVolatile = false, int? unaligned = null)
        {
            Ldobj(typeof(ValueType), isVolatile, unaligned);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadObject(System.Type, System.Boolen, System.Nullable&lt;int&gt;)" />
        public EmitShorthand Ldobj(Type valueType, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.LoadObject(valueType, isVolatile, unaligned);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadConstant(System.String)" />
        public EmitShorthand Ldstr(string str)
        {
            InnerEmit.LoadConstant(str);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadConstant(System.Reflection.FieldInfo)" />
        public EmitShorthand Ldtoken(FieldInfo field)
        {
            InnerEmit.LoadConstant(field);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadConstant(System.Reflection.MethodInfo)" />
        public EmitShorthand Ldtoken(MethodInfo method)
        {
            InnerEmit.LoadConstant(method);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadConstant(System.Type)" />
        public EmitShorthand Ldtoken<Type>()
        {
            Ldtoken(typeof(Type));

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadConstant(System.Type)" />
        public EmitShorthand Ldtoken(Type type)
        {
            InnerEmit.LoadConstant(type);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadVirtualFunctionPointer(System.Reflection.MethodInfo)" />
        public EmitShorthand Ldvirtftn(MethodInfo method)
        {
            InnerEmit.LoadVirtualFunctionPointer(method);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Leave(Sigil.Label)" />
        public EmitShorthand Leave(Label label)
        {
            InnerEmit.Leave(label);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Leave(System.String)" />
        public EmitShorthand Leave(string name)
        {
            InnerEmit.Leave(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LocalAllocate" />
        public EmitShorthand Localloc()
        {
            InnerEmit.LocalAllocate();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Multiply" />
        public EmitShorthand Mul()
        {
            InnerEmit.Multiply();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.MultiplyOverflow" />
        public EmitShorthand Mul_Ovf()
        {
            InnerEmit.MultiplyOverflow();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedMultiplyOverflow" />
        public EmitShorthand Mul_Ovf_Un()
        {
            InnerEmit.UnsignedMultiplyOverflow();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Negate" />
        public EmitShorthand Neg()
        {
            InnerEmit.Negate();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.NewArray(System.Type)" />
        public EmitShorthand Newarr<ElementType>()
        {
            Newarr(typeof(ElementType));

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.NewArray(System.Type)" />
        public EmitShorthand Newarr(Type elementType)
        {
            InnerEmit.NewArray(elementType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.NewObject(System.Reflection.ConstructorInfo)" />
        public EmitShorthand Newobj(ConstructorInfo constructor)
        {
            InnerEmit.NewObject(constructor);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Nop" />
        public EmitShorthand Nop()
        {
            InnerEmit.Nop();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Not" />
        public EmitShorthand Not()
        {
            InnerEmit.Not();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Or" />
        public EmitShorthand Or()
        {
            InnerEmit.Or();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Pop" />
        public EmitShorthand Pop()
        {
            InnerEmit.Pop();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Remainder" />
        public EmitShorthand Rem()
        {
            InnerEmit.Remainder();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedRemainder" />
        public EmitShorthand Rem_Un()
        {
            InnerEmit.UnsignedRemainder();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Return" />
        public EmitShorthand Ret()
        {
            InnerEmit.Return();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.ReThrow" />
        public EmitShorthand Rethrow()
        {
            InnerEmit.ReThrow();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.ShiftLeft" />
        public EmitShorthand Shl()
        {
            InnerEmit.ShiftLeft();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.ShiftRight" />
        public EmitShorthand Shr()
        {
            InnerEmit.ShiftRight();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedShiftRight" />
        public EmitShorthand Shr_Un()
        {
            InnerEmit.UnsignedShiftRight();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.SizeOf(System.Type)" />
        public EmitShorthand Sizeof<ValueType>()
        {
            return Sizeof(typeof(ValueType));
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.SizeOf(System.Type)" />
        public EmitShorthand Sizeof(Type valueType)
        {
            InnerEmit.SizeOf(valueType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.StoreArgument(System.Int32)" />
        public EmitShorthand Starg(ushort index)
        {
            InnerEmit.StoreArgument(index);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.StoreElement``1" />
        public EmitShorthand Stelem<ElementType>()
        {
            return Stelem(typeof(ElementType));
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.StoreElement(System.Type)" />
        public EmitShorthand Stelem(Type elementType)
        {
            InnerEmit.StoreElement(elementType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.StoreField(System.Reflection.FieldInfo, System.Boolean, System.Nullable&lt;int&gt;)" />
        public EmitShorthand Stfld(FieldInfo field, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.StoreField(field, isVolatile, unaligned);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.StoreIndirect(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
        public EmitShorthand Stind<Type>(bool isVolatile = false, int? unaligned = null)
        {
            return Stind(typeof(Type), isVolatile, unaligned);
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.StoreIndirect(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
        public EmitShorthand Stind(Type type, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.StoreIndirect(type, isVolatile, unaligned);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.StoreLocal(Sigil.Local)" />
        public EmitShorthand Stloc(Local local)
        {
            InnerEmit.StoreLocal(local);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.StoreLocal(System.String)" />
        public EmitShorthand Stloc(string name)
        {
            InnerEmit.StoreLocal(name);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.StoreObject(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
        public EmitShorthand Stobj<ValueType>(bool isVolatile = false, int? unaligned = null)
        {
            return Stobj(typeof(ValueType), isVolatile, unaligned);
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.StoreObject(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
        public EmitShorthand Stobj(Type valueType, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.StoreObject(valueType, isVolatile, unaligned);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Subtract" />
        public EmitShorthand Sub()
        {
            InnerEmit.Subtract();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.SubtractOverflow" />
        public EmitShorthand Sub_Ovf()
        {
            InnerEmit.SubtractOverflow();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedSubtractOverflow" />
        public EmitShorthand Sub_Ovf_Un()
        {
            InnerEmit.UnsignedSubtractOverflow();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Switch(Sigil.Label[])" />
        public EmitShorthand Switch(params Label[] labels)
        {
            InnerEmit.Switch(labels);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Switch(System.String[])" />
        public EmitShorthand Switch(params string[] names)
        {
            InnerEmit.Switch(names);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Throw" />
        public EmitShorthand Throw()
        {
            InnerEmit.Throw();

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Unbox(System.Type)" />
        public EmitShorthand Unbox<ValueType>()
        {
            return Unbox(typeof(ValueType));
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Unbox(System.Type)" />
        public EmitShorthand Unbox(Type valueType)
        {
            InnerEmit.Unbox(valueType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnboxAny(System.Type)" />
        public EmitShorthand Unbox_Any<ValueType>()
        {
            return Unbox_Any(typeof(ValueType));
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnboxAny(System.Type)" />
        public EmitShorthand Unbox_Any(Type valueType)
        {
            InnerEmit.UnboxAny(valueType);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.WriteLine(System.String)" />
        public EmitShorthand WriteLine(string line, params Local[] locals)
        {
            InnerEmit.WriteLine(line, locals);

            return this;
        }

        /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Xor" />
        public EmitShorthand Xor()
        {
            InnerEmit.Xor();

            return this;
        }
    }
}