using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    /// <summary>
    /// A version of Emit with shorter named versions of it's methods.
    /// 
    /// Method names map more or less to OpCodes fields.
    /// </summary>
    public class EmitShorthand<DelegateType>
    {
        private readonly Emit<DelegateType> InnerEmit;

        internal EmitShorthand(Emit<DelegateType> inner)
        {
            InnerEmit = inner;
        }

        /// <summary cref="M:Sigil.Emit`1.DeclareLocal``1(System.String)" />
        public Local DeclareLocal<Type>(string name = null)
        {
            return DeclareLocal(typeof(Type), name);
        }

        /// <summary cref="M:Sigil.Emit`1.DeclareLocal(System.Type, System.String)" />
        public Local DeclareLocal(Type type, string name = null)
        {
            return InnerEmit.DeclareLocal(type, name);
        }

        /// <summary cref="M:Sigil.Emit`1.DefineLabel(System.String)" />
        public Label DefineLabel(string name = null)
        {
            return InnerEmit.DefineLabel(name);
        }

        /// <summary cref="M:Sigil.Emit`1.MarkLabel(Sigil.Label)" />
        public void MarkLabel(Label label)
        {
            InnerEmit.MarkLabel(label);
        }

        /// <summary cref="M:Sigil.Emit`1.BeginExceptionBlock" />
        public ExceptionBlock BeginExceptionBlock()
        {
            return InnerEmit.BeginExceptionBlock();
        }

        /// <summary cref="M:Sigil.Emit`1.BeginCatchBlock``1(Sigil.ExceptionBlock)" />
        public CatchBlock BeginCatchBlock<ExceptionType>(ExceptionBlock forTry)
        {
            return BeginCatchBlock(forTry, typeof(ExceptionType));
        }

        /// <summary cref="M:Sigil.Emit`1.BeginCatchBlock(System.Type, Sigil.ExceptionBlock)" />
        public CatchBlock BeginCatchBlock(ExceptionBlock forTry, Type exceptionType)
        {
            return InnerEmit.BeginCatchBlock(forTry, exceptionType);
        }

        /// <summary cref="M:Sigil.Emit`1.EndCatchBlock(Sigil.CatchBlock)" />
        public void EndCatchBlock(CatchBlock forCatch)
        {
            InnerEmit.EndCatchBlock(forCatch);
        }

        /// <summary cref="M:Sigil.Emit`1.BeginFinallyBlock(Sigil.ExceptionBlock)" />
        public FinallyBlock BeginFinallyBlock(ExceptionBlock forTry)
        {
            return InnerEmit.BeginFinallyBlock(forTry);
        }

        /// <summary cref="M:Sigil.Emit`1.EndFinallyBlock(Sigil.FinallyBlock)" />
        public void EndFinallyBlock(FinallyBlock forFinally)
        {
            InnerEmit.EndFinallyBlock(forFinally);
        }

        /// <summary cref="M:Sigil.Emit`1.EndExceptionBlock(Sigil.ExceptionBlock)" />
        public void EndExceptionBlock(ExceptionBlock forTry)
        {
            InnerEmit.EndExceptionBlock(forTry);
        }

        /// <summary cref="M:Sigil.Emit`1.CreateDelegate" />
        public DelegateType CreateDelegate()
        {
            return InnerEmit.CreateDelegate();
        }

        /// <summary cref="M:Sigil.Emit`1.Add" />
        public void Add()
        {
            InnerEmit.Add();
        }

        /// <summary cref="M:Sigil.Emit`1.AddOverflow" />
        public void Add_Ovf()
        {
            InnerEmit.AddOverflow();
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedAddOverflow" />
        public void Add_Ovf_Un()
        {
            InnerEmit.UnsignedAddOverflow();
        }

        /// <summary cref="M:Sigil.Emit`1.And" />
        public void And()
        {
            InnerEmit.And();
        }

        /// <summary cref="M:Sigil.Emit`1.BranchIfEqual(Sigil.Label)" />
        public void Beq(Label label)
        {
            InnerEmit.BranchIfEqual(label);
        }

        /// <summary cref="M:Sigil.Emit`1.BranchIfGreaterOrEqual(Sigil.Label)" />
        public void Bge(Label label)
        {
            InnerEmit.BranchIfGreaterOrEqual(label);
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedBranchIfGreaterOrEqual(Sigil.Label)" />
        public void Bge_Un(Label label)
        {
            InnerEmit.UnsignedBranchIfGreaterOrEqual(label);
        }

        /// <summary cref="M:Sigil.Emit`1.BranchIfGreater(Sigil.Label)" />
        public void Bgt(Label label)
        {
            InnerEmit.BranchIfGreater(label);
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedBranchIfGreater(Sigil.Label)" />
        public void Bgt_Un(Label label)
        {
            InnerEmit.UnsignedBranchIfGreater(label);
        }

        /// <summary cref="M:Sigil.Emit`1.BranchIfLessOrEqual(Sigil.Label)" />
        public void Ble(Label label)
        {
            InnerEmit.BranchIfLessOrEqual(label);
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedBranchIfLessOrEqual(Sigil.Label)" />
        public void Ble_Un(Label label)
        {
            InnerEmit.UnsignedBranchIfLessOrEqual(label);
        }

        /// <summary cref="M:Sigil.Emit`1.BranchIfLess(Sigil.Label)" />
        public void Blt(Label label)
        {
            InnerEmit.BranchIfLess(label);
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedBranchIfLess(Sigil.Label)" />
        public void Blt_Un(Label label)
        {
            InnerEmit.UnsignedBranchIfLess(label);
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedBranchIfNotEqual(Sigil.Label)" />
        public void Bne_Un(Label label)
        {
            InnerEmit.UnsignedBranchIfNotEqual(label);
        }

        /// <summary cref="M:Sigil.Emit`1.Box``1()" />
        public void Box<ValueType>()
        {
            Box(typeof(ValueType));
        }

        /// <summary cref="M:Sigil.Emit`1.Box(System.Type)" />
        public void Box(Type valueType)
        {
            InnerEmit.Box(valueType);
        }

        /// <summary cref="M:Sigil.Emit`1.Branch(Sigil.Label)" />
        public void Br(Label label)
        {
            InnerEmit.Branch(label);
        }

        /// <summary cref="M:Sigil.Emit`1.Break" />
        public void Break()
        {
            InnerEmit.Break();
        }

        /// <summary cref="M:Sigil.Emit`1.BranchIfFalse(Sigil.Label)" />
        public void Brfalse(Label label)
        {
            InnerEmit.BranchIfFalse(label);
        }

        /// <summary cref="M:Sigil.Emit`1.BranchIfTrue(Sigil.Label)" />
        public void Brtrue(Label label)
        {
            InnerEmit.BranchIfTrue(label);
        }

        /// <summary cref="M:Sigil.Emit`1.Call(System.Reflection.MethodInfo)" />
        public void Call(MethodInfo method)
        {
            InnerEmit.Call(method);
        }

        /// <summary cref="M:Sigil.Emit`1.CallIndirect(System.Reflection.CallingConventions,System.Type,System.Type[])" />
        public void Calli(CallingConventions callingConvention, Type returnType, params Type[] parameterTypes)
        {
            InnerEmit.CallIndirect(callingConvention, returnType, parameterTypes);
        }

        /// <summary cref="M:Sigil.Emit`1.CallVirtual(System.Reflection.MethodInfo, System.Type)" />
        public void Callvirt(MethodInfo method, Type constrained = null)
        {
            InnerEmit.CallVirtual(method, constrained);
        }

        /// <summary cref="M:Sigil.Emit`1.CastClass``1" />
        public void Castclass<ReferenceType>()
        {
            Castclass(typeof(ReferenceType));
        }

        /// <summary cref="M:Sigil.Emit`1.CastClass(System.Type)" />
        public void Castclass(Type referenceType)
        {
            InnerEmit.CastClass(referenceType);
        }

        /// <summary cref="M:Sigil.Emit`1.CompareEqual" />
        public void Ceq()
        {
            InnerEmit.CompareEqual();
        }

        /// <summary cref="M:Sigil.Emit`1.CompareGreaterThan" />
        public void Cgt()
        {
            InnerEmit.CompareGreaterThan();
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedCompareGreaterThan" />
        public void Cgt_Un()
        {
            InnerEmit.UnsignedCompareGreaterThan();
        }

        /// <summary cref="M:Sigil.Emit`1.CheckFinite" />
        public void Ckfinite()
        {
            InnerEmit.CheckFinite();
        }

        /// <summary cref="M:Sigil.Emit`1.CompareLessThan" />
        public void Clt()
        {
            InnerEmit.CompareLessThan();
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedCompareLessThan" />
        public void Clt_Un()
        {
            InnerEmit.UnsignedCompareLessThan();
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedConvertOverflow(System.Type)" />
        public void Conv_Ovf_Un<PrimitiveType>()
        {
            Conv_Ovf_Un(typeof(PrimitiveType));
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedConvertOverflow(System.Type)" />
        public void Conv_Ovf_Un(Type primitiveType)
        {
            InnerEmit.UnsignedConvertOverflow(primitiveType);
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedConvertToFloat" />
        public void Conv_R_Un()
        {
            InnerEmit.UnsignedConvertToFloat();
        }

        /// <summary cref="M:Sigil.Emit`1.Convert(System.Type)" />
        public void Conv<PrimitiveType>()
        {
            Conv(typeof(PrimitiveType));
        }

        /// <summary cref="M:Sigil.Emit`1.Convert(System.Type)" />
        public void Conv(Type primitiveType)
        {
            InnerEmit.Convert(primitiveType);
        }

        /// <summary cref="M:Sigil.Emit`1.ConvertOverflow(System.Type)" />
        public void Conv_Ovf<PrimitType>()
        {
            Conv_Ovf(typeof(PrimitType));
        }

        /// <summary cref="M:Sigil.Emit`1.ConvertOverflow(System.Type)" />
        public void Conv_Ovf(Type primitiveType)
        {
            InnerEmit.ConvertOverflow(primitiveType);
        }

        /// <summary cref="M:Sigil.Emit`1.CopyBlock(System.Boolean, System.Nullable&lt;int&gt;)" />
        public void Cpblk(bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.CopyBlock(isVolatile, unaligned);
        }

        /// <summary cref="M:Sigil.Emit`1.CopyObject(System.Type)" />
        public void Cpobj<ValueType>()
        {
            Cpobj(typeof(ValueType));
        }

        /// <summary cref="M:Sigil.Emit`1.CopyObject(System.Type)" />
        public void Cpobj(Type valueType)
        {
            InnerEmit.CopyObject(valueType);
        }

        /// <summary cref="M:Sigil.Emit`1.Divide" />
        public void Div()
        {
            InnerEmit.Divide();
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedDivide" />
        public void Div_Un()
        {
            InnerEmit.UnsignedDivide();
        }

        /// <summary cref="M:Sigil.Emit`1.Duplicate" />
        public void Dup()
        {
            InnerEmit.Duplicate();
        }

        /// <summary cref="M:Sigil.Emit`1.InitializeBlock(System.Boolean, System.Nullable&lt;int&gt;)" />
        public void Initblk(bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.InitializeBlock(isVolatile, unaligned);
        }

        /// <summary cref="M:Sigil.Emit`1.InitializeObject(System.Type)" />
        public void Initobj<ValueType>()
        {
            Initobj(typeof(ValueType));
        }

        /// <summary cref="M:Sigil.Emit`1.InitializeObject(System.Type)" />
        public void Initobj(Type valueType)
        {
            InnerEmit.InitializeObject(valueType);
        }

        /// <summary cref="M:Sigil.Emit`1.IsInstance(System.Type)" />
        public void Isinst<Type>()
        {
            Isinst(typeof(Type));
        }

        /// <summary cref="M:Sigil.Emit`1.IsInstance(System.Type)" />
        public void Isinst(Type type)
        {
            InnerEmit.IsInstance(type);
        }

        /// <summary cref="M:Sigil.Emit`1.Jump(System.Reflection.MethodInfo)" />
        public void Jmp(MethodInfo method)
        {
            InnerEmit.Jump(method);
        }

        /// <summary cref="M:Sigil.Emit`1.LoadArgument(System.Int32)" />
        public void Ldarg(int index)
        {
            InnerEmit.LoadArgument(index);
        }

        /// <summary cref="M:Sigil.Emit`1.LoadArgumentAddress(System.Int32)" />
        public void Ldarga(int index)
        {
            InnerEmit.LoadArgumentAddress(index);
        }

        /// <summary cref="M:Sigil.Emit`1.LoadConstant(System.Boolean)" />
        public void Ldc(bool b)
        {
            InnerEmit.LoadConstant(b);
        }

        /// <summary cref="M:Sigil.Emit`1.LoadConstant(System.Single)" />
        public void Ldc(float f)
        {
            InnerEmit.LoadConstant(f);
        }

        /// <summary cref="M:Sigil.Emit`1.LoadConstant(System.Double)" />
        public void Ldc(double d)
        {
            InnerEmit.LoadConstant(d);
        }

        /// <summary cref="M:Sigil.Emit`1.LoadConstant(System.UInt32)" />
        public void Ldc(uint u)
        {
            InnerEmit.LoadConstant(u);
        }

        /// <summary cref="M:Sigil.Emit`1.LoadConstant(System.Int32)" />
        public void Ldc(int i)
        {
            InnerEmit.LoadConstant(i);
        }

        /// <summary cref="M:Sigil.Emit`1.LoadConstant(System.Int64)" />
        public void Ldc(long l)
        {
            InnerEmit.LoadConstant(l);
        }

        /// <summary cref="M:Sigil.Emit`1.LoadConstant(System.UInt64)" />
        public void Ldc(ulong u)
        {
            InnerEmit.LoadConstant(u);
        }

        /// <summary cref="M:Sigil.Emit`1.LoadElement" />
        public void Ldelem()
        {
            InnerEmit.LoadElement();
        }

        /// <summary cref="M:Sigil.Emit`1.LoadElementAddress(System.Type)" />
        public void Ldelema<ElementType>()
        {
            Ldelema(typeof(ElementType));
        }

        /// <summary cref="M:Sigil.Emit`1.LoadElementAddress(System.Type)" />
        public void Ldelema(Type elementType)
        {
            InnerEmit.LoadElementAddress(elementType);
        }

        /// <summary cref="M:Sigil.Emit`1.LoadField(System.Reflection.FieldInfo, System.Boolean, System.Nullable&lt;int&gt;)" />
        public void Ldfld(FieldInfo field, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.LoadField(field, isVolatile, unaligned);
        }

        /// <summary cref="M:Sigil.Emit`1.LoadFieldAddress(System.Reflection.FieldInfo)" />
        public void Ldflda(FieldInfo field)
        {
            InnerEmit.LoadFieldAddress(field);
        }

        /// <summary cref="M:Sigil.Emit`1.LoadFunctionPointer(System.Reflection.MethodInfo)" />
        public void Ldftn(MethodInfo method)
        {
            InnerEmit.LoadFunctionPointer(method);
        }

        /// <summary cref="M:Sigil.Emit`1.LoadIndirect(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
        public void Ldind<Type>(bool isVolatile = false, int? unaligned = null)
        {
            Ldind(typeof(Type), isVolatile, unaligned);
        }

        /// <summary cref="M:Sigil.Emit`1.LoadIndirect(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
        public void Ldind(Type type, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.LoadIndirect(type, isVolatile, unaligned);
        }

        /// <summary cref="M:Sigil.Emit`1.LoadLength" />
        public void Ldlen()
        {
            InnerEmit.LoadLength();
        }

        /// <summary cref="M:Sigil.Emit`1.LoadLocal(Sigil.Local)" />
        public void Ldloc(Local local)
        {
            InnerEmit.LoadLocal(local);
        }

        /// <summary cref="M:Sigil.Emit`1.LoadLocalAddress(Sigil.Local)" />
        public void Ldloca(Local local)
        {
            InnerEmit.LoadLocalAddress(local);
        }

        /// <summary cref="M:Sigil.Emit`1.LoadNull" />
        public void Ldnull()
        {
            InnerEmit.LoadNull();
        }

        /// <summary cref="M:Sigil.Emit`1.LoadObject(System.Type, System.Boolen, System.Nullable&lt;int&gt;)" />
        public void Ldobj<ValueType>(bool isVolatile = false, int? unaligned = null)
        {
            Ldobj(typeof(ValueType), isVolatile, unaligned);
        }

        /// <summary cref="M:Sigil.Emit`1.LoadObject(System.Type, System.Boolen, System.Nullable&lt;int&gt;)" />
        public void Ldobj(Type valueType, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.LoadObject(valueType, isVolatile, unaligned);
        }

        /// <summary cref="M:Sigil.Emit`1.LoadConstant(System.String)" />
        public void Ldstr(string str)
        {
            InnerEmit.LoadConstant(str);
        }

        /// <summary cref="M:Sigil.Emit`1.LoadConstant(System.Reflection.FieldInfo)" />
        public void Ldtoken(FieldInfo field)
        {
            InnerEmit.LoadConstant(field);
        }

        /// <summary cref="M:Sigil.Emit`1.LoadConstant(System.Reflection.MethodInfo)" />
        public void Ldtoken(MethodInfo method)
        {
            InnerEmit.LoadConstant(method);
        }

        /// <summary cref="M:Sigil.Emit`1.LoadConstant(System.Type)" />
        public void Ldtoken<Type>()
        {
            Ldtoken(typeof(Type));
        }

        /// <summary cref="M:Sigil.Emit`1.LoadConstant(System.Type)" />
        public void Ldtoken(Type type)
        {
            InnerEmit.LoadConstant(type);
        }

        /// <summary cref="M:Sigil.Emit`1.LoadVirtualFunctionPointer(System.Reflection.MethodInfo)" />
        public void Ldvirtftn(MethodInfo method)
        {
            InnerEmit.LoadVirtualFunctionPointer(method);
        }

        /// <summary cref="M:Sigil.Emit`1.Leave(Sigil.Label)" />
        public void Leave(Label label)
        {
            InnerEmit.Leave(label);
        }

        /// <summary cref="M:Sigil.Emit`1.LocalAllocate" />
        public void Localloc()
        {
            InnerEmit.LocalAllocate();
        }

        /// <summary cref="M:Sigil.Emit`1.Multiply" />
        public void Mul()
        {
            InnerEmit.Multiply();
        }

        /// <summary cref="M:Sigil.Emit`1.MultiplyOverflow" />
        public void Mul_Ovf()
        {
            InnerEmit.MultiplyOverflow();
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedMultiplyOverflow" />
        public void Mul_Ovf_Un()
        {
            InnerEmit.UnsignedMultiplyOverflow();
        }

        /// <summary cref="M:Sigil.Emit`1.Negate" />
        public void Neg()
        {
            InnerEmit.Negate();
        }

        /// <summary cref="M:Sigil.Emit`1.NewArray(System.Type)" />
        public void Newarr<ElementType>()
        {
            Newarr(typeof(ElementType));
        }

        /// <summary cref="M:Sigil.Emit`1.NewArray(System.Type)" />
        public void Newarr(Type elementType)
        {
            InnerEmit.NewArray(elementType);
        }

        /// <summary cref="M:Sigil.Emit`1.NewObject(System.Reflection.ConstructorInfo)" />
        public void Newobj(ConstructorInfo constructor)
        {
            InnerEmit.NewObject(constructor);
        }

        /// <summary cref="M:Sigil.Emit`1.Nop" />
        public void Nop()
        {
            InnerEmit.Nop();
        }

        /// <summary cref="M:Sigil.Emit`1.Not" />
        public void Not()
        {
            InnerEmit.Not();
        }

        /// <summary cref="M:Sigil.Emit`1.Or" />
        public void Or()
        {
            InnerEmit.Or();
        }

        /// <summary cref="M:Sigil.Emit`1.Pop" />
        public void Pop()
        {
            InnerEmit.Pop();
        }

        /// <summary cref="M:Sigil.Emit`1.Remainder" />
        public void Rem()
        {
            InnerEmit.Remainder();
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedRemainder" />
        public void Rem_Un()
        {
            InnerEmit.UnsignedRemainder();
        }

        /// <summary cref="M:Sigil.Emit`1.Return" />
        public void Ret()
        {
            InnerEmit.Return();
        }

        /// <summary cref="M:Sigil.Emit`1.ReThrow" />
        public void Rethrow()
        {
            InnerEmit.ReThrow();
        }

        /// <summary cref="M:Sigil.Emit`1.ShiftLeft" />
        public void Shl()
        {
            InnerEmit.ShiftLeft();
        }

        /// <summary cref="M:Sigil.Emit`1.ShiftRight" />
        public void Shr()
        {
            InnerEmit.ShiftRight();
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedShiftRight" />
        public void Shr_Un()
        {
            InnerEmit.UnsignedShiftRight();
        }

        /// <summary cref="M:Sigil.Emit`1.SizeOf(System.Type)" />
        public void Sizeof<ValueType>()
        {
            Sizeof(typeof(ValueType));
        }

        /// <summary cref="M:Sigil.Emit`1.SizeOf(System.Type)" />
        public void Sizeof(Type valueType)
        {
            InnerEmit.SizeOf(valueType);
        }

        /// <summary cref="M:Sigil.Emit`1.StoreArgument(System.Int32)" />
        public void Starg(int index)
        {
            InnerEmit.StoreArgument(index);
        }

        /// <summary cref="M:Sigil.Emit`1.StoreElement" />
        public void Stelem()
        {
            InnerEmit.StoreElement();
        }

        /// <summary cref="M:Sigil.Emit`1.StoreField(System.Reflection.FieldInfo, System.Boolean, System.Nullable&lt;int&gt;)" />
        public void Stfld(FieldInfo field, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.StoreField(field, isVolatile, unaligned);
        }

        /// <summary cref="M:Sigil.Emit`1.StoreIndirect(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
        public void Stind<Type>(bool isVolatile = false, int? unaligned = null)
        {
            Stind(typeof(Type), isVolatile, unaligned);
        }

        /// <summary cref="M:Sigil.Emit`1.StoreIndirect(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
        public void Stind(Type type, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.StoreIndirect(type, isVolatile, unaligned);
        }

        /// <summary cref="M:Sigil.Emit`1.StoreLocal(Sigil.Local)" />
        public void Stloc(Local local)
        {
            InnerEmit.StoreLocal(local);
        }

        /// <summary cref="M:Sigil.Emit`1.StoreObject(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
        public void Stobj<ValueType>(bool isVolatile = false, int? unaligned = null)
        {
            Stobj(typeof(ValueType), isVolatile, unaligned);
        }

        /// <summary cref="M:Sigil.Emit`1.StoreObject(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
        public void Stobj(Type valueType, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.StoreObject(valueType, isVolatile, unaligned);
        }

        /// <summary cref="M:Sigil.Emit`1.Subtract" />
        public void Sub()
        {
            InnerEmit.Subtract();
        }

        /// <summary cref="M:Sigil.Emit`1.SubtractOverflow" />
        public void Sub_Ovf()
        {
            InnerEmit.SubtractOverflow();
        }

        /// <summary cref="M:Sigil.Emit`1.UnsignedSubtractOverflow" />
        public void Sub_Ovf_Un()
        {
            InnerEmit.UnsignedSubtractOverflow();
        }

        /// <summary cref="M:Sigil.Emit`1.Switch(Sigil.Label[])" />
        public void Switch(params Label[] labels)
        {
            InnerEmit.Switch(labels);
        }

        /// <summary cref="M:Sigil.Emit`1.Throw" />
        public void Throw()
        {
            InnerEmit.Throw();
        }

        /// <summary cref="M:Sigil.Emit`1.Unbox(System.Type)" />
        public void Unbox<ValueType>()
        {
            Unbox(typeof(ValueType));
        }

        /// <summary cref="M:Sigil.Emit`1.Unbox(System.Type)" />
        public void Unbox(Type valueType)
        {
            InnerEmit.Unbox(valueType);
        }

        /// <summary cref="M:Sigil.Emit`1.UnboxAny(System.Type)" />
        public void Unbox_Any<ValueType>()
        {
            Unbox_Any(typeof(ValueType));
        }

        /// <summary cref="M:Sigil.Emit`1.UnboxAny(System.Type)" />
        public void Unbox_Any(Type valueType)
        {
            InnerEmit.UnboxAny(valueType);
        }

        /// <summary cref="M:Sigil.Emit`1.Xor" />
        public void Xor()
        {
            InnerEmit.Xor();
        }
    }
}
